using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.ExploradorGiros;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.LogisticaInversa.Notificaciones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using System.Linq;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Dominio.Comun.LogisticaInversa;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega
{
    //clase de negocio para logistica inversa
    internal class LIConfiguradorPruebasEntrega : ControllerBase
    {
        private static readonly LIConfiguradorPruebasEntrega instancia = (LIConfiguradorPruebasEntrega)FabricaInterceptores.GetProxy(new LIConfiguradorPruebasEntrega(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

        public static LIConfiguradorPruebasEntrega Instancia
        {
            get { return LIConfiguradorPruebasEntrega.instancia; }
        }

        public LIConfiguradorPruebasEntrega()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

        private IOUFachadaOperacionUrbana fachadaOpUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        private ICAFachadaCentroAcopio fachadaCentroAcopio = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCentroAcopio>();

        private ILIFachadaLogisticaInversa fachadaLogisticaInversa = COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversa>();


        #region Manifiesto

        #region Consultar

        /// <summary>
        /// Método para consultar los tipos de manifiesto
        /// </summary>
        /// <returns>lista con los tipos de manifiesto</returns>
        public IEnumerable<LITipoManifiestoDC> ObtenerTiposManifiesto()
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerTiposManifiesto();
        }

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a una agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns>Lista con los manifiestos filtrados</returns>
        public IEnumerable<LIManifiestoDC> ObtenerManifiestosFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerManifiestosFiltro(filtro, indicePagina, registrosPorPagina);
        }

        #endregion Consultar

        #region Adicionar

        /// <summary>
        /// Metodo para adicionar manifiestos
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns>id del manifiesto generado</returns>
        public long AdicionarManifiesto(LIManifiestoDC manifiesto)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (manifiesto.NumeroGuiaInterna != 0 && manifiesto.NumeroGuiaInterna != null)
                {
                    ADGuiaInternaDC guiaInterna = new ADGuiaInternaDC();
                    guiaInterna = fachadaMensajeria.ObtenerGuiaInterna(manifiesto.GuiaInterna.NumeroGuia);
                    if (guiaInterna != null && guiaInterna.IdAdmisionGuia > 0)
                    {
                        if (manifiesto.GuiaInterna.LocalidadOrigen == null || manifiesto.GuiaInterna.LocalidadDestino == null || guiaInterna.LocalidadOrigen.IdLocalidad != manifiesto.GuiaInterna.LocalidadOrigen.IdLocalidad || guiaInterna.LocalidadDestino.IdLocalidad != manifiesto.GuiaInterna.LocalidadDestino.IdLocalidad)
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_INTERNA_MANIFIESTO.ToString(),
                             LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_INTERNA_MANIFIESTO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                        else
                            manifiesto.GuiaInterna = guiaInterna;
                    }
                    //else
                    //    manifiesto.GuiaInterna = fachadaMensajeria.AdicionarGuiaInterna(manifiesto.GuiaInterna);
                }
                else
                    manifiesto.GuiaInterna = null;

                long NumeroManifiesto = LIRepositorioPruebasEntrega.Instancia.AdicionarManifiesto(manifiesto);
                transaccion.Complete();
                return NumeroManifiesto;
            }
        }

        /// <summary>
        /// Método para insertar guías en un manifiesto
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        public void AdicionarGuiaManifiesto(LIGuiaDC guia)
        {
            LIGuiaDC guiaManifiesto = new LIGuiaDC();
            using (TransactionScope transaccion = new TransactionScope())
            {
                guiaManifiesto = ValidarTipoGuia(guia);
                if (guiaManifiesto.IdGuia != 0)
                {
                    LIRepositorioPruebasEntrega.Instancia.AdicionarGuiaManifiesto(guiaManifiesto);
                    transaccion.Complete();
                }
                else
                {
                    string nombreManifiesto = LIRepositorioPruebasEntrega.Instancia.ConsultaNombreTipoManifiesto(guiaManifiesto.TipoManifiesto);
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                      LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA.ToString(),
                      LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA));
                    excepcion.Mensaje = excepcion.Mensaje + nombreManifiesto;
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        #endregion Adicionar

        #region Eliminar

        /// <summary>
        /// Elimina un manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarManifiesto(LIManifiestoDC manifiesto)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                LIRepositorioPruebasEntrega.Instancia.EliminarManifiesto(manifiesto);
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().EliminarGuiaInterna(manifiesto.GuiaInterna);
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().EliminarAdmision(manifiesto.IdGuiaInterna);
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Elimina una guia asociadad a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaManifiesto(LIGuiaDC guia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (guia.EstadoGuia == LOIConstantesLogisticaInversa.ENTREGA_EXITOSA)
                {
                    //cambia estado en admision
                    ADTrazaGuia estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = guia.Ciudad,
                        IdCiudad = guia.IdCiudad,
                        IdAdmision = guia.IdGuia,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdGuia)),
                        IdNuevoEstadoGuia = (short)(ADEnumEstadoGuia.CentroAcopio),
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                    };
                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                }
                LIRepositorioPruebasEntrega.Instancia.AuditarGuia(guia);
                LIRepositorioPruebasEntrega.Instancia.EliminarGuiaManifiesto(guia);
                transaccion.Complete();
            }
        }

        #endregion Eliminar

        #region Validar

        /// <summary>
        /// Valida si un numero de guia interno esta provisionada en una agencia
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="tipoSuministro"></param>
        /// <param name="idCentroServicio"></param>
        public bool ValidarGuiaInterna(long numeroGuia, int tipoSuministro, long idCentroServicio)
        {
            SUSuministro suministro = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ValidarSuministroSerial(numeroGuia, tipoSuministro, idCentroServicio);
            if (suministro.Id == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Método para validar que una guía o un giro existan en sus respectivos modelos
        /// </summary>
        /// <param name="guia"></param>
        /// <returns>bool de validación</returns>
        private LIGuiaDC ValidarTipoGuia(LIGuiaDC guia)
        {
            LIGuiaDC guiaRetorna = new LIGuiaDC();

            if (guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_PRUEBAS_ENTREGA || guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_FACTURA_MENSAJERIA)
            {
                guiaRetorna.IdGuia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value).IdAdmision;
            }

            if (guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_FACTURA_GIROS)
            {
                guiaRetorna.IdGuia = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaExploradorGiros>().ValidarGiro(guia.NumeroGuia.Value);
                guiaRetorna.IdServicio = TAConstantesServicios.SERVICIO_GIRO;
            }

            if (guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_COMPROBANTE_PAGO)
            {
                guiaRetorna.IdGuia = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaExploradorGiros>().ValidarPago(guia.NumeroGuia.Value);
                guiaRetorna.IdServicio = TAConstantesServicios.SERVICIO_GIRO;
            }
            guiaRetorna.IdCiudad = guia.IdCiudad;
            guiaRetorna.Ciudad = guia.Ciudad;
            guiaRetorna.IdManifiesto = guia.IdManifiesto;
            guiaRetorna.ManifestadaOrigen = guia.ManifestadaOrigen;
            guiaRetorna.NumeroGuia = guia.NumeroGuia;
            guiaRetorna.TipoDescarga = guia.TipoDescarga;
            guiaRetorna.TipoManifiesto = guia.TipoManifiesto;
            guiaRetorna.UsuarioDescarga = guia.UsuarioDescarga;
            guiaRetorna.EstadoGuia = guia.EstadoGuia;
            guiaRetorna.FechaDescarga = guia.FechaDescarga;
            guiaRetorna.EstaDescargada = guia.EstaDescargada;
            guiaRetorna.EstadoGuia = guia.EstadoGuia;
            guiaRetorna.NuevoEstadoGuia = guia.NuevoEstadoGuia;
            return guiaRetorna;
        }

        private short ValidarEstado(string estadoGuiaManifiesto)
        {
            if (estadoGuiaManifiesto == LOIConstantesLogisticaInversa.ENTREGA_EXITOSA)
                return (short)(ADEnumEstadoGuia.Entregada);
            if (estadoGuiaManifiesto == LOIConstantesLogisticaInversa.DEVOLUCION)
                return (short)(ADEnumEstadoGuia.IntentoEntrega);
            if (estadoGuiaManifiesto == LOIConstantesLogisticaInversa.ENTREGA_MAL_DILIGENCIADA)
                return (short)(ADEnumEstadoGuia.CentroAcopio);
            else
                return 0;
        }

        #endregion Validar

        #endregion Manifiesto

        #region Descarga de Manifiesto

        #region Consultar

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a un Col destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<LIManifiestoDC> ObtenerManifiestosDestinoFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerManifiestosDestinoFiltro(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Metodo para obtener las guias por manifiesto
        /// </summary>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public IEnumerable<LIGuiaDC> ObtenerGuiasManifiestoDescarga(long idManifiesto)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerGuiasManifiestoDescarga(idManifiesto);
        }


        /// <summary>
        /// Método para validar si una guia ya tiene un recibido
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ValidarRecibidoGuia(long numeroGuia)
        {
            return LIRepositorioNotificaciones.Instancia.ValidarRecibidoGuia(numeroGuia);
        }

        #endregion Consultar

        #region Adicionar

        public OUEnumValidacionDescargue GuardarCambiosGuiaAgencia(LIGuiaDC guia)
        {
            ControllerException excepcion;
            ADGuia guiaAdmision = new ADGuia();
            if (guia == null)
            {
                excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA));
                throw new FaultException<ControllerException>(excepcion);
            }
            else
            {
                guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);

                guia.LocalidadDestino = new PALocalidadDC
                {
                    IdLocalidad = guiaAdmision.IdCiudadDestino,
                    Nombre = guiaAdmision.NombreCiudadDestino
                };



                OUPlanillaAsignacionMensajero planillamensajero = fachadaOpUrbana.ObtenerUltimaPlanillaMensajeroGuia((long)guia.NumeroGuia);
                if (planillamensajero != null && planillamensajero.EstadoEnPlanilla == "PLA")
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA, guia.NumeroGuia.ToString(), "La prueba de entrega que intenta descargar se encuentra asignada al mensajero " + planillamensajero.Mensajero.NombreCompleto + ", diríjase a la pantalla de descargue de mensajeros"));

                if (guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_PRUEBAS_ENTREGA || guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_FACTURA_MENSAJERIA)
                {

                    guia.IdServicio = guiaAdmision.IdServicio;
                    guia.IdGuia = guiaAdmision.IdAdmision;


                    if (guiaAdmision.IdServicio == (int)TAConstantesServicios.SERVICIO_NOTIFICACIONES && guia.NuevoEstadoGuia == LOIConstantesLogisticaInversa.ENTREGA_EXITOSA)
                    {
                        if (LIRepositorioNotificaciones.Instancia.ValidarRecibidoGuia(guia.NumeroGuia.Value))
                            if (guia.Notificacion == null)
                                return OUEnumValidacionDescargue.Notificacion;
                            else
                            {
                                guia.Notificacion.IdGuia = guiaAdmision.IdAdmision;
                                GuardarResultadoNotificacion(guia.Notificacion);
                            }
                        GuardarGuia(guia, guiaAdmision);
                        return OUEnumValidacionDescargue.Exitosa;
                    }
                    else if (guiaAdmision.IdServicio == (int)TAConstantesServicios.SERVICIO_RAPIRADICADO)
                        if (guia.RadicadoVerificado)
                            return GuardarGuia(guia, guiaAdmision);
                        else
                            return OUEnumValidacionDescargue.Rapiradicado;
                    else
                        return GuardarGuia(guia, guiaAdmision);
                }
                else if (guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_FACTURA_GIROS)
                {
                    guia.IdGuia = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaExploradorGiros>().ValidarGiro(guia.NumeroGuia.Value);
                    guia.IdServicio = TAConstantesServicios.SERVICIO_GIRO;
                    if (guia.IdGuia == 0)
                    {
                        string nombreManifiesto = LIRepositorioPruebasEntrega.Instancia.ConsultaNombreTipoManifiesto(guia.TipoManifiesto);
                        excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                         LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA.ToString(),
                         LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA));
                        excepcion.Mensaje = excepcion.Mensaje + nombreManifiesto;
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    return GuardarGuia(guia, guiaAdmision);
                }
                else if (guia.TipoManifiesto == (short)LOIEnumTiposManifiesto.TIPO_COMPROBANTE_PAGO)
                {
                    guia.IdGuia = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaExploradorGiros>().ValidarPago(guia.NumeroGuia.Value);
                    guia.IdServicio = TAConstantesServicios.SERVICIO_GIRO;
                    if (guia.IdGuia == 0)
                    {
                        string nombreManifiesto = LIRepositorioPruebasEntrega.Instancia.ConsultaNombreTipoManifiesto(guia.TipoManifiesto);
                        excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                         LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA.ToString(),
                         LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA));
                        excepcion.Mensaje = excepcion.Mensaje + nombreManifiesto;
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    return GuardarGuia(guia, guiaAdmision);
                }
                excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        public OUEnumValidacionDescargue GuardarEntregaGuiaAgencia(LIGuiaDC guia)
        {
            ///validar estados de la guia
            /// verificar si la egencia destino se apoya en el col            
            ///si no esta en centro de acopio destino ingresarla
            ///cambiar estado a entregada
            ///si es alcobro y no esta pagada afectar caja
            ///
            ControllerException excepcion;
            ADGuia guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);

            if (guiaAdmision == null)
            {
                excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(),
                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                throw new FaultException<ControllerException>(excepcion);
            }
            ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.IdGuia);
            ADTrazaGuia estadoGuia = new ADTrazaGuia
            {
                IdAdmision = guia.IdGuia,
                IdEstadoGuia = estadoActual.IdEstadoGuia,
                Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                NumeroGuia = guia.NumeroGuia,
                Observaciones = guia.Observaciones,
                Ciudad = guiaAdmision.NombreCiudadDestino,
                IdCiudad = guiaAdmision.IdCiudadDestino
            };
            if (estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.Admitida
                || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio
                || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoNacional
                || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoRegional
                || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoUrbano)
            {
                //traza para cambiar estado en admision
                if (estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio)
                {
                    PUCentroServiciosDC colCubrimiento = fachadaCentroServicios.ObtenerCOLResponsable(guiaAdmision.IdCentroServicioDestino);

                    if (colCubrimiento.IdCentroServicio != ControllerContext.Current.IdCentroServicio && guiaAdmision.IdServicio != (int)TAConstantesServicios.SERVICIO_INTERNACIONAL)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA, guia.NumeroGuia.ToString(), "La prueba de entrega que esta intentando descargar es de otra racol"));

                }
                else
                {
                    estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio;
                    estadoGuia.Observaciones = "guia ingresada de manera manual a bodega";
                }

                // valida que la guia este pagada si es al cobro
                if (guiaAdmision.EsAlCobro == true && guiaAdmision.EstaPagada == false)
                {
                    if (!fachadaMensajeria.AlCobroCargadoACoordinadorCol(guiaAdmision.IdAdmision))
                    {
                        fachadaMensajeria.ActualizarPagadoGuia(guiaAdmision.IdAdmision);
                        AfectarCajaAgenciaPorEntrega(guiaAdmision);
                    }
                }

                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                {
                    IdTrazaGuia = estadoGuia.IdTrazaGuia,
                    NumeroImpreso = guia.IdManifiesto,
                    TipoImpreso = ADEnumTipoImpreso.Manifiesto,
                    Usuario = ControllerContext.Current.Usuario,
                    FechaGrabacion = DateTime.Now,
                };
                EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);
            }
            else
            {
                excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
                throw new FaultException<ControllerException>(excepcion);
            }
            return 0;
        }

        /// <summary>
        /// Método para insertar guías en un manifiesto manual
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        private OUEnumValidacionDescargue GuardarGuia(LIGuiaDC guia, ADGuia guiaAdmision)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                //Valida si la guía fue adicionada o ya se encontraba en el manifiesto
                if (!guia.ManifestadaOrigen)
                    //si la guía existe en admisiones o giros la adiciona al manifiesto
                    LIRepositorioPruebasEntrega.Instancia.AdicionarGuiaManifiesto(guia);
                else
                    //Actualiza el estado a las guías que ya se encuentran en el manifiesto
                    LIRepositorioPruebasEntrega.Instancia.ActualizarEstadoGuiaManifiesto(guia);

                //actualiza intentos de entrega
                fachadaMensajeria.ActualizarReintentosEntrega(guiaAdmision.IdAdmision);

                if (guia.DescargueSupervisado || (string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && string.IsNullOrEmpty(guiaAdmision.Remitente.Telefono)))
                    fachadaMensajeria.ActualizarSupervisionGuia(guiaAdmision.IdAdmision);

                //Valida el ultimo cambio de estado
                if (guia.NuevoEstadoGuia == LOIConstantesLogisticaInversa.ENTREGA_EXITOSA)
                {
                    PUCentroServiciosDC colCubrimiento = fachadaCentroServicios.ObtenerCOLResponsable(guiaAdmision.IdCentroServicioDestino);

                    if (colCubrimiento.IdCentroServicio != ControllerContext.Current.IdCentroServicio && guiaAdmision.IdServicio != (int)TAConstantesServicios.SERVICIO_INTERNACIONAL)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA, guia.NumeroGuia.ToString(), "La prueba de entrega que esta intentando descargar es de otra racol"));


                    // valida que la guia este pagada si es al cobro
                    if (guiaAdmision.EsAlCobro == true && guiaAdmision.EstaPagada == false)
                    {
                        if (!fachadaMensajeria.AlCobroCargadoACoordinadorCol(guiaAdmision.IdAdmision))
                        {
                            fachadaMensajeria.ActualizarPagadoGuia(guiaAdmision.IdAdmision);
                            AfectarCajaAgenciaPorEntrega(guiaAdmision);
                        }
                    }
                    if (guiaAdmision.TipoCliente != ADEnumTipoCliente.INT)
                    {
                        //Adiciona la comision x entregar
                        GuardarComisionEntrega(guiaAdmision, CMEnumTipoComision.Entregar);
                    }
                    CambiarEstado(guia, guiaAdmision, 1);
                }
                else if (guia.NuevoEstadoGuia == LOIConstantesLogisticaInversa.DEVOLUCION)
                {
                    //cambia el estado de el intento de entrega
                    long idTraza = CambiarEstado(guia, guiaAdmision, guia.Motivo.IdMotivoGuia);
                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        IdTrazaGuia = idTraza,
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty
                    };
                    //inserta el estado motivo de la devolución
                    EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);
                    if (guia.EvidenciasDevolucion != null)
                    {
                        guia.EvidenciasDevolucion.ForEach(evidencia =>
                        {
                            evidencia.IdEstadoGuialog = idTraza;
                            evidencia.IdEvidenciaDevolucion = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarEvidenciaDevolucion(evidencia);
                            if (evidencia.Archivo != null && !string.IsNullOrEmpty(evidencia.Archivo.NombreServidor))
                            {
                                evidencia.Archivo.IdEvidenciaDevolucion = evidencia.IdEvidenciaDevolucion;
                                evidencia.Archivo.Fecha = DateTime.Now;
                                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarArchivo(evidencia.Archivo);
                            }
                        });
                    }
                }
                transaccion.Complete();
                return OUEnumValidacionDescargue.Exitosa;
            }
        }

        #endregion Adicionar

        #region Operaciones

        /// <summary>
        /// Método para el cambio de estado de la guía en admisiones descargue agencias
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public long CambiarEstado(LIGuiaDC guia, ADGuia Admision, int idMotivo)
        {
            ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(guia.IdGuia);

            ADTrazaGuia estadoGuia = new ADTrazaGuia
            {
                IdAdmision = guia.IdGuia,
                IdEstadoGuia = estadoActual.IdEstadoGuia,
                Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                NumeroGuia = guia.NumeroGuia,
                Observaciones = guia.Observaciones,
            };


            if (guia.NuevoEstadoGuia == LOIConstantesLogisticaInversa.ENTREGA_EXITOSA)
            {
                estadoGuia.Ciudad = Admision.NombreCiudadDestino;
                estadoGuia.IdCiudad = Admision.IdCiudadDestino;
                //evalua que la guia este en transito nacional, regional o urbano



                if (estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.Admitida || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoNacional || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoRegional || estadoActual.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoUrbano)
                {
                    //traza para cambiar estado en admision
                    if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio;
                        estadoGuia.Observaciones = "guia ingresada de manera manual a bodega";
                        EstadosGuia.InsertaEstadoGuia(estadoGuia);
                    }
                }
                else
                {
                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoActual.IdEstadoGuia)).ToString();
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                   LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CAMBIO_ESTADO.ToString(),
                   LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CAMBIO_ESTADO));
                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                    throw new FaultException<ControllerException>(excepcion);
                }

                //Cambia el estado de la guia a entregada, validando la transicion
                estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada;
                estadoGuia.Observaciones = string.Empty;

                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                //if (estadoGuia.IdTrazaGuia == 0)
                //{
                //    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                //    //no pudo realizar el cambio de estado
                //    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                //    LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                //    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
                //    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                //    throw new FaultException<ControllerException>(excepcion);
                //}
                // guarda la relacion entre el manifiesto y la trazaguia en  traza impreso
                ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                {
                    IdTrazaGuia = estadoGuia.IdTrazaGuia,
                    NumeroImpreso = guia.IdManifiesto,
                    TipoImpreso = ADEnumTipoImpreso.Manifiesto,
                    Usuario = ControllerContext.Current.Usuario,
                    FechaGrabacion = DateTime.Now,
                };
                EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);
            }



            else if (guia.NuevoEstadoGuia == LOIConstantesLogisticaInversa.DEVOLUCION)
            {

                bool validarEstado = true;

                if (idMotivo == 47 || idMotivo == 48)
                    validarEstado = false;


                if (validarEstado)
                {
                    if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoActual.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                       LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DIF_CENTROACOPIO.ToString(),
                       LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DIF_CENTROACOPIO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    else if (estadoActual.IdCentroServicioEstado != ControllerContext.Current.IdCentroServicio)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                          LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CENTROACOPIO_CESDIFERENTE.ToString(),
                          LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CENTROACOPIO_CESDIFERENTE));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    estadoGuia.Ciudad = Admision.NombreCiudadDestino;
                    estadoGuia.IdCiudad = Admision.IdCiudadDestino;
                    estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega;
                    estadoGuia.Observaciones = string.Empty;
                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                }


                //obtiene el proximo estado a partir del actual y el id del motivo
                estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega));

                //Inserta transicion de estado segun motivo
                PUCentroServiciosDC COL = fachadaCentroServicios.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                estadoGuia.Ciudad = COL.CiudadUbicacion.Nombre;
                estadoGuia.IdCiudad = COL.CiudadUbicacion.IdLocalidad;
                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                // guarda la reklacion entre el manifiesto y la trazaguia en  traza impreso
                ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                {
                    IdTrazaGuia = estadoGuia.IdTrazaGuia,
                    NumeroImpreso = guia.IdManifiesto,
                    TipoImpreso = ADEnumTipoImpreso.Manifiesto,
                    Usuario = ControllerContext.Current.Usuario,
                    FechaGrabacion = DateTime.Now,
                };

                EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

            }
            return estadoGuia.IdTrazaGuia.Value;
        }


        /// <summary>
        /// Método que guarda el resultado del descargue de una notificación
        /// </summary>
        /// <param name="notificacion"></param>
        private void GuardarResultadoNotificacion(LIRecibidoGuia notificacion)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                LINotificaciones.Instancia.RegistrarRecibidoGuiaManual(notificacion);
                transaccion.Complete();
            }
        }

        public void AfectarCajaAgenciaPorEntrega(ADGuia guia)
        {
            PUCentroServiciosDC agenciaDestino = fachadaCentroServicios.ObtenerAgenciaLocalidad(guia.IdCiudadDestino);

            CMComisionXVentaCalculadaDC comisionDestino = fachadaComisiones.CalcularComisionesxVentas(
            new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
            {
                IdCentroServicios = agenciaDestino.IdCentroServicio,// guia.IdCentroServicioDestino,
                IdServicio = guia.IdServicio,
                TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Entregar,
                ValorBaseComision = guia.ValorTotal,
                NumeroOperacion = guia.NumeroGuia
            });

            comisionDestino.EsRegistroValido = true;

            List<CARegistroVentaFormaPagoDC> formasDePago = new List<CARegistroVentaFormaPagoDC>();

            formasDePago.Add(new CARegistroVentaFormaPagoDC()
            {
                Valor = guia.ValorTotal,
                IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
                Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
                NumeroAsociado = ""
            });

            CARegistroTransacCajaDC registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = comisionDestino.IdCentroServicioResponsable,
                IdCentroServiciosVenta = comisionDestino.IdCentroServicioVenta,
                NombreCentroResponsable = comisionDestino.NombreCentroServicioResponsable,
                NombreCentroServiciosVenta = comisionDestino.NombreCentroServicioVenta,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
            {
              new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC() {
                 Cantidad = 1,
                 ConceptoCaja = new CAConceptoCajaDC() { IdConceptoCaja =(int) CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO },
                 ConceptoEsIngreso = true,
                 EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                 FechaFacturacion = DateTime.Now,
                 Numero = guia.NumeroGuia,
                 NumeroFactura = guia.NumeroGuia.ToString(),
                 Observacion = guia.Observaciones,
                 ValorDeclarado = guia.ValorDeclarado,
                 ValoresAdicionales = guia.ValorAdicionales,
                 ValorImpuestos = guia.ValorTotalImpuestos,
                 ValorPrimaSeguros = guia.ValorPrimaSeguro, ValorRetenciones = guia.ValorTotalRetenciones,
                 ValorServicio = guia.ValorServicio,
                 ValorTercero = 0
              }
            },
                ValorTotal = guia.ValorTotal,
                TotalImpuestos = guia.ValorTotalImpuestos,
                TotalRetenciones = guia.ValorTotalRetenciones,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = formasDePago
            };

            fachadaCajas.AdicionarMovimientoCaja(registro);
        }

        /// <summary>
        /// Método para afectar la cuenta de la agencia origen como entrega correcta
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="conceptoCaja"></param>
        /// <param name="esIngreso"></param>
        private void GuardarTransaccionAgencia(LIGuiaDC guiaLogistica, ADGuia guia)
        {
            fachadaCajas.AdicionarMovimientoCaja(new CARegistroTransacCajaDC
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = guiaLogistica.IdCaja,
                    IdCodigoUsuario = Framework.Servidor.Excepciones.ControllerContext.Current.CodigoUsuario,
                },
                IdCentroResponsable = 0,
                IdCentroServiciosVenta = guia.IdCentroServicioDestino,
                NombreCentroResponsable = string.Empty,
                NombreCentroServiciosVenta = guia.NombreCentroServicioDestino,

                ValorTotal = guia.ValorTotal,
                TotalImpuestos = 0,
                TotalRetenciones = 0,

                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>()
          {
             new CARegistroTransacCajaDetalleDC()
            {
              ConceptoCaja= new CAConceptoCajaDC()
              {
                IdConceptoCaja = (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO,
                EsIngreso=false
              },
              Cantidad=1,
              EstadoFacturacion=CAEnumEstadoFacturacion.FAC,
              FechaFacturacion=DateTime.Now,
              ValorServicio=guia.ValorServicio,
              ValorTercero=0,
              ValorImpuestos=0,
              ValorPrimaSeguros=0,
              ValorRetenciones=0,
              Numero=guia.NumeroGuia,
              NumeroFactura= guia.NumeroGuia.ToString(),
              ValorDeclarado=0,
              ValoresAdicionales=0,
              Observacion=guia.Observaciones,
              ConceptoEsIngreso=false
            }
          },
                Usuario = Framework.Servidor.Excepciones.ControllerContext.Current.Usuario,
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
          {
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago=TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
              Descripcion=TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
              Valor=guia.ValorTotal
            }
          }
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

        /// <summary>
        /// Método para guardar un manifiesto manual y la guia asociada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        public LIManifiestoDC GuardarManifiestoManual(LIGuiaDC guia, LIManifiestoDC manifiesto)
        {
            // if (ValidarGuiaInterna(manifiesto.NumeroGuiaInterna, (int)SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA, manifiesto.LocalidadOrigen.IdCentroServicio))
            //{
            using (TransactionScope transaccion = new TransactionScope())
            {
                manifiesto.IdManifiesto = AdicionarManifiesto(manifiesto);
                guia.IdManifiesto = manifiesto.IdManifiesto;
                GuardarCambiosGuiaAgencia(guia);
                transaccion.Complete();
            }
            //  }
            return manifiesto;
        }

        #endregion Operaciones

        #region Actualizar

        /// <summary>
        /// Método encargado de actualizar el inicio de la fecha de descarga de un manifiesto
        /// </summary>
        public void ActualizarManifiesto(long idManifiesto)
        {
            LIRepositorioPruebasEntrega.Instancia.ActualizarManifiesto(idManifiesto);
        }

        #endregion Actualizar

        #endregion Descarga de Manifiesto

        #region Descargue de planillas

        #region Consulta

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalregistros, long puntoServicio)
        {
            return fachadaOpUrbana.ObtenerMensajeroDescargueAgencia
              (filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalregistros, puntoServicio);
        }

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return fachadaOpUrbana.ObtenerGuiasMensajero(idMensajero);
        }


        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a una COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return fachadaOpUrbana.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Método para obtener las guías pendientes del col asignadas a logistica inversa
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasCol(long idCol)
        {
            IList<OUGuiaIngresadaDC> listaRetorna = new List<OUGuiaIngresadaDC>();
            List<CAAsignacionGuiaDC> enviosCol = fachadaCentroAcopio.ObtenerEnviosAsignadosporEstado(idCol, ADEnumEstadoGuia.CentroAcopio);
            if (enviosCol.Any())
            {
                enviosCol.ForEach(e =>
                    {
                        ADGuia admision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(e.NumeroGuia);
                        listaRetorna.Add
                            (
                            new OUGuiaIngresadaDC
                            {
                                IdAdmision = e.IdAdmision,
                                NumeroGuia = e.NumeroGuia,
                                DireccionDestinatario = e.DireccionDestino,
                                FechaAuditoria = admision.FechaEstimadaEntrega,
                                CantidadReintentosEntrega = (short)admision.CantidadIntentosEntrega,
                                FechaAsignacion = e.FechaAsignacion.Value,
                                TipoCliente = e.TipoCliente
                            }
                            );
                    });
            }
            return listaRetorna;
        }



        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero)
        {
            return fachadaOpUrbana.ObtenerUltimaPLanillaMensajero(idMensajero);
        }

        public OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo)
        {
            return fachadaOpUrbana.ObtenerConteoMotivosDevolucion(numeroGuia, nombreMensajero, idCol, NombreMotivo);
        }


        /// <summary>
        /// Obtiene los mensajeros que han estadpos asignados a un planilla de la cual se descargo una guia como no entregada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIMensajeroResponsableDC> ObtenerMensajerosResponsablesDescargue(long numeroGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerMensajerosResponsablesDescargue(numeroGuia);
        }

        #endregion Consulta

        #region Adición

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        public OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega)
        {
            return fachadaOpUrbana.GuardarCambiosGuia(guia, validaEntrega);
        }

        #endregion Adición

        #region Deshacer

        /// <summary>
        /// Método para deshacer la entrega exitosa de una prueba de entrega
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public void DeshacerEntrega(OUGuiaIngresadaDC guia)
        {
            fachadaOpUrbana.DeshacerEntrega(guia);
        }

        internal bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {            
            return fachadaLogisticaInversa.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        internal bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            return fachadaLogisticaInversa.ValidarRecepcionHistoricoEcapture(numeroGuia, codigoProceso);
        }

        #endregion Deshacer

        #endregion Descargue de planillas


        #region SISPOSTAL Masivos

        public bool ValidarGuiaDescargadaXAppMasivos(long NGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.ValidarGuiaDescargadaXAppMasivos(NGuia);
        }

        #endregion

        #region Ecapture

        public List<ArchivoGuia> VerificarGuia(string numeroGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.VerificarGuia(numeroGuia);
        }

        public void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            LIRepositorioPruebasEntrega.Instancia.ActualizarArchivoGuiaDigitalizada(archivoGuia);
        }

        public void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            LIRepositorioPruebasEntrega.Instancia.InsertaHistoricoArchivoGuiaDigitalizada(archivoGuia);
        }

        public List<ArchivoVolante> VerificarVolante(string numeroVolante)
        {
            return LIRepositorioPruebasEntrega.Instancia.VerificarVolante(numeroVolante);
        }

        public void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante)
        {
            LIRepositorioPruebasEntrega.Instancia.ActualizarArchivoVolanteSincronizado(archivoVolante);
        }

        public int ConsultarOrigenGuia(long numeroGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.ConsultarOrigenGuia(numeroGuia);
        }

        #endregion

    }
}