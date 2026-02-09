using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.MotorReglas;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;

namespace CO.Servidor.LogisticaInversa.Telemercadeo
{
    public class LIConfiguradorPlanillas : ControllerBase
    {
        private static readonly LIConfiguradorPlanillas instancia = (LIConfiguradorPlanillas)FabricaInterceptores.GetProxy(new LIConfiguradorPlanillas(), COConstantesModulos.TELEMERCADEO);

        public static LIConfiguradorPlanillas Instancia
        {
            get { return LIConfiguradorPlanillas.instancia; }
        }


        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private IADFachadaAdmisionesMensajeria fachada = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();


        #region Consultas

        /// <summary>
        /// Método para obtener planillas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IList<LIPlanillaDC> ObtenerPlanillas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, ADEnumTipoImpreso tipoImpreso)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerPlanillas(filtro, indicePagina, registrosPorPagina, tipoImpreso);
        }

        /// <summary>
        /// Método para obtener las guias de una planilla
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public IList<LIPlanillaDetalleDC> ObtenerGuiasPlanilla(LIPlanillaDC planilla)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerGuiasPlanilla(planilla);
        }

        public IList<LISalidaCustodia> ObtenerSalidasCustodiaPorDia(long idCentroServicio, DateTime fechaConsulta)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerSalidasCustodiaPorDia(idCentroServicio, fechaConsulta);
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar admisiones en una planilla de nuevas facturas
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanilla(LIPlanillaDetalleDC guia)
        {
            IADFachadaAdmisionesMensajeria fachada = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            ADRangoTrayecto RangoCasillero = new ADRangoTrayecto();
            ADGuia guiaAdmision = fachada.ObtenerGuiaXNumeroGuia(guia.AdmisionMensajeria.NumeroGuia);

            SUNumeradorPrefijo numeroSuministro = new SUNumeradorPrefijo();
            TAServicioDC Servicio = new TAServicioDC();

            if (guiaAdmision.NumeroGuia != 0)
            {
                if (guiaAdmision.IdServicio == (int)TAEnumServiciosDC.Notificaciones)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                   LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION.ToString(),
                   LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION));
                    throw new FaultException<ControllerException>(excepcion);
                }

                if (guiaAdmision.Peso <= 2)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                   LOIEnumTipoErrorLogisticaInversa.EX_ERROR_PESO_GUIA.ToString(),
                   LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_PESO_GUIA));
                    throw new FaultException<ControllerException>(excepcion);
                }


                LIRepositorioTelemercadeo.Instancia.ValidarGuiaPlanilla(guia.AdmisionMensajeria.NumeroGuia);

                if (guiaAdmision.TipoCliente == ADEnumTipoCliente.CRE || guiaAdmision.TipoCliente == ADEnumTipoCliente.CPE || guiaAdmision.TipoCliente == ADEnumTipoCliente.PCO)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CLIENTE_GUIA_DEVOLUCION.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CLIENTE_GUIA_DEVOLUCION));
                    throw new FaultException<ControllerException>(excepcion);
                }

                Servicio = fachadaTarifas.ObtenerDatosServicio(guiaAdmision.IdServicio);

                if (!EstadosGuia.ValidarEstadoGuia(guiaAdmision.NumeroGuia, (short)ADEnumEstadoGuia.DevolucionRatificada) && !EstadosGuia.ValidarEstadoGuia(guiaAdmision.NumeroGuia, (short)ADEnumEstadoGuia.Custodia))
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV));
                    throw new FaultException<ControllerException>(excepcion);
                }

                using (TransactionScope transaccion = new TransactionScope())
                {

                    PUCentroServiciosDC CentroOrigen = fachadaCentroServicios.ObtenerCentroServicio(guia.GuiaInterna.IdCentroServicioOrigen);

                    ADMensajeriaTipoCliente peaton = new ADMensajeriaTipoCliente
                    {
                        PeatonDestinatario = new ADPeaton
                        {
                            Apellido1 = string.Empty,
                            Apellido2 = string.Empty,
                            Direccion = guiaAdmision.Remitente.Direccion,
                            Email = string.Empty,
                            Identificacion = guiaAdmision.Remitente.Identificacion,
                            Nombre = guiaAdmision.Remitente.Nombre,
                            Telefono = guiaAdmision.Remitente.Telefono,
                            TipoIdentificacion = guiaAdmision.Remitente.TipoId,
                        },
                        PeatonRemitente = new ADPeaton
                        {
                            Apellido1 = string.Empty,
                            Apellido2 = string.Empty,
                            Direccion = CentroOrigen.Direccion,
                            Email = string.Empty,
                            Identificacion = guia.GuiaInterna.IdCentroServicioOrigen.ToString(),
                            Nombre = guia.GuiaInterna.NombreCentroServicioOrigen,
                            Telefono = CentroOrigen.Telefono1,
                            TipoIdentificacion = ConstantesFramework.TIPO_DOCUMENTO_NIT,
                        },
                    };
                    ADGuia guiaAdmisionNueva = fachada.ObtenerGuiaXNumeroGuia(guiaAdmision.NumeroGuia);
                    guiaAdmisionNueva.EsAlCobro = true;
                    guiaAdmisionNueva.GuidDeChequeo = Guid.NewGuid().ToString();
                    guiaAdmisionNueva.EstaPagada = false;
                    guiaAdmisionNueva.TipoCliente = ADEnumTipoCliente.PPE;
                    if (guia.AdmisionMensajeriaNueva == null)
                    {
                            guiaAdmisionNueva.Destinatario = guiaAdmision.Remitente;
                        }
                    else if (guia.AdmisionMensajeriaNueva != null)
                    {
                        guiaAdmisionNueva.Destinatario.TipoId = guia.AdmisionMensajeriaNueva.Destinatario.TipoId;
                        guiaAdmisionNueva.Destinatario.Identificacion = guia.AdmisionMensajeriaNueva.Destinatario.Identificacion;
                        guiaAdmisionNueva.Destinatario.Nombre = guia.AdmisionMensajeriaNueva.Destinatario.Nombre;
                        guiaAdmisionNueva.Destinatario.Apellido1 = string.Empty;
                        guiaAdmisionNueva.Destinatario.Apellido2 = string.Empty;
                        guiaAdmisionNueva.Destinatario.Direccion = guia.AdmisionMensajeriaNueva.Destinatario.Direccion;
                        guiaAdmisionNueva.Destinatario.Telefono = guia.AdmisionMensajeriaNueva.Destinatario.Telefono;
                        guiaAdmisionNueva.Destinatario.Email = guia.AdmisionMensajeriaNueva.Destinatario.Email;
                        guiaAdmisionNueva.DireccionDestinatario = guia.AdmisionMensajeriaNueva.Destinatario.Direccion;

                        peaton.PeatonDestinatario.Apellido1 = string.Empty;
                        peaton.PeatonDestinatario.Apellido2 = string.Empty;
                        peaton.PeatonDestinatario.Direccion = guia.AdmisionMensajeriaNueva.Destinatario.Direccion;
                        peaton.PeatonDestinatario.Email = guia.AdmisionMensajeriaNueva.Destinatario.Email;
                        peaton.PeatonDestinatario.Identificacion = guia.AdmisionMensajeriaNueva.Destinatario.Identificacion;
                        peaton.PeatonDestinatario.Nombre = guia.AdmisionMensajeriaNueva.Destinatario.Nombre;
                        peaton.PeatonDestinatario.Telefono = guia.AdmisionMensajeriaNueva.Destinatario.Telefono;
                        peaton.PeatonDestinatario.TipoIdentificacion = guia.AdmisionMensajeriaNueva.Destinatario.TipoId;
                    }
                    TAServicioDC servicio = new TAServicioDC { IdServicio =guiaAdmisionNueva.IdServicio };
                    PALocalidadDC municipioDestino = new PALocalidadDC { IdLocalidad = guiaAdmisionNueva.IdCiudadOrigen };
                    ADValidacionServicioTrayectoDestino validacionTrayectoDestino = new ADValidacionServicioTrayectoDestino();                    
                    validacionTrayectoDestino = fachada.ValidarServicioTrayectoDestino(guia.GuiaInterna.LocalidadOrigen, municipioDestino, servicio, guia.GuiaInterna.IdCentroServicioOrigen, guiaAdmision.Peso);
                    DateTime FechaEstimadaEntrega = DateTime.Now.AddHours(validacionTrayectoDestino.DuracionTrayectoEnHoras);
                    DateTime FechaEstimadaDigitalizacion = DateTime.Now.AddHours(validacionTrayectoDestino.NumeroHorasDigitalizacion);
                    DateTime FechaEstimadaArchivo = DateTime.Now.AddHours(validacionTrayectoDestino.NumeroHorasArchivo);
                    guiaAdmisionNueva.FechaEstimadaEntrega = new DateTime(FechaEstimadaEntrega.Year, FechaEstimadaEntrega.Month, FechaEstimadaEntrega.Day, 18, 0, 0);
                    guiaAdmisionNueva.FechaEstimadaDigitalizacion = new DateTime(FechaEstimadaDigitalizacion.Year, FechaEstimadaDigitalizacion.Month, FechaEstimadaDigitalizacion.Day, FechaEstimadaDigitalizacion.Hour, 0, 0);
                    guiaAdmisionNueva.FechaEstimadaArchivo = new DateTime(FechaEstimadaArchivo.Year, FechaEstimadaArchivo.Month, FechaEstimadaArchivo.Day, FechaEstimadaArchivo.Hour, 0, 0);
                    RangoCasillero = validacionTrayectoDestino.InfoCasillero; //SE DEJA LA INFORMACION DEL CASILLERO AQUI YA QUE ES LA UNICA VARIABLE QUE NO SE USA COMO EN EL CASO DE LA ADMISION POR SILVERLIGTH       
                    guiaAdmisionNueva.IdConceptoCaja = Servicio.IdConceptoCaja;
                    guiaAdmisionNueva.FormasPago = new List<ADGuiaFormaPago> { new ADGuiaFormaPago { IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO, Valor = guiaAdmision.ValorTotal, Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_AL_COBRO } };
                    guiaAdmisionNueva.IdCodigoUsuario = ControllerContext.Current.CodigoUsuario;
                    guiaAdmisionNueva.IdCentroServicioDestino = guiaAdmisionNueva.IdCentroServicioOrigen;
                    guiaAdmisionNueva.NombreCentroServicioDestino = guiaAdmisionNueva.NombreCentroServicioOrigen;
                    guiaAdmisionNueva.IdCentroServicioOrigen = guia.GuiaInterna.IdCentroServicioOrigen;
                    guiaAdmisionNueva.NombreCentroServicioOrigen = guia.GuiaInterna.NombreCentroServicioOrigen;
                    guiaAdmisionNueva.NumeroGuia = 0;
                    guiaAdmisionNueva.IdAdmision = 0;
                    guiaAdmisionNueva.IdCliente = 0;
                    guiaAdmisionNueva.IdTipoEntrega = "1";
                    guiaAdmisionNueva.DescripcionTipoEntrega = "ENTREGA EN DIRECCION";
                    guiaAdmisionNueva.NumeroBolsaSeguridad = string.Empty;
                    guiaAdmisionNueva.DireccionDestinatario = guiaAdmisionNueva.Remitente.Direccion;
                    guiaAdmisionNueva.IdCiudadDestino = guiaAdmisionNueva.IdCiudadOrigen;
                    guiaAdmisionNueva.NombreCiudadDestino = guiaAdmisionNueva.NombreCiudadOrigen;
                    guiaAdmisionNueva.IdCiudadOrigen = guia.GuiaInterna.LocalidadOrigen.IdLocalidad;                    
                    guiaAdmisionNueva.NombreCiudadOrigen = guia.GuiaInterna.LocalidadOrigen.Nombre;                    
                    guiaAdmisionNueva.TelefonoDestinatario = guiaAdmision.Remitente.Telefono;
                    guiaAdmisionNueva.EsAutomatico = true;
                    guiaAdmisionNueva.DiceContener = "Devolución de la guia " + guiaAdmision.NumeroGuia;
                    guiaAdmisionNueva.Remitente = new CLClienteContadoDC
                    {
                        Apellido1 = string.Empty,
                        Apellido2 = string.Empty,
                        Nombre = guia.GuiaInterna.NombreCentroServicioOrigen,
                        Telefono = CentroOrigen.Telefono1,
                        Direccion = CentroOrigen.Direccion,
                        Identificacion = guia.GuiaInterna.IdCentroServicioOrigen.ToString(),
                        TipoId = ConstantesFramework.TIPO_DOCUMENTO_NIT,
                    };

                    ///VALIDAR AQUI PARA GUARDAR AUTOMATICA O MANUAL
                    ADResultadoAdmision resultadoAdmision;
                    if (guia.DestinatarioModificado != null && guia.DestinatarioModificado == 1)
                    {
                        //SUNumeradorPrefijo numeroSuministroAuto = fachada.ObtenerConsecutivoFacturaVenta();
                        //guiaAdmisionNueva.NumeroGuia = numeroSuministroAuto.ValorActual;
                        guiaAdmisionNueva.IdCiudadDestino = guia.AdmisionMensajeria.IdCiudadDestino;
                        guiaAdmisionNueva.NombreCiudadDestino = guia.AdmisionMensajeria.NombreCiudadDestino;
                        guiaAdmisionNueva.IdCiudadOrigen = guia.AdmisionMensajeria.IdCiudadOrigen;
                        guiaAdmisionNueva.NombreCiudadOrigen = guia.AdmisionMensajeria.NombreCiudadOrigen;
                        //guiaAdmisionNueva.ValorTotal = guia.AdmisionMensajeria.ValorTotal;
                        //guiaAdmisionNueva.ValorServicio = guia.AdmisionMensajeria.ValorTotal;
                        //guiaAdmisionNueva.ValorPrimaSeguro = 0;
                        guiaAdmisionNueva.FormasPagoDescripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_AL_COBRO;
                        guiaAdmisionNueva.FormasPagoIds = TAConstantesServicios.ID_FORMA_PAGO_AL_COBRO.ToString();
                        //resultadoAdmision = fachada.RegistrarGuiaManual(guiaAdmisionNueva, guia.caja, peaton);
                        //guiaAdmisionNueva.IdAdmision = resultadoAdmision.IdAdmision;
                    }

                    if (RangoCasillero != null)
                    {
                        if (RangoCasillero.Rangos != null && RangoCasillero.Rangos.Count == 1) // AEREO
                        {
                            guiaAdmisionNueva.MotivoNoUsoBolsaSeguriDesc = RangoCasillero.Rangos.FirstOrDefault().Casillero;
                        }
                        else
                        {
                            decimal _pesoAUsar = guiaAdmisionNueva.Peso;
                            // Si tipo de envío es sobre carta o sobre manila debe aplicar la regla, de lo contrario: NO
                            if (guiaAdmisionNueva.IdTipoEnvio > 2)
                            {
                                var rangoMayor = RangoCasillero.Rangos.OrderByDescending(r => r.RangoInicial).FirstOrDefault();
                                if (rangoMayor != null)
                                {
                                    _pesoAUsar = rangoMayor.RangoInicial;
                                }
                            }
                            var rango = RangoCasillero.Rangos.FirstOrDefault(r => _pesoAUsar <= r.RangoFinal && _pesoAUsar >= r.RangoInicial);
                            if (rango != null)
                                guiaAdmisionNueva.MotivoNoUsoBolsaSeguriDesc = rango.Casillero;
                        }
                    }

                    resultadoAdmision = fachada.RegistrarGuiaAutomatica(guiaAdmisionNueva, guia.caja, peaton);
                    guiaAdmisionNueva.NumeroGuia = resultadoAdmision.NumeroGuia;
                    guiaAdmisionNueva.IdAdmision = resultadoAdmision.IdAdmision;
                    guia.AdmisionMensajeriaNueva = guiaAdmisionNueva;
                    guia.AdmisionMensajeria = guiaAdmision;
                    guia.IdPlanillaGuia = LIRepositorioTelemercadeo.Instancia.AdicionarGuiaPlanilla(guia);

                  

                    if (guiaAdmision.EsAlCobro == true && guiaAdmision.EstaPagada == false)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO.ToString(),
                        LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    transaccion.Complete();
                }
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(),
                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                throw new FaultException<ControllerException>(excepcion);
            }

            return guia;
        }

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente contado
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaContado(LIPlanillaDetalleDC guia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                
                ADRangoTrayecto rangoCas = new ADRangoTrayecto();
                ADGuia guiaAdmision = fachada.ObtenerGuiaXNumeroGuia(guia.AdmisionMensajeria.NumeroGuia);
                if (guiaAdmision.NumeroGuia > 0)
                {
                    //Ajuste planilla contado no valide tipo cliente
                    //if (guiaAdmision.TipoCliente == ADEnumTipoCliente.CCO || guiaAdmision.TipoCliente == ADEnumTipoCliente.CPE || guiaAdmision.TipoCliente == ADEnumTipoCliente.CRE || guiaAdmision.TipoCliente == ADEnumTipoCliente.PCO)
                    //{
                    //    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA, LOIEnumTipoErrorLogisticaInversa.EX_GUIA_CREDITO.ToString(), LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_CREDITO));
                    //    throw new FaultException<ControllerException>(excepcion);
                    //}

                    if (guiaAdmision.IdServicio == (int)TAEnumServiciosDC.Notificaciones)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                       LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION.ToString(),
                       LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    LIRepositorioTelemercadeo.Instancia.ValidarGuiaPlanilla(guia.AdmisionMensajeria.NumeroGuia);

                    if (!EstadosGuia.ValidarEstadoGuia(guiaAdmision.NumeroGuia, (short)ADEnumEstadoGuia.DevolucionRatificada) && !EstadosGuia.ValidarEstadoGuia(guiaAdmision.NumeroGuia, (short)ADEnumEstadoGuia.Custodia))
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV.ToString(),
                        LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    guia.AdmisionMensajeria = guiaAdmision;
                    PUCentroServiciosDC centroServicio = fachadaCentroServicios.ObtenerCentroServicio(Convert.ToInt64(guia.GuiaInterna.IdCentroServicioOrigen));

                    ADGuiaInternaDC GuiaInterna = new ADGuiaInternaDC()
                    {
                        GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                        DiceContener = LOIConstantesLogisticaInversa.DEVOLUCION_GUIA_NO + guia.AdmisionMensajeria.NumeroGuia,
                        DireccionDestinatario = guiaAdmision.Remitente.Direccion,
                        DireccionRemitente = centroServicio.Direccion,
                        EsManual = false,
                        IdAdmisionGuia = 0,
                        IdCentroServicioOrigen = centroServicio.IdCentroServicio,
                        LocalidadDestino = new PALocalidadDC() { IdLocalidad = guiaAdmision.IdCiudadOrigen, Nombre = guiaAdmision.NombreCiudadOrigen },
                        LocalidadOrigen = guia.GuiaInterna.LocalidadOrigen,
                        NombreCentroServicioOrigen = centroServicio.Nombre,
                        NombreDestinatario = guiaAdmision.Remitente.Nombre,
                        NombreRemitente = centroServicio.Nombre,
                        NumeroGuia = 0,
                        PaisDefault = guia.GuiaInterna.PaisDefault,
                        TelefonoDestinatario = guiaAdmision.Remitente.Telefono,
                        TelefonoRemitente = centroServicio.Telefono1
                    };
                    if (guia.GuiaInterna.EsOrigenGestion)
                        GuiaInterna.GestionOrigen = guia.GuiaInterna.GestionOrigen;
                    else
                        GuiaInterna.GestionOrigen = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty };

                    guia.GuiaInterna = fachada.AdicionarGuiaInterna(GuiaInterna);
                    guia.AdmisionMensajeriaNueva = new ADGuia { IdAdmision = GuiaInterna.IdAdmisionGuia, NumeroGuia = GuiaInterna.NumeroGuia };
                    guia.IdPlanillaGuia = LIRepositorioTelemercadeo.Instancia.AdicionarGuiaPlanilla(guia);

                    if (guiaAdmision.EsAlCobro == true && guiaAdmision.EstaPagada == false)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                         LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO.ToString(),
                          LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
           LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(),
           LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                transaccion.Complete();
            }

            return guia;
        }

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente credito
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaCredito(LIPlanillaDetalleDC guia)
        {
            CLClientesDC cliente;
            LIPlanillaDC planilla;
            ADGuiaInternaDC GuiaInterna;

            using (TransactionScope transaccion = new TransactionScope())
            {
                planilla = LIRepositorioTelemercadeo.Instancia.ObtenerPlanilla(guia.NumeroPlanilla);
                cliente = fachadaClientes.ObtenerClientexNit(planilla.ClienteCredito.Nit);

                ADGuia guiaAdmision = fachada.ObtenerGuiaXNumeroGuiaCredito(guia.AdmisionMensajeria.NumeroGuia);
                if (guiaAdmision.IdAdmision > 0)
                {

                    if (guiaAdmision.TipoCliente == ADEnumTipoCliente.PEA || guiaAdmision.TipoCliente == ADEnumTipoCliente.PPE || guiaAdmision.TipoCliente == ADEnumTipoCliente.INT)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA, LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_CREDITO.ToString(), LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_CREDITO));
                        throw new FaultException<ControllerException>(excepcion);
                    }


                    if (guiaAdmision.IdServicio == (int)TAEnumServiciosDC.Notificaciones)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                       LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION.ToString(),
                       LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION));
                        throw new FaultException<ControllerException>(excepcion);
                    }


                    // LIRepositorioTelemercadeo.Instancia.ValidarGuiaPlanilla(guia.AdmisionMensajeria.NumeroGuia);

                    if (!EstadosGuia.ValidarEstadoGuia(guiaAdmision.NumeroGuia, (short)ADEnumEstadoGuia.DevolucionRatificada) && !EstadosGuia.ValidarEstadoGuia(guiaAdmision.NumeroGuia, (short)ADEnumEstadoGuia.Custodia))
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV.ToString(),
                        LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV));
                        throw new FaultException<ControllerException>(excepcion);
                    }


                    if (guiaAdmision.IdCliente != cliente.IdCliente)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CLIENTE_GUIA.ToString(),
                        LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CLIENTE_GUIA));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    guia.AdmisionMensajeria = guiaAdmision;
                    IList<LIPlanillaDetalleDC> listaGuias = LIRepositorioTelemercadeo.Instancia.ObtenerGuiasPlanilla(new LIPlanillaDC { NumeroPlanilla = guia.NumeroPlanilla });
                    PUCentroServiciosDC centroServicio = fachadaCentroServicios.ObtenerCentroServicio(Convert.ToInt64(guia.GuiaInterna.IdCentroServicioOrigen));

                    if (planilla.EsConsolidado && listaGuias.Any())
                        GuiaInterna = fachada.ObtenerGuiaInterna(listaGuias.FirstOrDefault().AdmisionMensajeriaNueva.NumeroGuia);
                    else
                    {
                        GuiaInterna = new ADGuiaInternaDC()
                     {
                         GestionOrigen = new ARGestionDC(),
                         GestionDestino = new ARGestionDC(),
                         DireccionRemitente = centroServicio.Direccion,
                         IdCentroServicioOrigen = centroServicio.IdCentroServicio,
                         NombreCentroServicioOrigen = centroServicio.Nombre,
                         LocalidadOrigen = guia.GuiaInterna.LocalidadOrigen,
                         NombreRemitente = centroServicio.Nombre,
                         TelefonoRemitente = centroServicio.Telefono1,
                         DireccionDestinatario = guiaAdmision.Remitente.Direccion,
                         IdCentroServicioDestino = guiaAdmision.IdCentroServicioOrigen,
                         NombreCentroServicioDestino = guiaAdmision.NombreCiudadOrigen,
                         LocalidadDestino = new PALocalidadDC() { IdLocalidad = guiaAdmision.IdCiudadOrigen, Nombre = guiaAdmision.NombreCiudadOrigen },
                         NombreDestinatario = guiaAdmision.Remitente.Nombre,
                         TelefonoDestinatario = guiaAdmision.Remitente.Telefono,
                         EsManual = false,
                         IdAdmisionGuia = 0,
                         DiceContener = LOIConstantesLogisticaInversa.DEVOLUCION_GUIA_NO + guia.AdmisionMensajeria.NumeroGuia,
                         NumeroGuia = 0,
                         PaisDefault = guia.GuiaInterna.PaisDefault,
                     };
                        GuiaInterna.NumeroGuia = fachada.AdicionarGuiaInterna(GuiaInterna).NumeroGuia;
                    }

                    guia.GuiaInterna = GuiaInterna;


                    guia.AdmisionMensajeriaNueva = new ADGuia { IdAdmision = GuiaInterna.IdAdmisionGuia, NumeroGuia = GuiaInterna.NumeroGuia };
                    guia.IdPlanillaGuia = LIRepositorioTelemercadeo.Instancia.AdicionarGuiaPlanilla(guia);
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                transaccion.Complete();
            }

            return guia;
        }

        /// <summary>
        /// Método para insertar una planilla de guías internas
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaDC AdicionarPlanilla(LIPlanillaDC planilla)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                planilla.NumeroPlanilla = LIRepositorioTelemercadeo.Instancia.AdicionarPlanilla(planilla);
                transaccion.Complete();
            }
            return planilla;
        }

        /// <summary>
        /// Crea la planilla de devolucion para cliente credito o contado y adiciona la primer guia
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guia"></param>
        /// <param name="planillaCredito"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC CrearPlanillaAdicionarGuia(LIPlanillaDC planilla, LIPlanillaDetalleDC guia, bool planillaCredito)
        {

            using (TransactionScope transaccion = new TransactionScope())
            {


                planilla.NumeroPlanilla = LIRepositorioTelemercadeo.Instancia.AdicionarPlanilla(planilla);

                guia.NumeroPlanilla = planilla.NumeroPlanilla;

                if (planillaCredito)
                {
                    guia = AdicionarGuiaPlanillaCredito(guia);
                }
                else
                {
                    guia = AdicionarGuiaPlanillaContado(guia);
                }

                transaccion.Complete();
            }
            return guia;

        }


        #endregion Inserciones

        #region Eliminaciones

        /// <summary>
        /// Método para eliminar una guia de una planilla con auditoria
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaPLanilla(LIPlanillaDetalleDC guia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                LIRepositorioTelemercadeo.Instancia.EliminarGuiaPLanilla(guia);
                transaccion.Complete();
            }
        }

        #endregion Eliminaciones
    }
}