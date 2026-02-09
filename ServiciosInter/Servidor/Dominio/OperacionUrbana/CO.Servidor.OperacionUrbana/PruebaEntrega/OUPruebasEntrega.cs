using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.MotorReglas;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Transactions;

namespace CO.Servidor.OperacionUrbana.PruebaEntrega
{
    /// <summary>
    /// Clase de dominio para el manejo de las pruebas de entrega
    /// </summary>
    internal class OUPruebasEntrega : ControllerBase
    {
        #region Constructor

        public OUPruebasEntrega()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

        #region Campos

        private const string DIRECCION_ADMISION = "DireccionAdmision";

        public const string DESTINATARIO = "Destinatario";

        public const string LOCALIDAD = "Localidad";

        private static readonly OUPruebasEntrega instancia = (OUPruebasEntrega)FabricaInterceptores.GetProxy(new OUPruebasEntrega(), COConstantesModulos.MODULO_OPERACION_URBANA);

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private ILIFachadaLogisticaInversa fachadaLogisticaInversa = COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversa>();

        private ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna una instancia del administrador de pruebas de entrega
        /// </summary>
        public static OUPruebasEntrega Instancia
        {
            get { return OUPruebasEntrega.instancia; }
        }

        #endregion Propiedades

        #region Consulta

        /// <summary>
        /// Obtiene las planillas donde esta asignado un envio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerPlanillasGuia(long idAdmision)
        {
            return OURepositorio.Instancia.ObtenerPlanillasGuia(idAdmision);
        }


        /// <summary>
        /// Valida si el envío está asignado a una planilla
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia)
        {
            return OURepositorio.Instancia.ConsultaEnvioPlanillaAsignacionGuia(idGuia);
        }

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long puntoServicio)
        {
            return OURepositorio.Instancia.ObtenerMensajeroDescargueAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, puntoServicio);
        }

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerGuiasMensajero(idMensajero);
        }


         /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a un COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return OURepositorio.Instancia.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerUltimaPLanillaMensajero(idMensajero);
        }

        /// <summary>
        /// Retorn el numero de motivos diferente porque ha sido devuelta la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>numero de motivos diferente porque ha sido devuelta la guia</returns>
        public OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo)
        {
            OUMotivosDevolucionMensajeDC motivosDevolucion = new OUMotivosDevolucionMensajeDC();
            int cantidadMotivos = OURepositorio.Instancia.ObtenerConteoMotivosDevolucion(numeroGuia);
            string historicoMotivosDev = string.Empty;
            List<string> lista = OURepositorio.Instancia.ObtenerHistoricoMotivosDevolucion(numeroGuia);

            if (lista.Count == 0)
            {
                cantidadMotivos += 1;   
                OURepositorio.Instancia.AuditarDescMensajerosMotivos(numeroGuia, nombreMensajero, NombreMotivo, cantidadMotivos, idCol);                            
            }
            else if (NombreMotivo != lista[lista.Count - 1] && cantidadMotivos >= 1 && lista[lista.Count - 1] != null)
            {
                cantidadMotivos += 1;
                OURepositorio.Instancia.AuditarDescMensajerosMotivos(numeroGuia, nombreMensajero, NombreMotivo, cantidadMotivos, idCol);                
            }
            else
                OURepositorio.Instancia.AuditarDescMensajerosMotivos(numeroGuia, nombreMensajero, NombreMotivo, cantidadMotivos, idCol);

            List<string> lista2 = OURepositorio.Instancia.ObtenerHistoricoMotivosDevolucion(numeroGuia);
            historicoMotivosDev = string.Join(", ", lista2.ToArray());
            motivosDevolucion.cantidadMotivos = cantidadMotivos;
            motivosDevolucion.historicoMotivos = historicoMotivosDev;
            return motivosDevolucion;
        }

        #endregion Consulta

        #region Adición

        public OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega)
        {
            ADGuia guiaAdmision = new ADGuia();
            OUNombresMensajeroDC mensajero = new OUNombresMensajeroDC();

            if (guia == null)
            {
                ControllerException excepcion =
                new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(),
                OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA));
                throw new FaultException<ControllerException>(excepcion);
            }
            else
            {
                try
                {
                    OUPlanillaAsignacionMensajero planillaGuia = OURecogidas.Instancia.ObtenerUltimaPlanillaMensajeroGuia(guia.NumeroGuia.Value);

                    if (planillaGuia != null)
                    {
                        if (planillaGuia.EstadoEnPlanilla == OUConstantesOperacionUrbana.ESTADO_PLANILLADA)
                        {
                            guia.IdMensajero = planillaGuia.Mensajero.IdMensajero;
                            guia.Planilla = planillaGuia.IdPlanillaAsignacionEnvio;
                            guia.EstaPlanillada = true;

                            if (guia.IdMensajero == 0)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, "0", "La guía se encuentra asociada a la planilla No. " + planillaGuia.IdPlanillaAsignacionEnvio + " pero no ha sido asignada a ningun mensajero"));

                            guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                            guia.IdAdmision = guiaAdmision.IdAdmision;
                            guia.EsAlCobro = guiaAdmision.EsAlCobro;
                            mensajero = OURepositorio.Instancia.ObtenerMensajeroporId(guia.IdMensajero);
                            if (guiaAdmision.IdServicio == (int)TAConstantesServicios.SERVICIO_NOTIFICACIONES && guia.NuevoEstadoGuia == OUConstantesOperacionUrbana.ESTADO_ENTREGADO_PLANILLA)
                            {
                                if (validaEntrega)
                                {
                                    if (COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversaPruebasEntrega>().ValidarRecibidoGuia(guia.NumeroGuia.Value))
                                        if (guia.Notificacion == null)
                                            return OUEnumValidacionDescargue.Notificacion;
                                        else
                                        {
                                            guia.Notificacion.IdGuia = guiaAdmision.IdAdmision;
                                            GuardarResultadoNotificacion(guia.Notificacion);
                                        }
                                }
                                GuardarGuia(guia, guiaAdmision, mensajero);
                                return OUEnumValidacionDescargue.Exitosa;
                            }
                            else if (guiaAdmision.IdServicio == (int)TAConstantesServicios.SERVICIO_RAPIRADICADO)
                                if (guia.RadicadoVerificado)
                                    return GuardarGuia(guia, guiaAdmision, mensajero);
                                else
                                    return OUEnumValidacionDescargue.Rapiradicado;
                            else
                                return GuardarGuia(guia, guiaAdmision, mensajero);
                        }
                        else
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, "0", "La guía ya se encuentra descargada de la planilla No. " + planillaGuia.IdPlanillaAsignacionEnvio));
                        }
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, "0", "La guía que intenta descargar no pertenece a ninguna planilla"));
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FaultException<ControllerException>)
                    {
                        if ((ex as FaultException<ControllerException>).Detail.TipoError == ETipoErrorFramework.EX_REGISTRO_YA_EXISTE.ToString())
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, ETipoErrorFramework.EX_REGISTRO_YA_EXISTE.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_LA_GUIA_YA_ESTA_DESCARGADA)));
                        else
                            throw;
                    }
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        private OUEnumValidacionDescargue GuardarGuia(OUGuiaIngresadaDC guia, ADGuia guiaAdmision, OUNombresMensajeroDC mensajero)
        {
            ADTrazaGuia estadoGuia;
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (guia.EstaPlanillada)
                {
                    //cambia estado de la guía en la  planilla
                    OURepositorio.Instancia.ActualizarGuiaMensajero(guia);
                }
                else
                {
                    guia.IdAdmision = guiaAdmision.IdAdmision;
                    guia.RecogeDinero = guiaAdmision.EsAlCobro;
                    guia.TotalPiezas = guiaAdmision.TotalPiezas;
                    guia.ValorTotal = guiaAdmision.ValorTotal;
                    guia.Consecutivo = 1;
                    guia.CantidadRadicados = 0;
                    guia.TelefonoDestinatario = guiaAdmision.TelefonoDestinatario;
                    guia.DireccionDestinatario = guiaAdmision.DireccionDestinatario;
                    guia.CreadoPor = ControllerContext.Current.Usuario;
                    guia.DetalleGuia = new OUDetalleGuiaDC()
                    {
                        CiudadDestino = guiaAdmision.NombreCiudadDestino,
                        CiudadOrigen = guiaAdmision.NombreCiudadOrigen,
                        IdCiudadDestino = guiaAdmision.IdCiudadDestino,
                        IdCiudadOrigen = guiaAdmision.IdCiudadOrigen,
                        IdServicio = guiaAdmision.IdServicio,
                        NombreServicio = guiaAdmision.NombreServicio,
                        IdTipoEnvio = guiaAdmision.IdTipoEnvio,
                        TipoEnvio = guiaAdmision.NombreTipoEnvio,
                    };
                    guia.EstadoGuiaPlanilla = guia.NuevoEstadoGuia;
                    OUPlanillaAsignacionDC guiaPlanilla = new OUPlanillaAsignacionDC
                    {
                        IdPlanillaAsignacion = guia.Planilla,
                        Guias = guia,
                    };

                    OURepositorio.Instancia.GuardarGuiaPlanillaAsignacion(guiaPlanilla);

                    OUAdministradorOperacionUrbana.Instancia.ActualizaTotalEnviosPlanillados(guiaPlanilla.IdPlanillaAsignacion);

                    //Actualiza total de guías de la planilla
                }

                if (guia.NuevoEstadoGuia == OUConstantesOperacionUrbana.ESTADO_ENTREGADO_PLANILLA)
                {
                    //Confirma direccion de entrega
                    fachadaMensajeria.ConfirmarDireccion(guia.NumeroGuia.Value, ControllerContext.Current.Usuario, true, false);

                    //Cambia estado de la guía
                    estadoGuia = CambiarEstado(guia);

                    if (guiaAdmision.TipoCliente != ADEnumTipoCliente.INT)
                    {
                        guiaAdmision.IdCentroServicioDestino = guia.IdCentroServicioDestino;

                        //Genera comision por entrega
                        GuardarComisionEntrega(guiaAdmision, CMEnumTipoComision.Entregar);
                    }

                    //Envia correo electronico
                    if (guiaAdmision.NotificarEntregaPorEmail && !string.IsNullOrEmpty(guiaAdmision.Remitente.Email))
                    {
                        InformacionAlerta informacionAlerta = PAParametros.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_ENTREGA_EXITOSA);
                        string mensaje = String.Format(informacionAlerta.Mensaje, String.Concat(guiaAdmision.Destinatario.Nombre, ' ', guiaAdmision.Destinatario.Apellido1), Environment.NewLine);
                        PAEnvioCorreoAsyn.Instancia.EnviarCorreoAsyn(guiaAdmision.Remitente.Email, informacionAlerta.Asunto, mensaje);
                    }
                }
                else if (guia.NuevoEstadoGuia == OUConstantesOperacionUrbana.ESTADO_DEVOLUCION)
                {
                    if (guia.Motivo.Tipo == ADEnumTipoMotivoDC.DevolucionesSupervisor && !guiaAdmision.Supervision)
                    {
                        ControllerException excepcion =
                          new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_SUPERVISADA.ToString(),
                          OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_SUPERVISADA));
                        throw new FaultException<ControllerException>(excepcion);

                        //Genera excepcion cuando se guarda con motivo supervision y la guia no esta supervisada
                    }

                    if (guia.EsAlCobro)
                    {
                        IOUFachadaOperacionUrbana fachaOperUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

                        fachaOperUrbana.NivelarCuentasMensajerosACeroXFactura(guiaAdmision.NumeroGuia, "Ajuste de al cobro a mensajero por devolución del envio", (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO);
                        //GuardarTransaccionMensajero(guiaAdmision, (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO, false, "Se descuenta al cobro a mensajero por devolución del envio", mensajero);
                    }
                    //Descuenta valor de Alcobro a cuenta de mensajero

                    estadoGuia = CambiarEstado(guia);

                    //cambia estado

                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        IdTrazaGuia = estadoGuia.IdTrazaGuia,
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty
                    };

                    //inserta el estado motivo de la devolución
                    EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                    //Se valida si el motivo causa supervision, se actualiza la admisión
                    if (guia.Motivo.CausaSupervision)
                        fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);

                    if (guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                    {
                        guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                        guia.EvidenciaDevolucion.EstaDigitalizado = false;
                        guia.EvidenciaDevolucion.IdEvidenciaDevolucion = fachadaMensajeria.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);

                        // se quita validación de los volantes de devolución
                        //SUPropietarioGuia duenoVolante = fachadaSuministros.ObtenerPropietarioSuministro(guia.EvidenciaDevolucion.NumeroEvidencia, (SUEnumSuministro)(guia.EvidenciaDevolucion.TipoEvidencia.IdSuministro), guia.IdCentroServicioOrigen);
                        //SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
                        //{
                        //    Cantidad = 1,
                        //    EstadoConsumo = SUEnumEstadoConsumo.CON,
                        //    GrupoSuministro = SUEnumGrupoSuministroDC.MEN,
                        //    IdDuenoSuministro = duenoVolante.Id,
                        //    IdServicioAsociado = TAConstantesServicios.SERVICIO_MENSAJERIA,
                        //    NumeroSuministro = guia.EvidenciaDevolucion.NumeroEvidencia,
                        //    Suministro = (SUEnumSuministro)(guia.EvidenciaDevolucion.TipoEvidencia.IdSuministro)
                        //};

                        // Inserta consumo del suministro usado
                        // GuardarConsumoVolante(consumo);
                        //if (!string.IsNullOrEmpty(guia.EvidenciaDevolucion.Archivo.NombreServidor))
                        //    fachadaMensajeria.AdicionarArchivo(guia.EvidenciaDevolucion.Archivo);
                    }

                    //Valida si existe alguna regla asociada a el motivo para ejecutarla
                    if (!string.IsNullOrEmpty(guia.Motivo.nombreAssembly))
                    {
                        IDictionary<string, object> parametrosRegla = new Dictionary<string, object>();
                        if (guia.Motivo.IdMotivoGuia == 5 || guia.Motivo.IdMotivoGuia == 6)
                        {
                            parametrosRegla.Add(DIRECCION_ADMISION, guiaAdmision.Destinatario.Direccion);
                            parametrosRegla.Add(DESTINATARIO, "true");
                            parametrosRegla.Add(LOCALIDAD, guiaAdmision.IdCiudadDestino);
                        }
                        RespuestaEjecutorReglas resultado = Ejecutor.EjecutarRegla(guia.Motivo.nombreAssembly, guia.Motivo.@namespace, guia.Motivo.nombreClase, parametrosRegla);
                        if (resultado.HuboError)
                        {
                            if (resultado.ParametrosRegla.ContainsKey(ClavesReglasFramework.EXCEPCION))
                                throw new FaultException<ControllerException>((ControllerException)resultado.ParametrosRegla[ClavesReglasFramework.EXCEPCION]);
                        }
                    }

                    // Consulta las reglas asociadas al motivo para envio de correo
                    // EnviarCorreoAsync(guia.Motivo, guiaAdmision);    
                }

                //actualiza intentos de entrega
                fachadaMensajeria.ActualizarReintentosEntrega(guiaAdmision.IdAdmision);

                transaccion.Complete();
                return OUEnumValidacionDescargue.Exitosa;
            }
        }

        /// <summary>
        /// Método para actualizar el estado entregado de una guia de una planilla
        /// </summary>
        /// <param name="guia"></param>
        public bool ActualizarGuiaPlanilla(long numeroGuia)
        {
            ADGuia guiaAdmision;
            OUGuiaIngresadaDC guia;

            guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(numeroGuia);

            guia = OURepositorio.Instancia.ObtenerGuiaPlanilla(numeroGuia);

            if (guia != null)
            {
                guia.NuevoEstadoGuia = OUConstantesOperacionUrbana.ESTADO_ENTREGADO_PLANILLA;
                guia.EstaDescargada = true;
                using (TransactionScope transaccion = new TransactionScope())
                {
                    OURepositorio.Instancia.ActualizarGuiaMensajero(guia);

                    //if (guiaAdmision.TipoCliente != ADEnumTipoCliente.INT)
                    //{
                    //    //Genera comision por entrega
                    //    GuardarComisionEntrega(guiaAdmision, CMEnumTipoComision.Entregar);
                    //}
            
                    //Envia correo electronico
                    if (guiaAdmision.NotificarEntregaPorEmail && !string.IsNullOrEmpty(guiaAdmision.Remitente.Email))
                    {
                        InformacionAlerta informacionAlerta = PAParametros.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_ENTREGA_EXITOSA);
                        string mensaje = String.Format(informacionAlerta.Mensaje, String.Concat(guiaAdmision.Destinatario.Nombre, ' ', guiaAdmision.Destinatario.Apellido1), Environment.NewLine);
                        PAEnvioCorreoAsyn.Instancia.EnviarCorreoAsyn(guiaAdmision.Remitente.Email, informacionAlerta.Asunto, mensaje);
                    }
                    transaccion.Complete();
                    return true;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Método que guarda el resultado del descargue de una notificación
        /// </summary>
        /// <param name="notificacion"></param>
        private void GuardarResultadoNotificacion(LIRecibidoGuia notificacion)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                fachadaLogisticaInversa.RegistrarRecibidoGuiaManual(notificacion);
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Método para el cambio de estado de la guía en admisiones
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        private ADTrazaGuia CambiarEstado(OUGuiaIngresadaDC guia)
        {
            string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;

            //cambia estado en admision
            ADTrazaGuia estadoGuia = new ADTrazaGuia
            {
                Ciudad = guia.Ciudad,
                IdCiudad = guia.IdCiudad,
                IdAdmision = guia.IdAdmision,
                IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                IdNuevoEstadoGuia = ValidarEstado(guia.NuevoEstadoGuia),
                Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                NumeroGuia = guia.NumeroGuia,
                Observaciones = observaciones,
            };

            estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
            if (estadoGuia.IdTrazaGuia == 0)
            {
                string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                //no pudo realizar el cambio de estado
                ControllerException excepcion =
                              new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                              OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                throw new FaultException<ControllerException>(excepcion);
            }

            // Traza impreso asocia el cambio de estado con el documento que lo genero
            ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
            {
                IdTrazaGuia = estadoGuia.IdTrazaGuia,
                NumeroImpreso = guia.Planilla,
                TipoImpreso = guia.TipoImpreso,
                Usuario = ControllerContext.Current.Usuario,
                FechaGrabacion = DateTime.Now,
            };

            if (estadoGuia.IdNuevoEstadoGuia == (short)(ADEnumEstadoGuia.IntentoEntrega))
            {
                estadoGuia.IdTrazaGuia = 0;
                estadoGuia.IdEstadoGuia = (short)(ADEnumEstadoGuia.IntentoEntrega);

                //obtiene el proximo estado a partir del actual y el id del motivo
                estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), 1);
                estadoGuia.FechaGrabacion = DateTime.Now.AddMilliseconds(500);
                estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);

                if (estadoGuia.IdTrazaGuia == 0)
                {
                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                    //no pudo realizar el cambio de estado
                    ControllerException excepcion =
                                  new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                                  OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                    throw new FaultException<ControllerException>(excepcion);
                }
            }

            trazaImpreso.IdTrazaGuia = estadoGuia.IdTrazaGuia;
            EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

            if (!guia.EstaPlanillada)
            {
                //Verifica si está planillada la guia, si lo está, la descarga al mensajero que la tiene
                List<OUGuiaIngresadaDC> guiaPlani = OURepositorio.Instancia.ObtenerPlanillasGuia(guia.IdAdmision);
                if (guiaPlani != null)
                {
                    guiaPlani.ForEach(g =>
                        {
                            if (guia.Planilla != g.Planilla)
                            {
                                g.NuevoEstadoGuia = guia.NuevoEstadoGuia;
                                g.EstaDescargada = guia.EstaDescargada;
                                //cambia estado de la guía en la  planilla
                                OURepositorio.Instancia.ActualizarGuiaMensajero(g);
                            }
                        });
                }
            }

            return estadoGuia;
        }

        //public void EnviarCorreoAsync(ADMotivoGuiaDC motivo, ADGuia guiaAdmision)
        //{
        //    if (!string.IsNullOrEmpty(guiaAdmision.Remitente.Email) && !string.IsNullOrEmpty(motivo.nombreClase))
        //    {
        //        IDictionary<string, object> parametrosRegla = new Dictionary<string, object>();
        //        parametrosRegla.Add(PAConstantesParametros.PARAMETRO_NOMBRE_DESTINATARIO, String.Concat(guiaAdmision.Destinatario.Nombre, ' ', guiaAdmision.Destinatario.Apellido1));
        //        parametrosRegla.Add(PAConstantesParametros.PARAMETRO_CORREO_ELECTRONICO, guiaAdmision.Remitente.Email);

        //        RespuestaEjecutorReglas resultado = Ejecutor.EjecutarRegla(motivo.nombreAssembly, motivo.@namespace, motivo.nombreClase, parametrosRegla);
        //        if (resultado.HuboError)
        //        {
        //            if (resultado.ParametrosRegla.ContainsKey(ClavesReglasFramework.EXCEPCION))
        //                throw new FaultException<ControllerException>((ControllerException)resultado.ParametrosRegla[ClavesReglasFramework.EXCEPCION]);
        //        }
        //    }
        //}

        #endregion Adición

        #region Validación

        ///// <summary>
        ///// Guarda la guia de mensajería como consumida
        ///// </summary>
        ///// <param name="guia"></param>
        //private void GuardarConsumoVolante(SUConsumoSuministroDC consumo)
        //{
        //    fachadaSuministros.GuardarConsumoSuministro(consumo);
        //}

        /// <summary>
        /// Método para validar el cambio de estado en admisión de acuerdo al estado en la planilla
        /// </summary>
        /// <param name="estadoGuiaManifiesto"></param>
        /// <returns></returns>
        private short ValidarEstado(string estadoGuiaManifiesto)
        {
            if (estadoGuiaManifiesto == OUConstantesOperacionUrbana.ESTADO_ENTREGADO_PLANILLA)
                return (short)(ADEnumEstadoGuia.Entregada);
            if (estadoGuiaManifiesto == OUConstantesOperacionUrbana.ESTADO_DEVOLUCION)
                return (short)(ADEnumEstadoGuia.IntentoEntrega);
            else
                return 0;
        }

        /// <summary>
        /// Método para afectar la cuenta del mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="conceptoCaja"></param>
        /// <param name="esIngreso"></param>
        public void GuardarTransaccionMensajero(ADGuia guia, int conceptoCaja, bool esIngreso, string Observaciones, OUNombresMensajeroDC Mensajero)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

            fachadaCajas.AdicionarTransaccMensajero(new CACuentaMensajeroDC()
            {
                ConceptoEsIngreso = esIngreso,
                FechaGrabacion = DateTime.Now,
                ConceptoCajaMensajero = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = conceptoCaja
                },
                Mensajero = Mensajero,
                NumeroDocumento = guia.NumeroGuia,
                Valor = guia.ValorTotal,
                UsuarioRegistro = ControllerContext.Current.Usuario,
                Observaciones = Observaciones
            });
        }

        /// <summary>
        /// Método para calcular la comision de entrega de un envio
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="tipoComision"></param>
        private void GuardarComisionEntrega(ADGuia guia, CMEnumTipoComision tipoComision)
        {
            if (guia.ValorServicio > 0)
            {
                CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
             new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
             {
                 IdCentroServicios = guia.IdCentroServicioDestino,
                 IdServicio = guia.IdServicio,
                 TipoComision = tipoComision,
                 ValorBaseComision = guia.ValorServicio,
                 NumeroOperacion = guia.NumeroGuia,
             });
                fachadaComisiones.GuardarComision(comision);
            }
        }

        #endregion Validación

        #region Deshacer

        /// <summary>
        /// Método para deshacer la entrega exitosa de una prueba de entrega
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public void DeshacerEntrega(OUGuiaIngresadaDC guia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                //cambia estado en admision
                ADTrazaGuia estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guia.Ciudad,
                    IdCiudad = guia.IdCiudad,
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                    IdNuevoEstadoGuia = (short)(ADEnumEstadoGuia.EnReparto),
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = string.Empty,
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                if (estadoGuia.IdTrazaGuia == 0)
                {
                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                    //no pudo realizar el cambio de estado
                    ControllerException excepcion =
                                  new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                                  OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                    throw new FaultException<ControllerException>(excepcion);
                }

                //cambia estado de la guía en la  planilla
                OURepositorio.Instancia.ActualizarGuiaMensajero(guia);

                if (guia.RecogeDinero)

                //validar Alcobro
                {
                    //no pudo realizar el cambio de estado
                    ControllerException excepcion =
                                  new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_DESHACER_DESCARGUE_INVALIDO.ToString(),
                                  OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_DESHACER_DESCARGUE_INVALIDO));
                    throw new FaultException<ControllerException>(excepcion);
                }

                transaccion.Complete();
            }
        }

        #endregion Deshacer
    }
}