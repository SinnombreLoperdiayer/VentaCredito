using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.LogisticaInversa.PruebasEntrega.Descargue;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.LogisticaInversa.Telemercadeo
{
    /// <summary>
    /// Manejador para las guía en telemercadeo
    /// </summary>
    internal class LIConfiguradorTelemercadeo : ControllerBase
    {
        private static readonly LIConfiguradorTelemercadeo instancia = (LIConfiguradorTelemercadeo)FabricaInterceptores.GetProxy(new LIConfiguradorTelemercadeo(), COConstantesModulos.TELEMERCADEO);

        public static LIConfiguradorTelemercadeo Instancia
        {
            get { return LIConfiguradorTelemercadeo.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        protected IADFachadaAdmisionesMensajeria FachadaMensajeria
        {
            get
            {
                if (fachadaMensajeria == null)
                {
                    fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
                }

                return fachadaMensajeria;
            }
        }


        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        private ICAFachadaCentroAcopio fachadaCentroAcopio = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCentroAcopio>();

        private IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();


        #region Consultas

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuias(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol)
        {
            if (esCol)
            {
                return LIRepositorioTelemercadeo.Instancia.ObtenerGestionesGuiasCol((short)estado, idCentroServicio, numeroGuia);
            }
            else
                return LIRepositorioTelemercadeo.Instancia.ObtenerGestionesGuiasAgencia((short)estado, idCentroServicio, numeroGuia);
        }

        /// <summary>
        /// Método para obtener los archivos de evidencia de una guia en telemercadeo
        /// </summary>
        /// <param name="IdEstadoGuiaLog"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ObtenerArchivosEvidenciaDevolucion(long IdEstadoGuiaLog)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerArchivosEvidenciaDevolucion(IdEstadoGuiaLog);
        }

        /// <summary>
        /// Método para obtener el detalle de una guia con el número de gestiones
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGuiaGestiones(long numeroGuia, short idEstado, string localidad)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerGuiaGestiones(numeroGuia, idEstado, localidad);
        }

        /// <summary>
        /// Método encargado de obtener gestiones de una guía
        /// </summary>
        /// <param name="idTrazaguia"></param>
        /// <returns></returns>
        public IList<LIGestionesDC> ObtenerGestionesGuia(long idTrazaguia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerGestionesGuia(idTrazaguia);
        }

        /// <summary>
        /// Obtener estados de la Guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIEstadoYMotivoGuiaDC ObtenerEstadoYMotivoGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerEstadoYMotivoGuia(numeroGuia);
        }
        /// <summary>
        /// Obtener flujo de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>lista de acciones de la guia</returns>
        public List<LIFlujoGuiaDC> ObtenerFlujoGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerFlujoGuia(numeroGuia);
        }
        /// <summary>
        /// Método para obtener los resultados posibles de una gestión de telemercadeo
        /// </summary>
        /// <returns></returns>
        public IList<LIResultadoTelemercadeoDC> ObtenerResultados()
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerResultados();
        }

        /// <summary>
        /// Método encargado de obtener la información de la guía en admisión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>guía de admision</returns>
        public ADGuia ObtenerInfoAdmision(long numeroGuia)
        {
            ADGuia guiaMensajeria = FachadaMensajeria.ObtenerGuiaTelemercadeo(numeroGuia);
            guiaMensajeria.EstadoGuia = (EstadosGuia.ObtenerUltimoEstado(guiaMensajeria.IdAdmision));
            return guiaMensajeria;
        }

        /// <summary>
        /// Método para obtener los posibles estados de transición
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public IList<ADEnumEstadoGuia> ObtenerEstados(ADEnumEstadoGuia estadoGuia)
        {
            return EstadosGuia.ObtenerPosiblesEstadosSiguientes(estadoGuia);
        }

        public List<ADEstadoGuia> ObtenerEstadosParaDevolver()
        {
            return EstadosGuia.ObtenerEstadosParaDevolver();
        }

        /// <summary>
        /// Método para obtener las razones de borrado de una gestión
        /// </summary>
        /// <returns></returns>
        public IList<LIGestionMotivoBorradoDC> ObtenerMotivosBorrado()
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerMotivosBorrado();
        }

        /// <summary>
        /// Retorna el stream de un archivo de envidencia de devolución dado su id
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoEvidenciaAdjunto(long idArchivo)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerArchivoEvidenciaAdjunto(idArchivo);
        }

        /// <summary>
        /// Consulta la informacion del flujo de la guia para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIAdmisionGuiaFlujoDC ObtenerAdmisionGuiaFlujo(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerAdmisionGuiaFlujo(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio nacional
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioNacionalDC> ObtenerIngresoAcopioNacional(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerIngresoAcopioNacional(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio urbano
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioUrbanoDC> ObtenerIngresoCentroAcopioUrbano(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerIngresoCentroAcopioUrbano(numeroGuia);
        }

        /// <summary>
        /// Consulta de asignaciones a mensajero para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIAsignacionMensajero> ObtenerAsignacionMensajero(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerAsignacionMensajero(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion del manifiestó para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIManifiestoMercadeoDC> ObtenerManifiesto(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerManifiesto(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion del archivo de la prueba de entrega
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoEntregaDC ObtenerArchivoPruebaEntrega(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerArchivoPruebaEntrega(numeroGuia);
        }

        /// <summary>
        /// Consulta la última gestión del telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIDetalleTelemercadeoDC ObtenerUltimaGestionTelemercadeoGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerUltimaGestionTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="esCol"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuiasTelemercadeo(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol)
        {
            if (esCol)
            {
                return LIRepositorioTelemercadeo.Instancia.ObtenerGestionesGuiasTelemercadeoCol((short)estado, idCentroServicio, numeroGuia);
            }
            else
            {
                return null;
                //return LIRepositorioTelemercadeo.Instancia.ObtenerGestionesGuiasAgencia((short)estado, idCentroServicio, numeroGuia);
            }
        }

        /// <summary>
        /// Consulta La gestion realizada por el usuario conectado el dia de hoy
        /// </summary>
        /// <returns>objeto estadistica telemercadeo</returns>
        public LIEstadisticaTelemercadeoDC ObtenerEstadisticaGestion()
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerEstadisticaGestion();
        }

        /// <summary>
        /// Obtener detalle de Telemercadeo al observar el flujo de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleTelemercadeoDC> ObtenerDetalleTelemercadeoGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerDetalleTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene el detalle de los motivos de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleMotivoGuiaDC> ObtenerDetalleMotivoGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerDetalleMotivoGuia(numeroGuia);
        }

        /// <summary>
        /// Consulta el historial de entregas de una direccion para una localidad
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<LIHistorialEntregaDC> ObtenerHistorialEntregas(string direccion, string idLocalidad)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerHistorialEntregas(direccion, idLocalidad);
        }

        /// <summary>
        /// Consulta las reclamaciondes de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIReclamacionesGuiaDC> ObtenerReclamacionesGuia(long numeroGuia)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerReclamacionesGuia(numeroGuia);
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar una gestión de telemercadeo
        /// </summary>
        /// <param name="?"></param>
        public int InsertarGestion(LIGestionesDC gestion)
        {
            if (gestion == null)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO,
                     LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL.ToString(),
                      LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL));
                throw new FaultException<ControllerException>(excepcion);
            }

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (gestion.AsignarASupervisor)
                    fachadaMensajeria.ActualizarSupervisionGuia(gestion.idAdmisionGuia);
                int id = LIRepositorioTelemercadeo.Instancia.InsertarGestion(gestion);
                ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(gestion.NumeroGuia);

                if (gestion.Resultado.IdResultado == 11)
                {
                    PUCentroServicioApoyo puntoRO = fachadaCentroServicio.ObtenerPuntosREOSegunUbicacionDestino(Convert.ToInt32(guia.IdCiudadDestino)).FirstOrDefault();
                    fachadaCentroAcopio.CambiarTipoEntregaTelemercadeo_REO(gestion.NumeroGuia, puntoRO.IdCentroservicio);

                }

                transaccion.Complete();
                return id;
            }
        }


        public ADEnumEstadoGuia InsertarGestionWpf(LIGestionesDC gestion)
        {
            if (gestion == null)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                    , LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL.ToString()
                    , LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL));
                throw new FaultException<ControllerException>(excepcion);
            }

            ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(gestion.NumeroGuia);
            ADEnumEstadoGuia estadoSalePara = ADEnumEstadoGuia.SinEstado;

            try
            {
                bool integrarRap = false;

                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (gestion.AsignarASupervisor)
                        fachadaMensajeria.ActualizarSupervisionGuia(gestion.idAdmisionGuia);

                    LIRepositorioTelemercadeo.Instancia.InsertarGestionDAO(gestion);

                    if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.NuevaDireccion) // NUEVA DIRECCIÓN
                    {
                        // REENVÍO
                        // Se Envía al COL Propietario de la Bodega de Confirmaciones y Devoluciones  
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Reenvio,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        //creacion de la tapa logistica
                        LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(new
                           LITapaLogisticaDC
                        {
                            NumeroGuia = gestion.NumeroGuia,
                            Tipo = LIEnumTipoTapaLogisticaDC.NuevaDireccion
                        });

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.Reenvio;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.Nohubocomunicaciondestinatario) //NO HUBO COMUNICACIÓN DESTINATARIO
                    {
                        //validacion para cuando regresa de auditoria clase admEstadoGuia
                        var nuevoEstadoGuia = DecideEstadoNoHuboComunicacion(guia);

                        //AUDITORIA VALOR POR DEFECTO
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)nuevoEstadoGuia,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };


                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "La guía ya estuvo en Auditoria, no es posible enviarla nuevamente, seleccione otra opción");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = nuevoEstadoGuia;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.Nohubocomunicacionremitente) //NO HUBO COMUNICACIÓN REMITENTE
                    {
                        //validacion para cuando regresa de auditoria clase admEstadoGuia
                        var nuevoEstadoGuia = DecideEstadoNoHuboComunicacion(guia);

                        //AUDITORIA VALOR POR DEFECTO
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)nuevoEstadoGuia,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "La guía ya estuvo en Auditoria, no es posible enviarla nuevamente, seleccione otra opción");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = nuevoEstadoGuia;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.Solicituddedevolucion) //SOLICITUD DE DEVOLUCIÓN
                    {
                        // DEVOLUCION RATIFICADA
                        if (guia.EsAlCobro && !guia.EstaPagada)
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                , LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL.ToString()
                                , "No se puede Devolver por que el alcobro no se encuentra cancelado!");
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.DevolucionRatificada;
                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.ClienteNoAceptaDevolucionDelAlcobro) //REMITENTE NO ACEPTA DEVOLUCIÓN DEL ALCOBRO
                    {
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.PendienteIngresoaCustodia,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        //creacion de la tapa logistica
                        LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(new LITapaLogisticaDC
                        {
                            NumeroGuia = gestion.NumeroGuia,
                            Tipo = LIEnumTipoTapaLogisticaDC.Custodia
                        });

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.Custodia;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.RemitenteSolicitaUltimoReenvio) //REMITENTE SOLICITA ULTIMO REENVÍO
                    {
                        // REENVÍO

                        // Se Envía al COL Propietario de la Bodega de Confirmaciones y Devoluciones
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Reenvio,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.Reenvio;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.FalsoMotivoDevolución) //FALSO MOTIVO DEVOLUCIÓN
                    {
                        // AUDITORIA
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Auditoria,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);


                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.Auditoria;
                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.ReintentarLlamada) //REINTENTAR LLAMADA
                    {
                        // TELEMERCADEO
                        // La Guia Continua en Telemercadeo

                        estadoSalePara = ADEnumEstadoGuia.Telemercadeo;
                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.DevolucionRatificada) //DEVOLUCION RATIFICADA
                    {
                        // DEVOLUCION RATIFICADA
                        if (guia.EsAlCobro && !guia.EstaPagada)
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                , LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL.ToString()
                                , "No se puede Devolver por que el alcobro no se encuentra cancelado!");
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }


                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.DevolucionRatificada;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.EnvioParaCustodia) //ENVÍO PARA CUSTODIA
                    {
                        //CUSTODIA
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.PendienteIngresoaCustodia,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        //creacion de la tapa logistica
                        LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(new LITapaLogisticaDC
                        {
                            NumeroGuia = gestion.NumeroGuia,
                            Tipo = LIEnumTipoTapaLogisticaDC.Custodia
                        });

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.Custodia;

                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.ReclamoEnOficina) //Cambio Reclame en Oficina
                    {
                        // Cambio Reclame en Oficina
                        fachadaCentroAcopio.CambiarTipoEntregaTelemercadeo_REO(gestion.NumeroGuia, gestion.IdCentroServicio);

                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(gestion.idAdmisionGuia)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Reenvio,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                            , "0"
                                            , "Cambio de Estado no válido");
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        // Aunque el Estado de la Guia cambia a Reenvio... enviamos mensaje a Cliente de Sale para Reclame on Oficina

                        //creacion de la tapa logistica
                        LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(new LITapaLogisticaDC
                        {
                            NumeroGuia = gestion.NumeroGuia,
                            Tipo = LIEnumTipoTapaLogisticaDC.ReclameOficina
                        });

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });

                        estadoSalePara = ADEnumEstadoGuia.ReclameEnOficina;
                    }
                    else if (gestion.TipoGestion == LIEnumTipoGestionTelemercadeo.EnEsperaConfirmacionCliente) //En espera confirmacion cliente
                    {
                        PUCentroServiciosDC CenSerActual = fachadaCentroServicio.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = CenSerActual.CiudadUbicacion.Nombre,
                            IdCiudad = CenSerActual.CiudadUbicacion.IdLocalidad,
                            IdAdmision = gestion.idAdmisionGuia,
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionEsperaConfirmacionCliente,
                            Modulo = COConstantesModulos.TELEMERCADEO,
                            NumeroGuia = gestion.NumeroGuia,
                            Observaciones = string.Empty,
                            FechaGrabacion = DateTime.Now
                        };
                        EstadosGuia.InsertaEstadoGuia(estadoGuia);

                        // Cambiar la Fecha estimada Entrega (24horas).
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                            TiempoAfectacion = (24)
                        });
                        estadoSalePara = ADEnumEstadoGuia.DevolucionEsperaConfirmacionCliente;
                    }

                    if (estadoSalePara == ADEnumEstadoGuia.Custodia)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guiaAdmision.TipoCliente == ADEnumTipoCliente.PPE)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia, "");
                        }

                    }

                    integrarRap = true;
                    transaccion.Complete();                   
                }
                if(integrarRap)
                    IntegrarRapsFalsoMotivoDevolucion(gestion, guia);

                return estadoSalePara;
            }
            catch (FaultException<ControllerException> errorControlado)
            {
                throw errorControlado;
            }
            catch (Exception err)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                     , LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION.ToString()
                     , err.Message);
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Crea el rap cuando hay un falso motivo de devolucion
        /// </summary>
        /// <param name="gestion"></param>
        /// <param name="guia"></param>
        private void IntegrarRapsFalsoMotivoDevolucion(LIGestionesDC gestion, ADGuia guia)
        {
            PUAgenciaDeRacolDC colResponsable = fachadaCentroServicio.ObteneColPropietarioBodega(ControllerContext.Current.IdCentroServicio);

            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            Comun.EnumTipoNovedadRaps motivoRaps = Comun.EnumTipoNovedadRaps.Pordefecto;
            switch ((Comun.EnumMotivoGuiaRaps)gestion.TipoGestion)
            {
                case Comun.EnumMotivoGuiaRaps.FALSO_MOTIVO_DEVOLUCION:
                    lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("FalsoMotivoDev");
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), gestion.FechaGestion);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), gestion.NombreMensajero);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), gestion.Idmensajero);
                    parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), colResponsable.IdResponsable);
                    motivoRaps = Comun.EnumTipoNovedadRaps.MotivoDevolucionFalsa;
                    break;
                default:
                    break;
            }

            if (motivoRaps != Comun.EnumTipoNovedadRaps.Pordefecto)
                RAIntegracionRaps.Instancia.CrearSolicitudAcumulativaRaps((Raps.Comun.Integraciones.EnumTipoNovedadRaps)motivoRaps.GetHashCode(), parametrosParametrizacion, colResponsable.IdCiudadResponsable.Substring(0, 5), ControllerContext.Current.Usuario);
        }

        /// <summary>
        /// Crea el rap cuando hay un falso motivo de devolucion
        /// </summary>
        /// <param name="gestion"></param>
        /// <param name="guia"></param>
        private void IntegrarRapsFalsoMotivoDevolucionV7(LIGestionesDC gestion, ADGuia guia)
        {
            PUAgenciaDeRacolDC colResponsable = fachadaCentroServicio.ObteneColPropietarioBodega(ControllerContext.Current.IdCentroServicio);

            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            CoEnumTipoNovedadRaps motivoRaps = CoEnumTipoNovedadRaps.Pordefecto;
            //switch ((Raps.Comun.Integraciones.EnumMotivoGuiaRaps)gestion.TipoGestion)
            //{
            //    case Raps.Comun.Integraciones.EnumMotivoGuiaRaps.FALSO_MOTIVO_DEVOLUCION:
            //        // lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion("FalsoMotivoDev");
            //       // lstParametros = RAIntegracionRaps.Instancia.ObtenerParametrosPorIntegracion(Raps.Comun.Integraciones.EnumMotivoGuiaRaps.FALSO_MOTIVO_DEVOLUCION.GetHashCode());
            //        parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NumeroGuia").FirstOrDefault().IdParametro.ToString(), guia.NumeroGuia);
            //        parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "FechaDescarga").FirstOrDefault().IdParametro.ToString(), gestion.FechaGestion);
            //        parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "NombreCompleto").FirstOrDefault().IdParametro.ToString(), gestion.NombreMensajero);
            //        parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdentificacionMensajero").FirstOrDefault().IdParametro.ToString(), gestion.Idmensajero);
            //        parametrosParametrizacion.Add(lstParametros.Where(p => p.NombreParametro == "IdCol").FirstOrDefault().IdParametro.ToString(), colResponsable.IdResponsable);
            //        motivoRaps = CoEnumTipoNovedadRaps.MotivoDevolucionFalsa;
            //        break;
            //    default:
            //        break;
            //}

            if (motivoRaps != CoEnumTipoNovedadRaps.Pordefecto)
                RAIntegracionesRaps.Instancia.CrearSolicitudAcumulativaRaps(motivoRaps, parametrosParametrizacion, colResponsable.IdCiudadResponsable.Substring(0, 5),ControllerContext.Current.Usuario);
        }

        public ADEnumEstadoGuia InsertarGestionAgenciaWpf(LIGestionesDC gestion)
        {
            if (gestion == null)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                    , LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL.ToString()
                    , LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL));
                throw new FaultException<ControllerException>(excepcion);
            }

            ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(gestion.NumeroGuia);
            ADEnumEstadoGuia estadoSalePara = ADEnumEstadoGuia.SinEstado;

            try
            {

                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (gestion.AsignarASupervisor)
                    {
                        fachadaMensajeria.ActualizarSupervisionGuia(gestion.idAdmisionGuia);
                    }

                    var tipoGestionGeneracion = gestion.TipoGestion;

                    this.ValidarEstadoGestionAgencia(ref gestion);

                    LIRepositorioTelemercadeo.Instancia.InsertarGestionDAO(gestion);

                    if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.NuevaDireccion) // NUEVA DIRECCIÓN
                    {
                        // REENVÍO
                        // Se Envía al COL Propietario de la Bodega de Confirmaciones y Devoluciones  
                        estadoSalePara = ADEnumEstadoGuia.Reenvio;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.Reenvio);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.Nohubocomunicaciondestinatario) //NO HUBO COMUNICACIÓN DESTINATARIO
                    {
                        //validacion para cuando regresa de auditoria clase admEstadoGuia                        

                        //AUDITORIA VALOR POR DEFECTO
                        estadoSalePara = ADEnumEstadoGuia.DevolucionRegional;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.DevolucionRegional);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.Nohubocomunicacionremitente) //NO HUBO COMUNICACIÓN REMITENTE
                    {
                        //validacion para cuando regresa de auditoria clase admEstadoGuia

                        //AUDITORIA VALOR POR DEFECTO
                        estadoSalePara = ADEnumEstadoGuia.DevolucionRegional;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.DevolucionRegional);

                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.Solicituddedevolucion) //SOLICITUD DE DEVOLUCIÓN
                    {
                        // DEVOLUCION RATIFICADA
                        if (guia.EsAlCobro && !guia.EstaPagada)
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                                , LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL.ToString()
                                , "No se puede Devolver por que el alcobro no se encuentra cancelado!");
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        estadoSalePara = ADEnumEstadoGuia.DevolucionRegional;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.DevolucionRegional);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.ClienteNoAceptaDevolucionDelAlcobro) //REMITENTE NO ACEPTA DEVOLUCIÓN DEL ALCOBRO
                    {
                        estadoSalePara = ADEnumEstadoGuia.DevolucionRegional;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.DevolucionRegional);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.RemitenteSolicitaUltimoReenvio) //REMITENTE SOLICITA ULTIMO REENVÍO
                    {
                        // REENVÍO
                        estadoSalePara = ADEnumEstadoGuia.Reenvio;
                        this.ObtencionEstadoGestionGuia(gestion, guia, estadoSalePara);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.FalsoMotivoDevolución) //FALSO MOTIVO DEVOLUCIÓN
                    {
                        // AUDITORIA
                        estadoSalePara = ADEnumEstadoGuia.Auditoria;
                        this.ObtencionEstadoGestionGuia(gestion, guia, estadoSalePara);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.ReintentarLlamada) //REINTENTAR LLAMADA
                    {
                        // TELEMERCADEO
                        // La Guia Continua en Telemercadeo

                        estadoSalePara = ADEnumEstadoGuia.Telemercadeo;
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.DevolucionRatificada) //DEVOLUCION RATIFICADA
                    {
                        // DEVOLUCION RATIFICADA
                        estadoSalePara = ADEnumEstadoGuia.DevolucionRegional;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.DevolucionRegional);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.EnvioParaCustodia) //ENVÍO PARA CUSTODIA
                    {
                        //CUSTODIA
                        estadoSalePara = ADEnumEstadoGuia.Custodia;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.PendienteIngresoaCustodia);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.ReclamoEnOficina) //Cambio Reclame en Oficina
                    {
                        // Cambio Reclame en Oficina
                        estadoSalePara = ADEnumEstadoGuia.ReclameEnOficina;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.Reenvio);
                    }
                    else if (tipoGestionGeneracion == LIEnumTipoGestionTelemercadeo.EnEsperaConfirmacionCliente) //En espera confirmacion cliente
                    {
                        estadoSalePara = ADEnumEstadoGuia.DevolucionEsperaConfirmacionCliente;
                        this.ObtencionEstadoGestionGuia(gestion, guia, ADEnumEstadoGuia.DevolucionEsperaConfirmacionCliente);
                    }

                    transaccion.Complete();
                    return estadoSalePara;
                }
            }
            catch (FaultException<ControllerException> errorControlado)
            {
                throw errorControlado;
            }
            catch (Exception err)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO
                     , LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION.ToString()
                     , err.Message);
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        private void ValidarEstadoGestionAgencia(ref LIGestionesDC gestion)
        {

            switch (gestion.TipoGestion)
            {
                case LIEnumTipoGestionTelemercadeo.Solicituddedevolucion:
                case LIEnumTipoGestionTelemercadeo.Nohubocomunicaciondestinatario:
                case LIEnumTipoGestionTelemercadeo.Nohubocomunicacionremitente:
                case LIEnumTipoGestionTelemercadeo.ClienteNoAceptaDevolucionDelAlcobro:
                case LIEnumTipoGestionTelemercadeo.DevolucionRatificada:
                    gestion.TipoGestion = LIEnumTipoGestionTelemercadeo.DevolucionRegional;
                    break;
                default:
                    break;
            }
        }

        private void ObtencionEstadoGestionGuia(LIGestionesDC gestion, ADGuia guia, ADEnumEstadoGuia nuevoEstadoGuia)
        {
            ADTrazaGuia estadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia);
            estadoGuia.IdNuevoEstadoGuia = (short)nuevoEstadoGuia;
            estadoGuia.Modulo = COConstantesModulos.TELEMERCADEO;
            estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuiaCentroServicio(estadoGuia);
            fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);

            // Cambiar la Fecha estimada Entrega (24horas).
            EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
            {
                Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                TiempoAfectacion = (24)
            });
        }

        private ADEnumEstadoGuia DecideEstadoNoHuboComunicacion(ADGuia guia)
        {
            // Consulta AdmEstado guia para validacion
            List<ADTrazaGuia> admEstadosGuia = EstadosGuia.ObtenerEstadosGuia(guia.NumeroGuia);
            var estado = admEstadosGuia.Where(s => s.IdEstadoGuia.Equals((short)ADEnumEstadoGuia.Auditoria));

            if (estado.Count() > 0)
            {
                if ((guia.FormasPago.FirstOrDefault().IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO
                    && guia.Peso <= 2) || guia.FormasPago.FirstOrDefault().IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CREDITO)
                {
                    return ADEnumEstadoGuia.DevolucionRatificada;
                }
                else
                {
                    return ADEnumEstadoGuia.Custodia;
                }
            }
            else
            {
                return ADEnumEstadoGuia.Auditoria;
            }
        }



        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="TrazaGuia"></param>
        /// <returns></returns>
        public long CambiarEstadoGuia(ADTrazaGuia trazaGuia, LIGestionesDC gestion, ADMotivoGuiaDC motivo)
        {
            long idTraza = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                // Se inhabilitan reglas de telemercadeo Walter 11/10/2012
                //if (!string.IsNullOrEmpty(Gestion.Resultado.nombreClase))
                //{
                //  IDictionary<string, object> parametrosRegla = new Dictionary<string, object>();

                //  parametrosRegla.Add(LOIConstantesLogisticaInversa.ESTADO_GUIA, TrazaGuia);
                //  parametrosRegla.Add(LOIConstantesLogisticaInversa.RESULTADO_TELEMERCADEO, Gestion);

                //RespuestaEjecutorReglas resultado = Ejecutor.EjecutarRegla(Gestion.Resultado.nombreAssembly, Gestion.Resultado.@namespace, Gestion.Resultado.nombreClase, parametrosRegla);
                //  if (resultado.HuboError)
                //  {
                //    if (resultado.ParametrosRegla.ContainsKey(ClavesReglasFramework.EXCEPCION))
                //      throw new FaultException<ControllerException>((ControllerException)resultado.ParametrosRegla[ClavesReglasFramework.EXCEPCION]);
                //  }
                //  else
                //  {
                //    idTraza = EstadosGuia.InsertaEstadoGuia(TrazaGuia);
                //  }
                //}
                //else
                //{
                //  idTraza = EstadosGuia.InsertaEstadoGuia(TrazaGuia);
                //}

                ADGuia guiaMensajeria = FachadaMensajeria.ObtenerGuiaXNumeroGuia(trazaGuia.NumeroGuia.Value);
                if (trazaGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.Custodia)
                {

                    if (guiaMensajeria.NotificarEntregaPorEmail && !string.IsNullOrEmpty(guiaMensajeria.Remitente.Email))
                    {
                        InformacionAlerta informacionAlerta = PAParametros.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_TELEMERCADEO_ENVIO_REMITENTE);
                        PAEnvioCorreoAsyn.Instancia.EnviarCorreoAsyn(guiaMensajeria.Remitente.Email, informacionAlerta.Asunto, informacionAlerta.Mensaje);
                    }
                }

                if (guiaMensajeria.EsAlCobro && guiaMensajeria.EstaPagada)
                {
                    ICAFachadaCajas fachadaCaja = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                    //SERGIO:CONSULTAR VALOR CARGADO A AGENCIAS X AL COBRO PAGADO 15/05/2014
                    decimal valorCargadoCaja = 0;
                    PUCentroServiciosDC CS = fachadaCaja.ConsultarCentroDeServiciosPagoAlCobro(guiaMensajeria.NumeroGuia, out valorCargadoCaja);

                    if (valorCargadoCaja > 0 && string.IsNullOrEmpty(CS.NoComprobante))
                    {
                        //SERGIO:APLICAR EGRESO A LA AGENCIA EN LA CUAL FUE CARGADO EL AL COBRO  15/05/2014
                        CAConceptoCajaDC conceptocaja = new CAConceptoCajaDC()
                        {
                            IdConceptoCaja = (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO,
                            Descripcion = "Descuento x al cobro devuelto",
                            EsIngreso = false,
                            EsEgreso = true,
                            ContraPartidaCasaMatriz = true,
                            ContraPartidaCS = false
                        };
                        ADEnumEstadoGuia estadonuevo = (ADEnumEstadoGuia)trazaGuia.IdNuevoEstadoGuia;
                        string observaciones = "Descuento por " + estadonuevo.ToString() + " de al cobro No.:" + guiaMensajeria.NumeroGuia.ToString();
                        CARegistroTransacCajaDC transAgencia = ArmarTransaccionCajaXDevRatificada(CS, conceptocaja, valorCargadoCaja, observaciones);
                        transAgencia.RegistrosTransacDetallesCaja.FirstOrDefault().Numero = guiaMensajeria.NumeroGuia;
                        fachadaCaja.AdicionarMovimientoCaja(transAgencia);

                        FachadaMensajeria.ActualizarPagadoGuia(guiaMensajeria.IdAdmision, false);
                    }
                }


                idTraza = EstadosGuia.InsertaEstadoGuia(trazaGuia);

                //Inserta el motivo del cambio de estado a devolución
                if (trazaGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada)
                {
                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        IdTrazaGuia = idTraza,
                        Motivo = motivo,
                        Observaciones = string.Empty,
                        FechaMotivo = trazaGuia.FechaGrabacion
                    };
                    EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);
                }

                transaccion.Complete();
            }
            return idTraza;
        }

        private CARegistroTransacCajaDC ArmarTransaccionCajaXDevRatificada(PUCentroServiciosDC cs, CAConceptoCajaDC conceptoCaja, decimal valor, string observaciones)
        {
            CARegistroTransacCajaDC transaccion = new CARegistroTransacCajaDC()
            {
                EsTransladoEntreCajas = false,
                EsUsuarioGestion = false,
                IdCentroServiciosVenta = cs.IdCentroServicio,
                NombreCentroServiciosVenta = cs.Nombre,
                IdCentroResponsable = cs.IdCentroServicio,
                NombreCentroResponsable = cs.Nombre,
                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>(),
                Usuario = ControllerContext.Current.Usuario,
                ValorTotal = valor,
            };

            CARegistroTransacCajaDetalleDC transDetelle = new CARegistroTransacCajaDetalleDC()
            {
                ConceptoCaja = conceptoCaja,
                ConceptoEsIngreso = conceptoCaja.EsIngreso,
                Observacion = observaciones,
                Descripcion = observaciones,
                ValorServicio = valor,
                Numero = 0,
                NumeroFactura = "0",
                EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                NumeroComprobante = "0",
                FechaFacturacion = DateTime.Now
            };

            transaccion.InfoAperturaCaja = new CAAperturaCajaDC()
            {
                IdCaja = 0,
                IdCodigoUsuario = ControllerContext.Current.CodigoUsuario
            };

            transaccion.RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>();

            CARegistroVentaFormaPagoDC formaPago = new CARegistroVentaFormaPagoDC()
            {
                Valor = valor,
                IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
                Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
                NumeroAsociado = "0"
            };

            transaccion.RegistroVentaFormaPago.Add(formaPago);
            transaccion.TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA;
            transaccion.RegistrosTransacDetallesCaja.Add(transDetelle);

            return transaccion;
        }

        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="TrazaGuia"></param>
        /// <returns></returns>
        public long CambiarEstadoGuia(ADTrazaGuia TrazaGuia)
        {
            long idTraza = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (TrazaGuia.IdNuevoEstadoGuia == (int)ADEnumEstadoGuia.Custodia)
                {
                    ADGuia guiaMensajeria = FachadaMensajeria.ObtenerGuiaXNumeroGuia(TrazaGuia.NumeroGuia.Value);
                    if (guiaMensajeria.EsAlCobro && guiaMensajeria.EstaPagada)
                    {
                        ICAFachadaCajas fachadaCaja = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

                        //SERGIO:CONSULTAR VALOR CARGADO A AGENCIAS X AL COBRO PAGADO 15/05/2014
                        decimal valorCargadoCaja = 0;
                        PUCentroServiciosDC CS = fachadaCaja.ConsultarCentroDeServiciosPagoAlCobro(guiaMensajeria.NumeroGuia, out valorCargadoCaja);

                        if (valorCargadoCaja > 0 && string.IsNullOrEmpty(CS.NoComprobante))
                        {
                            //SERGIO:APLICAR EGRESO A LA AGENCIA EN LA CUAL FUE CARGADO EL AL COBRO  15/05/2014
                            CAConceptoCajaDC conceptocaja = new CAConceptoCajaDC()
                            {
                                IdConceptoCaja = (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO,
                                Descripcion = "Descuento x al cobro devuelto",
                                EsIngreso = false,
                                EsEgreso = true,
                                ContraPartidaCasaMatriz = true,
                                ContraPartidaCS = false
                            };
                            ADEnumEstadoGuia estadonuevo = (ADEnumEstadoGuia)TrazaGuia.IdNuevoEstadoGuia;
                            string observaciones = "Descuento por " + estadonuevo.ToString() + " de al cobro No.:" + guiaMensajeria.NumeroGuia.ToString();
                            CARegistroTransacCajaDC transAgencia = ArmarTransaccionCajaXDevRatificada(CS, conceptocaja, valorCargadoCaja, observaciones);
                            transAgencia.RegistrosTransacDetallesCaja.FirstOrDefault().Numero = guiaMensajeria.NumeroGuia;
                            fachadaCaja.AdicionarMovimientoCaja(transAgencia);

                            FachadaMensajeria.ActualizarPagadoGuia(guiaMensajeria.IdAdmision, false);
                        }
                        //FachadaMensajeria.ActualizarPagadoGuia(guiaMensajeria.IdAdmision, false);
                    }
                }

                idTraza = EstadosGuia.ValidarInsertarEstadoGuia(TrazaGuia);
                if (idTraza == 0)
                {
                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(TrazaGuia.IdEstadoGuia)).ToString();

                    //no pudo realizar el cambio de estado
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                    throw new FaultException<ControllerException>(excepcion);

                }
                transaccion.Complete();

            }
            return idTraza;
        }



        #endregion Inserciones

        #region Eliminar

        /// <summary>
        /// Método para guardar
        /// </summary>

        /// <param name="idGestion"></param>
        public void EliminarGestion(LIGestionesDC Gestion)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {

                LIRepositorioTelemercadeo.Instancia.EliminarGestion(Gestion);
                transaccion.Complete();
            }
        }

        #endregion Eliminar

    }
}