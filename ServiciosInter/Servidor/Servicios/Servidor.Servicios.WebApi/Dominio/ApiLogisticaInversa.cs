using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Raps;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiLogisticaInversa : ApiDominioBase
    {
        /// <summary>
        /// Singleton ApiLogisticaInversa 
        /// </summary>
        private static readonly ApiLogisticaInversa instancia = (ApiLogisticaInversa)FabricaInterceptorApi.GetProxy(new ApiLogisticaInversa(), COConstantesModulos.LOGISTICA_INVERSA);
        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        public static ApiLogisticaInversa Instancia
        {
            get { return ApiLogisticaInversa.instancia; }
        }

        /// <summary>
        /// Metodo para obtener los motivos segun el tipo motivo 
        /// </summary>
        /// <param name="tipoMotivo"></param>
        /// <returns></returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosGuia(ADEnumTipoMotivoDC tipoMotivo)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerMotivosGuias(tipoMotivo);
        }
        /// <summary>
        /// Descargue de aplicacion controller app
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueMensajeroControllerApp(DescargueControllerAppRequest descargue)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdMensajero = descargue.IdMensajero;
            liDescargueController.IdCiudad = descargue.IdCiudad;
            liDescargueController.NombreCiudad = descargue.NombreCiudad;
            liDescargueController.NombreQuienRecibe = descargue.NombreQuienRecibe;
            liDescargueController.Observaciones = descargue.Observaciones;
            liDescargueController.NumeroGuia = descargue.NumeroGuia;
            liDescargueController.IdentificacionQuienRecibe = descargue.IdentificacionQuienRecibe;
            liDescargueController.Latitud = descargue.Latitud;
            liDescargueController.Longitud = descargue.Longitud;
            liDescargueController.IdServicio = descargue.IdServicio;
            liDescargueController.FechaEntrega = descargue.FechaEntrega;
            liDescargueController.TipoEvidencia = descargue.TipoEvidencia;
            liDescargueController.IdPlanilla = descargue.IdPlanilla;

            if (descargue.IdServicio == (short)TAEnumServiciosDC.Notificaciones)
            {
                if (descargue.RecibidoGuia != null)
                {
                    liDescargueController.RecibidoGuia = new LIRecibidoGuia()
                    {
                        IdAplicacionOrigen = descargue.RecibidoGuia.IdAplicacionOrigen,
                        Telefono = descargue.RecibidoGuia.Telefono,
                        EstadoRegistro = descargue.RecibidoGuia.EstadoRegistro,
                        Verificado = false,
                        Identificacion = descargue.RecibidoGuia.Identificacion,
                        NumeroGuia = descargue.NumeroGuia,
                        Otros = descargue.RecibidoGuia.Otros,
                        RecibidoPor = descargue.RecibidoGuia.RecibidoPor
                    };
                }
                else
                {
                    LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "Recibido guía sin información";
                    return respuesta;
                }
            }

            return FabricaServicios.ServicioLogisticaInversa.DescargueMensajeroControllerApp(liDescargueController);
        }

        /// <summary>
        /// Metodo para devolucion mensajero controller app
        /// </summary>
        /// <param name="devolucion"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DevolucionMensajeroControllerApp(DescargueControllerAppRequest devolucion)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdCiudad = devolucion.IdCiudad;
            liDescargueController.IdMensajero = devolucion.IdMensajero;
            liDescargueController.Latitud = devolucion.Latitud;
            liDescargueController.Longitud = devolucion.Longitud;
            liDescargueController.MotivoGuia = new ADMotivoGuiaDC()
            {
                IdMotivoGuia = devolucion.MotivoGuia.IdMotivoGuia,
                TiempoAfectacion = devolucion.MotivoGuia.TiempoAfectacion,
                Descripcion = devolucion.MotivoGuia.Descripcion,
                IntentoEntrega = devolucion.MotivoGuia.IntentoEntrega,
                EsEscaneo = devolucion.MotivoGuia.EsEscaneo
            };
            liDescargueController.NombreCiudad = devolucion.NombreCiudad;
            liDescargueController.NombreQuienRecibe = devolucion.NombreQuienRecibe;
            liDescargueController.NumeroGuia = devolucion.NumeroGuia;
            liDescargueController.NumeroIntentoFallidoEntrega = devolucion.NumeroIntentoFallidoEntrega;
            liDescargueController.Observaciones = devolucion.Observaciones;
            liDescargueController.TipoEvidencia = devolucion.TipoEvidencia;
            liDescargueController.FechaGrabacion = DateTime.Now;
            liDescargueController.Latitud = devolucion.Latitud;
            liDescargueController.Longitud = devolucion.Longitud;
            liDescargueController.IdPlanilla = devolucion.IdPlanilla;
            liDescargueController.FechaEntrega = devolucion.FechaEntrega;

            return FabricaServicios.ServicioLogisticaInversa.DevolucionMensajeroControllerApp(liDescargueController);
        }

        /// <summary>
        /// Metodo para descargue auditor desde controller app
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        [System.Obsolete()]

        public LIEstadoDescargueControllerAppDC DescargueAuditorControllerApp(DescargueControllerAppRequest descargue)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdMensajero = descargue.IdMensajero;
            liDescargueController.IdCiudad = descargue.IdCiudad;
            liDescargueController.NombreCiudad = descargue.NombreCiudad;
            liDescargueController.NombreQuienRecibe = descargue.NombreQuienRecibe;
            liDescargueController.Observaciones = descargue.Observaciones;
            liDescargueController.NumeroGuia = descargue.NumeroGuia;
            liDescargueController.IdentificacionQuienRecibe = descargue.IdentificacionQuienRecibe;
            liDescargueController.Latitud = descargue.Latitud;
            liDescargueController.Longitud = descargue.Longitud;
            liDescargueController.IdServicio = descargue.IdServicio;
            liDescargueController.IdPlanilla = descargue.IdPlanilla;
            liDescargueController.FechaEntrega = descargue.FechaEntrega;
            liDescargueController.TipoEvidencia = descargue.TipoEvidencia;

            /******************* Para notificaciones, se valida recibibo guia ***********************/
            if (descargue.IdServicio == (short)TAEnumServiciosDC.Notificaciones)
            {
                if (descargue.RecibidoGuia != null)
                {
                    liDescargueController.RecibidoGuia = new LIRecibidoGuia()
                    {
                        IdAplicacionOrigen = descargue.RecibidoGuia.IdAplicacionOrigen,
                        Telefono = descargue.RecibidoGuia.Telefono,
                        EstadoRegistro = descargue.RecibidoGuia.EstadoRegistro,
                        Verificado = false,
                        Identificacion = descargue.RecibidoGuia.Identificacion,
                        NumeroGuia = descargue.NumeroGuia,
                        Otros = descargue.RecibidoGuia.Otros,
                        RecibidoPor = descargue.RecibidoGuia.RecibidoPor
                    };
                }
                else
                {
                    LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "Recibido guía sin información";
                    return respuesta;
                }
            }

            /********************************* Gestion rap para devoluciones falsas por mensajero ****************************************/
            if (descargue.TieneIntentoEntrega)
            {
                RASolicitudes.Instancia.CrearSolicitudAcumulativa(descargue.IdSistema, descargue.TipoNovedad, descargue.Parametros, descargue.IdCiudad);
            }

            return FabricaServicios.ServicioLogisticaInversa.DescargueAuditorControllerApp(liDescargueController);
        }

        /// <summary>
        /// Metodo para descargue auditor desde controller app
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueAuditorControllerAppV7(DescargueControllerAppRequest descargue)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdMensajero = descargue.IdMensajero;
            liDescargueController.IdCiudad = descargue.IdCiudad;
            liDescargueController.NombreCiudad = descargue.NombreCiudad;
            liDescargueController.NombreQuienRecibe = descargue.NombreQuienRecibe;
            liDescargueController.Observaciones = descargue.Observaciones;
            liDescargueController.NumeroGuia = descargue.NumeroGuia;
            liDescargueController.IdentificacionQuienRecibe = descargue.IdentificacionQuienRecibe;
            liDescargueController.Latitud = descargue.Latitud;
            liDescargueController.Longitud = descargue.Longitud;
            liDescargueController.IdServicio = descargue.IdServicio;
            liDescargueController.IdPlanilla = descargue.IdPlanilla;
            liDescargueController.FechaEntrega = descargue.FechaEntrega;
            liDescargueController.TipoEvidencia = descargue.TipoEvidencia;

            /******************* Para notificaciones, se valida recibibo guia ***********************/
            if (descargue.IdServicio == (short)TAEnumServiciosDC.Notificaciones)
            {
                if (descargue.RecibidoGuia != null)
                {
                    liDescargueController.RecibidoGuia = new LIRecibidoGuia()
                    {
                        IdAplicacionOrigen = descargue.RecibidoGuia.IdAplicacionOrigen,
                        Telefono = descargue.RecibidoGuia.Telefono,
                        EstadoRegistro = descargue.RecibidoGuia.EstadoRegistro,
                        Verificado = false,
                        Identificacion = descargue.RecibidoGuia.Identificacion,
                        NumeroGuia = descargue.NumeroGuia,
                        Otros = descargue.RecibidoGuia.Otros,
                        RecibidoPor = descargue.RecibidoGuia.RecibidoPor
                    };
                }
                else
                {
                    LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "Recibido guía sin información";
                    return respuesta;
                }
            }
            RegistroSolicitudAppDC registroSolicitud = new RegistroSolicitudAppDC
            {
                IdSistema = descargue.IdSistema,
                IdTipoNovedad = descargue.TipoNovedad,
                ValoresParametros = descargue.Parametros,
                IdCiudad = descargue.IdCiudad,
            };


            /********************************* Gestion rap para devoluciones falsas por mensajero ****************************************/
            if (descargue.TieneIntentoEntrega)
            {                
               RASolicitud.Instancia.CrearSolicitudAcumulativa(registroSolicitud);
            }

            return FabricaServicios.ServicioLogisticaInversa.DescargueAuditorControllerApp(liDescargueController);
        }

        /// <summary>
        /// Metodo para devolucion auditor
        /// </summary>
        /// <param name="devolucion"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DevolucionRatificadaAuditorControllerApp(DescargueControllerAppRequest devolucion)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdCiudad = devolucion.IdCiudad;
            liDescargueController.IdMensajero = devolucion.IdMensajero;
            liDescargueController.Latitud = devolucion.Latitud;
            liDescargueController.Longitud = devolucion.Longitud;
            liDescargueController.MotivoGuia = new ADMotivoGuiaDC()
            {
                IdMotivoGuia = devolucion.MotivoGuia.IdMotivoGuia,
                TiempoAfectacion = devolucion.MotivoGuia.TiempoAfectacion,
                Descripcion = devolucion.MotivoGuia.Descripcion,
                IntentoEntrega = devolucion.MotivoGuia.IntentoEntrega,
                EsEscaneo = devolucion.MotivoGuia.EsEscaneo
            };
            liDescargueController.NombreCiudad = devolucion.NombreCiudad;
            liDescargueController.NombreQuienRecibe = devolucion.NombreQuienRecibe;
            liDescargueController.NumeroGuia = devolucion.NumeroGuia;
            liDescargueController.NumeroIntentoFallidoEntrega = devolucion.NumeroIntentoFallidoEntrega;
            liDescargueController.Observaciones = devolucion.Observaciones;
            liDescargueController.TipoEvidencia = devolucion.TipoEvidencia;
            liDescargueController.FechaGrabacion = DateTime.Now;
            liDescargueController.IdPlanilla = devolucion.IdPlanilla;

            liDescargueController.TipoPredio = devolucion.TipoPredio;
            liDescargueController.DescripcionPredio = devolucion.DescripcionPredio;
            liDescargueController.TipoContador = devolucion.TipoContador;
            liDescargueController.NumeroContador = devolucion.NumeroContador;

            return FabricaServicios.ServicioLogisticaInversa.DevolucionRatificadaAuditorControllerApp(liDescargueController);
        }

        /// <summary>
        /// Metodo para descargue entrega maestra auditor controller app
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueEMAuditorControllerApp(DescargueControllerAppRequest descargue)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdCiudad = descargue.IdCiudad;
            liDescargueController.IdMensajero = descargue.IdMensajero;
            liDescargueController.Latitud = descargue.Latitud;
            liDescargueController.Longitud = descargue.Longitud;
            liDescargueController.MotivoGuia = new ADMotivoGuiaDC()
            {
                IdMotivoGuia = descargue.MotivoGuia.IdMotivoGuia,
                TiempoAfectacion = descargue.MotivoGuia.TiempoAfectacion,
                Descripcion = descargue.MotivoGuia.Descripcion,
                IntentoEntrega = descargue.MotivoGuia.IntentoEntrega,
                EsEscaneo = descargue.MotivoGuia.EsEscaneo
            };
            liDescargueController.NombreCiudad = descargue.NombreCiudad;
            liDescargueController.NombreQuienRecibe = descargue.NombreQuienRecibe;
            liDescargueController.NumeroGuia = descargue.NumeroGuia;
            liDescargueController.NumeroIntentoFallidoEntrega = descargue.NumeroIntentoFallidoEntrega;
            liDescargueController.Observaciones = descargue.Observaciones;
            liDescargueController.TipoEvidencia = descargue.TipoEvidencia;
            liDescargueController.IdServicio = descargue.IdServicio;
            liDescargueController.FechaEntrega = descargue.FechaEntrega;
            liDescargueController.FechaGrabacion = DateTime.Now;
            liDescargueController.IdPlanilla = descargue.IdPlanilla;

            /******************* Para notificaciones, se valida recibibo guia ***********************/
            if (descargue.IdServicio == (short)TAEnumServiciosDC.Notificaciones)
            {
                if (descargue.RecibidoGuia != null)
                {
                    liDescargueController.RecibidoGuia = new LIRecibidoGuia()
                    {
                        IdAplicacionOrigen = descargue.RecibidoGuia.IdAplicacionOrigen,
                        Telefono = descargue.RecibidoGuia.Telefono,
                        EstadoRegistro = descargue.RecibidoGuia.EstadoRegistro,
                        Verificado = false,
                        Identificacion = descargue.RecibidoGuia.Identificacion,
                        NumeroGuia = descargue.NumeroGuia,
                        Otros = descargue.RecibidoGuia.Otros,
                        RecibidoPor = descargue.RecibidoGuia.RecibidoPor
                    };
                }
                else
                {
                    LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "Recibido guía sin información";
                    return respuesta;
                }
            }


            return FabricaServicios.ServicioLogisticaInversa.DescargueEMAuditorControllerApp(liDescargueController);

        }

       
        /// <summary>
        /// Registro de una entrega desde la agencia
        /// </summary>
        /// <param name="guia"></param>
        public LIDescargueGuiaDC DescargueEntregaAgencia(OUGuiaIngresadaDC guia)
        {
           return FabricaServicios.ServicioLogisticaInversa.DescargueEntregaAgencia(guia);
        }

        /// <summary>
        /// Guarda Devoluvion Agencia
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public OUEnumValidacionDescargue GuardarDevolucionAgencia(GuiaIngresadaRequest guiaRequest)
        {
            OUGuiaIngresadaDC guia = new OUGuiaIngresadaDC
            {
                Ciudad = guiaRequest.Ciudad,
                IdCiudad = guiaRequest.IdCiudad,
                IdAdmision = guiaRequest.IdAdmision,
                NumeroGuia = guiaRequest.NumeroGuia,
                Observaciones = guiaRequest.Observaciones,
                FechaMotivoDevolucion = guiaRequest.FechaMotivoDevolucion,
                Planilla = guiaRequest.Planilla,
                TipoImpreso = guiaRequest.TipoImpreso,
                EvidenciaDevolucion = guiaRequest.EvidenciaDevolucion,
                Motivo = guiaRequest.Motivo,
                Novedad = guiaRequest.Novedad,
                CantidadReintentosEntrega = guiaRequest.CantidadReintentosEntrega,
            };

            var respuesta = FabricaServicios.ServicioLogisticaInversa.GuardarDevolucionAgencia(guia);
            return respuesta.Resultado;
        }

        public List<string> ObtenerImagenFachadaApp(long numeroGuia)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerImagenFachadaApp(numeroGuia);
        }

        /// <summary>
        /// Obtiene la imagen de la Fachada de entrega para una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        public List<LIArchivoGuiaMensajeriaFachadaDC> ObtenerImagenFachada(long numeroGuia)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerArchivoGuiaFachadaFS(numeroGuia);
        }

        #region Sispostal - Masivos

        /// <summary>
        /// Metodo para traer los motivos de devoluicion en Sispostal
        /// </summary>
        /// <returns></returns>
        /// <returns> lista de motivos de devolucion</returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosDevolucionGuiasMasivos()
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerMotivosDevolucionGuiasMasivos();
        }

        /// <summary>
        /// Descargue de aplicacion AppMasivos, para cualquier estado de la guia
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueMasivosControllerApp(DescargueControllerAppRequest descargue)
        {
            LIDescargueControllerAppDC liDescargueController = new LIDescargueControllerAppDC();
            liDescargueController.IdMensajero = descargue.IdMensajero;
            liDescargueController.IdCiudad = descargue.IdCiudad;
            liDescargueController.NombreCiudad = descargue.NombreCiudad;
            liDescargueController.NombreQuienRecibe = descargue.NombreQuienRecibe;
            liDescargueController.Observaciones = descargue.Observaciones;
            liDescargueController.NumeroGuia = descargue.NumeroGuia;
            liDescargueController.IdentificacionQuienRecibe = descargue.IdentificacionQuienRecibe;
            liDescargueController.Latitud = descargue.Latitud;
            liDescargueController.Longitud = descargue.Longitud;
            liDescargueController.IdServicio = descargue.IdServicio;
            liDescargueController.FechaEntrega = descargue.FechaEntrega;
            liDescargueController.TipoEvidencia = descargue.TipoEvidencia;
            liDescargueController.IdPlanilla = descargue.IdPlanilla;
            liDescargueController.IdEstado = descargue.IdEstado;
            liDescargueController.Usuario = descargue.Usuario;
            liDescargueController.MotivoGuia = new ADMotivoGuiaDC()
            {
                IdMotivoGuia = descargue.MotivoGuia.IdMotivoGuia,
                Descripcion = descargue.MotivoGuia.Descripcion,
            };
            if (descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Intento_Etrega)
            {
                liDescargueController.TipoContador = descargue.TipoContador;
                liDescargueController.NumeroContador = descargue.NumeroContador;
            }

            return FabricaServicios.ServicioLogisticaInversa.DescargueMasivosControllerApp(liDescargueController);
        }

        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return FabricaServicios.ServicioLogisticaInversa.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        #endregion

    }
}