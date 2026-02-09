using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Facturacion;
using CO.Servidor.Dominio.Comun.Middleware;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.ControlCuentas.Datos;
using System.Transactions;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Cajas.GestionCajas;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Clientes;
using Framework.Servidor.Comun.Util;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.CentroServicios;
using CO.Servidor.Suministros;

namespace CO.Servidor.ControlCuentas
{
    public class CCNovedadesApp : ControllerBase
    {
        #region CrearInstancia

        private static readonly CCNovedadesApp instancia = (CCNovedadesApp)FabricaInterceptores.GetProxy(new CCNovedadesApp(), COConstantesModulos.MODULO_CONTROL_CUENTAS);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static CCNovedadesApp Instancia
        {
            get { return CCNovedadesApp.instancia; }
        }
        #endregion

        //private IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        //private IFAFachadaFacturacion fachadaFacturacion = COFabricaDominio.Instancia.CrearInstancia<IFAFachadaFacturacion>();

        //private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

        //private ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();

        #region Metodos

        /// <summary>
        /// Metodo para obtener cantidades de guias por auditar
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
        public CCRespuestaAuditoriaDC ObtenerCantidadGuiasPorAuditar(long idCentroLogistico)
        {
            return CCRepositorioApp.Instancia.ObtenerCantidadGuiasPorAuditar(idCentroLogistico);
        }

        /// <summary>
        /// Metodo para obtener la informacion de la guia para auditoria de liquidacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public CCRespuestaAuditoriaDC ObtenerGuiaAuditoriaLiquidacion(long numeroGuia)
        {
            CCRespuestaAuditoriaDC respuestaAuditoriaDC = new CCRespuestaAuditoriaDC();
            if (!CCRepositorioApp.Instancia.ConsultarNovedadesControlLiquidacionPorGuia(numeroGuia))
            {
                return CCRepositorioApp.Instancia.ObtenerGuiaAuditoriaLiquidacion(numeroGuia);
            }
            else
            {
                respuestaAuditoriaDC.MensajeRespuesta = "La guía ya fue auditada.";
                respuestaAuditoriaDC.EstadoSolicitud = false;
            }
            return respuestaAuditoriaDC;
        }

        /// <summary>
        /// Metodo para insertar novedades de control de liquidacion 
        /// </summary>
        /// <param name="guia"></param>
        public CCRespuestaAuditoriaDC InsertarNovedadControlLiquidacion(CCGuiaDC guia)
        {
            CCRespuestaAuditoriaDC respuestaAuditoria = new CCRespuestaAuditoriaDC();
            respuestaAuditoria.MensajeRespuesta = "La guía ya fue auditada";
            respuestaAuditoria.EstadoSolicitud = false;
            /*********************Validacion novedad control liquidacion para la guia ****************************/
            if (guia != null)
            {
                if (!CCRepositorioApp.Instancia.ConsultarNovedadesControlLiquidacionPorGuia(guia.NumeroGuia))
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        /*************** INSERCION AUDITORIA CONTROL LIQUIDACION ***********************/
                        int pk = CCRepositorioApp.Instancia.InsertarNovedadControlLiquidacion(guia);
                        CCRepositorioApp.Instancia.InsertarImagenesNovedad(pk, guia.ImagenesAuditoria, guia.NumeroGuia);
                        InsertarNovedadesAuditoriaControlLiquidacion(pk, guia);
                        respuestaAuditoria.MensajeRespuesta = "La novedad ha sido registrada";
                        respuestaAuditoria.NumeroGuia = guia.NumeroGuia;
                        respuestaAuditoria.EstadoSolicitud = true;
                        transaction.Complete();
                    }
                }
            }
            return respuestaAuditoria;
        }

        public bool validarCupoClienteCredito(int idContrato, decimal valorTransaccion)
        {
            
            bool avisoPorcentajeMinimoAviso = CLFachadaClientes.Instancia.ValidarCupoCliente(idContrato, valorTransaccion);
            return avisoPorcentajeMinimoAviso;
        }

        /// <summary>
        /// Metodo para insertar novedades de control de liquidacion de auditoria de pesos, realizando comunicado 
        /// </summary>
        /// <param name="guia"></param>
        public CCRespuestaAuditoriaDC InsertarNovedadAuditoriaPorPeso(CCGuiaDC guia, ADGuiaUltEstadoDC guiaSinAuditar, CCEmpleadoNovaSoftDC empleadoNovasoft, bool guardaTraza)
        {   
            CCRespuestaAuditoriaDC respuestaAuditoria = new CCRespuestaAuditoriaDC();
            respuestaAuditoria.MensajeRespuesta = "La guía ya fue auditada";
            respuestaAuditoria.EstadoSolicitud = false;
            /*********************Validacion novedad control liquidacion para la guia ****************************/
            if (guia != null)

            {
                if (CCRepositorioApp.Instancia.ConsultarNovedadesControlLiquidacionPorGuia(guia.NumeroGuia))
                {   
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        /*************** INSERCION AUDITORIA CONTROL LIQUIDACION ***********************/
                        if (guardaTraza)
                        {
                            int pk = CCRepositorioApp.Instancia.InsertarNovedadControlLiquidacion(guia);
                            CCRepositorioApp.Instancia.InsertarImagenesNovedad(pk, guia.ImagenesAuditoria, guia.NumeroGuia);
                            InsertarNovedadesAuditoriaControlLiquidacion(pk, guia);
                            respuestaAuditoria.MensajeRespuesta = "La novedad ha sido registrada";
                            respuestaAuditoria.NumeroGuia = guia.NumeroGuia;
                            respuestaAuditoria.EstadoSolicitud = true;
                        }
                        
                        //*********Se ingresa afectacion de caja****************/

                        //GCAdministradorCajas.Instancia.IngresarAjusteCajaNovedades(ajusteCaja);

                        //******************************************************
                        GenerarRapsReporteAuditoriaPesos(guia, guiaSinAuditar, empleadoNovasoft);
                        /*****************************Obtener datos  de solicitud acumulativa de rap****************************************/
                        
                        transaction.Complete();
                    }

                    

                }
            }
            return respuestaAuditoria;
        }

        public void GenerarRapsReporteAuditoriaPesos(CCGuiaDC guia, ADGuiaUltEstadoDC guiaSinAuditar, CCEmpleadoNovaSoftDC empleadoNovasoft)
        {
            RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
            RADatosFallaDC datosFalla = null;
            RAFallaMapper ma = new RAFallaMapper();
            PUCentroServiciosDC centroServicio = PUCentroServicios.Instancia.ObtenerCentroServicio(guiaSinAuditar.Guia.IdCentroServicioOrigen);
            switch (guia.TipoAuditoriaPeso)
            {
                case EnumTipoReporteAuditoriaPeso.AuditoriasDiarias:
                    datosFalla = new RADatosFallaDC()
                    {
                     //las auditorias se asignan desde la base de datos, medirlas desde la app aun no esta contemplado  
                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.DescuentoMensajero:
                    datosFalla = new RADatosFallaDC()
                    {
                        DocPersonaResponsable = Convert.ToString(centroServicio.IdentificacionPersonaResponsable),
                        NombreCompleto = Convert.ToString(centroServicio.NombrePersonaResponsable),
                        IdSistema = Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(),
                        NumeroGuia = guia.NumeroGuia,
                        PesoAuditoria = guia.PesoTotalAuditoria,
                        ValorAuditoria = guia.ValorTotalAdmision
                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.EnvioEstados:
                    datosFalla = new RADatosFallaDC()
                    {
                        DocPersonaResponsable = Convert.ToString(centroServicio.IdentificacionPersonaResponsable),
                        NombreCompleto = Convert.ToString(centroServicio.NombrePersonaResponsable),
                        IdSistema = Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(),
                        EstadoGuia = guiaSinAuditar.Guia.EstadoGuia,
                        NumeroGuia = guiaSinAuditar.Guia.NumeroGuia,
                        IdCentroLogistico = guiaSinAuditar.Guia.IdSucursal,
                        FechaAdmision = guiaSinAuditar.Guia.FechaAdmision
                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.ErrorAdmisionFactMan:
                    datosFalla = new RADatosFallaDC()
                    {
                        DocPersonaResponsable = Convert.ToString(centroServicio.IdentificacionPersonaResponsable),
                        NombreCompleto = Convert.ToString(centroServicio.NombrePersonaResponsable),
                        IdSistema = Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(),
                        IdAdmision = guiaSinAuditar.Guia.IdAdmision,
                        CentroSercicioOrigen = guiaSinAuditar.Guia.NombreCentroServicioOrigen,
                        NumeroGuia =  guiaSinAuditar.Guia.NumeroGuia,
                        //Buscar numero de factura
                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.ErrorEdVentaFactMan:
                    datosFalla = new RADatosFallaDC()
                    {
                        //Buscar numero de factura

                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.NotaCredito:
                    datosFalla = new RADatosFallaDC()
                    {
                        DocPersonaResponsable = Convert.ToString(centroServicio.IdentificacionPersonaResponsable),
                        NombreCompleto = Convert.ToString(centroServicio.NombrePersonaResponsable),
                        IdSistema = Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(),
                        NombreCliente = guiaSinAuditar.Guia.Remitente.Nombre,
                        IdCliente = guiaSinAuditar.Guia.Remitente.Identificacion,
                        DiferenciaPeso = guiaSinAuditar.Guia.Peso -guia.PesoTotalAuditoria,
                        ValorTotalAuditoria = guia.ValorTotalAdmision
                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.SinProsupuesto:
                    datosFalla = new RADatosFallaDC()
                    {
                        DocPersonaResponsable = Convert.ToString(centroServicio.IdentificacionPersonaResponsable),
                        NombreCompleto = Convert.ToString(centroServicio.NombrePersonaResponsable),
                        IdSistema = Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(),
                        NombreCliente = guiaSinAuditar.Guia.NombreCliente,
                        IdCliente = Convert.ToString(guiaSinAuditar.Guia.IdCliente)
                    };
                    break;
                case EnumTipoReporteAuditoriaPeso.ValorMayorOMenor:
                    datosFalla = new RADatosFallaDC()
                    {
                        DocPersonaResponsable = Convert.ToString(centroServicio.IdCentroServicio),
                        NombreCompleto = Convert.ToString(centroServicio.Nombre),
                        IdCiudad = Convert.ToString(centroServicio.CiudadUbicacion.IdLocalidad),
                        IdSistema = Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(),
                        NumeroGuia = guiaSinAuditar.Guia.NumeroGuia,
                        NombreCliente = guiaSinAuditar.Guia.NombreCliente,
                        IdCliente = Convert.ToString(guiaSinAuditar.Guia.IdCliente)
                    };

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<br>");
                    sb.AppendLine("<big><b>------------------------------------------</b></big>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<big><b>***************** CORREO DE PRUEBA ! IGNORAR ! *****************</b></big>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<big><b>------------------------------------------</b></big>");
                    sb.AppendLine("<br>");


                    sb.AppendLine("<b>Bogotá, fecha</b>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<b>Señor(a)</b>");
                    sb.AppendLine("<b>" + guiaSinAuditar.Guia.Remitente.NombreYApellidos + "</b>");
                    sb.AppendLine("<b>Razon social</b>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<b>Asunto: Novedades de peso en los envíos admitidos el 00 /xxx(fecha)</ b>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<b>Cordial saludo</b>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<b>Como parte de los procesos de optimización y control de la Calidad en el servicio de INTER  RAPIDISIMO  S.A.se  ha  implementado  un  sistema  de  control  de  pesos  y novedades de  los  envíos  admitidos, proceso  en  el  cual  se  evidencio  que  los  envíos amparados  con  las  Guías  de  Transporte  detalladas  a  continuación  presentan  las siguientes novedades:</b>");
                    sb.AppendLine("<br>");

                    sb.Append("<table><tr><th>Número de guia</th><th>Peso registrado en guia</th><th>Peso verificado</th><td>" + guia.NumeroGuia + "</td><td>" + guiaSinAuditar.Guia.Peso + "</td><td>" + guia.PesoTotalAuditoria + "</td></tr></table>");

                    sb.AppendLine("<b>Es importante  aclarar  que  en  virtud  de  lo  anterior  su  envío  ha  sido  reliquidado  y generará un valor adicional en el cobro de su factura; de igual forma agradecemos a usted el realizar las verificaciones que nos lleven a que este tipo de novedades no se vuelvan a generar, en razón a la sana relación comercial existente entre las partes.</b>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<b>Agradecemos de antemano su valiosa colaboración.</b>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<br>");
                    sb.AppendLine("<b>Firma responsable</b>");
                     
                    string t1 = "<table><tr><th>ID</th><th>N° Factura</th><th>Fecha de admision</th><th>Valor inicial factura</th><th>Peso liquidado</th><th>Peso Bascula</th><th>Largo</th><th>Ancho</th><th>Alto</th>";
                    string t2 = "<td>" + "ID" + "</td><td>" + "<td>" + guia.NumeroGuia + "</td><td>" + guiaSinAuditar.Guia.FechaAdmision + "</td><td>" + guiaSinAuditar.Guia.ValorTotal + "</td><td>" + guiaSinAuditar.Guia.Peso + "</td><td>" + guia.PesoBasculaAuditoria + "</td><td>" + guia.LargoVolumetricoAuditoria + "</td><td>" + guia.AnchoVolumetricoAuditoria + "</td><td>" + guia.AltoVolumetricoAuditoria + "</td></tr></table>";
                    sb.Append(t1 + t2);
                    decimal difpeso = guiaSinAuditar.Guia.Peso - guia.PesoBasculaAuditoria;
                    decimal difvalor = guiaSinAuditar.Guia.ValorTotal - guia.ValorTotalAdmision;
                    t1 = "<table><tr><th>peso volumetrico real</th><th>Diferencia peso liquidado real</th><th>valor envio real </th><th>diferencia valor liquidado</th><th>Porcentaje comision por venta de mensajeria</th><th>Valor comision a pagar por reliquidacion</th><th>Dice contener</th>";
                    t2 = "<td>" + guia.PesoVolumetricoTotalAuditoria + "</td><td>" + "<td>" + difpeso + "</td><td>" + guia.ValorTotalAdmision + "</td><td>" + difvalor + "</td><td>" + "Porcentaje por comision" + "</td><td>" + "Valor comision a pagar" + "</td><td>" + guiaSinAuditar.Guia.DiceContener + "</td></tr></table>";


                    CorreoElectronico.Instancia.Enviar(guiaSinAuditar.Guia.Remitente.Email, "Auditoria de peso(Prueba)", sb.ToString(), empleadoNovasoft.Correo, null, null);

                    break;
            }
            //datosFalla = ma.MapperDatosFallaAutomaticaRecogidas(guia, RAEnumSistemaOrigen.CONTROLLER.GetHashCode());
            
            if (guiaSinAuditar.Guia.EsAutomatico)
            {
                /********************* ES AUTOMATICA *************************/


                if (guiaSinAuditar.Guia.IdMensajero != 0)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Mensajero, CoEnumTipoNovedadRaps.GUIA_MAL_LIQUIDADA_PESO_MENSAJERO_MAN.GetHashCode());
                }
                else
                {
                    /****************************** EMITIDA DESDE OTROS SISTEMAS ********************************/

                    //PUAgenciaDeRacolDC agencia = PUCentroServicios.Instancia.ObtenerAgenciaResponsable(datosGuia.IdCentroServicioOrigen);
                     //RADatosFallaDC datosFalla = null;
                     //RAFallaMapper ma = null;
                     //RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
                     //ma = new RAFallaMapper();
                     //datosFalla = ma.MapperDatosFallaCentroAcopio(centroServicio, Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(), guia.NumeroGuia, numeroEnvioNN);

                    if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.AGE.ToString())
                    {
                        parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias, CoEnumTipoNovedadRaps.GUIA_MAL_LIQUIDADA_PESO_AGENCIA_MAN.GetHashCode());
                    }
                    else if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.PTO.ToString())
                    {
                        parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.GUIA_MAL_LIQUIDADA_PESO_PUNTO_MAN.GetHashCode());
                    }

                }
            }
            else
            {
                /********************* ES MANUAL *************************/
                var datosSuministros = SUAdministradorSuministros.Instancia.ObtenerResponsableSuministro(guiaSinAuditar.Guia.NumeroGuia);


                if (datosSuministros.TipoCentroServicios == PUEnumTipoCentroServicioDC.AGE.ToString())
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias, CoEnumTipoNovedadRaps.GUIA_MAL_LIQUIDADA_PESO_AGENCIA_MAN.GetHashCode());
                }
                else if (datosSuministros.TipoCentroServicios == PUEnumTipoCentroServicioDC.PTO.ToString())
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.GUIA_MAL_LIQUIDADA_PESO_PUNTO_MAN.GetHashCode());
                }
                else if (String.IsNullOrEmpty(datosSuministros.TipoCentroServicios))
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Mensajero, CoEnumTipoNovedadRaps.GUIA_MAL_LIQUIDADA_PESO_MENSAJERO_MAN.GetHashCode());
                }
            }

            /*****************************************CREA SOLICITUD ACUMULATIVA********************************************************/
            if (!parametrosSolicitudAcumulativa.EstaEnviado)
            {
                if (parametrosSolicitudAcumulativa.TipoNovedad != CoEnumTipoNovedadRaps.Pordefecto && parametrosSolicitudAcumulativa.Parametrosparametrizacion.Count > 0)
                {
                    RAIntegracionesRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)parametrosSolicitudAcumulativa.TipoNovedad.GetHashCode(), parametrosSolicitudAcumulativa.Parametrosparametrizacion, datosFalla.IdCiudad.Substring(0, 5), ControllerContext.Current == null ? "MotorRaps" : ControllerContext.Current.Usuario, datosFalla.IdSistema);
                }
            }
        }

        /// <summary>
        /// Metodo para insertar novedades auditoria control liquidacion 
        /// </summary>
        /// <param name="guia"></param>
        public void InsertarNovedadesAuditoriaControlLiquidacion(int pk, CCGuiaDC guia)
        {
            if (guia.TipoNovedades != null)
            {
                foreach (CCEnumTipoNovedadCtrLiquidacionDC item in guia.TipoNovedades)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        CCRepositorioApp.Instancia.InsertarTrazaNovedadControlLiquidacion(pk, guia, item, CCEnumEstadoNovedadCtrlLiquidacionDC.CREADA);
                        CCRepositorioApp.Instancia.InsertarNovedadPorNumeroGuia(pk, guia.NumeroGuia, item);
                        transaction.Complete();
                    }
                }
            }
        }

        /// <summary>
        /// Metodo para insertar novedades auditoria control liquidacion gestionadas 
        /// </summary>
        /// <param name="guia"></param>
        public void InsertarTrazaNovedadControlLiquidacion(int pk, CCGuiaDC guia)
        {
            if (guia.TipoNovedades != null)
            {
                foreach (CCEnumTipoNovedadCtrLiquidacionDC item in guia.TipoNovedades)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        CCRepositorioApp.Instancia.InsertarTrazaNovedadControlLiquidacion(pk, guia, item, CCEnumEstadoNovedadCtrlLiquidacionDC.GESTIONADA);
                        CCRepositorioApp.Instancia.ActualizarNovedadControlLiquidacion(guia.NumeroGuia, item, CCEnumEstadoNovedadCtrlLiquidacionDC.GESTIONADA);
                        transaction.Complete();
                    }
                }
            }
        }
        /// <summary>
        /// Metodo para consultar lista de guias de novedades control liquidacion 
        /// </summary>
        /// <returns></returns>
        public List<CCGuiaIdAuditoria> ConsultarGuiasNovedadesControlLiquidacion(int indicePagina, int registrosPorPagina)
        {
            return CCRepositorioApp.Instancia.ConsultarGuiasNovedadesControlLiquidacion(indicePagina, registrosPorPagina);
        }
        /// <summary>
        /// Metodo para obtener una lista con los tipos de novedades de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<short> ObtenerTipoNovedadesGuia(long numeroGuia, int IdEstadoNovedad)
        {
            return CCRepositorioApp.Instancia.ObtenerTipoNovedadesGuia(numeroGuia, IdEstadoNovedad);
        }
        /// <summary>
        /// Metodo para obtener peso volumetrico de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PesoVolGuiaDC ObtenerPesoVolumetricoGuia(long numeroGuia)
        {
            return CCRepositorioApp.Instancia.ObtenerPesoVolumetricoGuia(numeroGuia);
        }
        /// <summary>
        /// Metodo para obtener peso volumetrico de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PesoVolGuiaDC ObtenerPesoVolumetricoParaGuia(long numeroGuia)
        {
            return CCRepositorioApp.Instancia.ObtenerPesoVolumetricoParaGuia(numeroGuia);
        }
        /// <summary>
        /// obtiene imagenes por numero de guia
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesNovedadGuia(long numeroGuia)
        {
            return CCRepositorioApp.Instancia.ObtenerImagenesNovedadGuia(numeroGuia);
        }
    }
    #endregion
}