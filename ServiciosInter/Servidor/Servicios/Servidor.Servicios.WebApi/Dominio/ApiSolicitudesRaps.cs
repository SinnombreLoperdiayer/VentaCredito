using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.MotorReglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
//using CO.Servidor.Raps.ReglasFallasRaps;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Raps;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiSolicitudesRaps : ApiDominioBase
    {
        private static readonly ApiSolicitudesRaps instancia = (ApiSolicitudesRaps)FabricaInterceptorApi.GetProxy(new ApiSolicitudesRaps(), COConstantesModulos.MODULO_RAPS);

        #region constructor
        public static ApiSolicitudesRaps Instancia
        {
            get { return ApiSolicitudesRaps.instancia; }
        }

        private ApiSolicitudesRaps() { }
        #endregion

        #region Adjunto

        /// <summary>
        /// Crear adjunto
        /// </summary>
        /// <param name="adjunto"></param>
        /// <returns></returns>
        public bool CrearAdjunto(RAAdjuntoDC adjunto)
        {
            return FabricaServicios.ServicioSolicitudesRaps.CrearAdjunto(adjunto);
        }

        /// <summary>
        /// Lista los adjuntos de una gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public List<RAAdjuntoDC> ListarAdjunto(long idGestion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ListarAdjunto(idGestion);
        }

        /// <summary>
        /// Obtener una adjunto
        /// </summary>
        /// <param name="idAdjunto"></param>
        /// <returns></returns>
        public RAAdjuntoDC ObtenerAdjunto(long idAdjunto)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerAdjunto(idAdjunto);
        }

        #endregion

        #region metodos

        /// <summary>
        /// Crea un gestion de una solicitud
        /// </summary>
        /// <param name="gestion"></param>
        /// <returns></returns>
        public bool CrearGestion(List<RAAdjuntoDC> adjunto, RAGestionDC gestion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.CrearGestion(adjunto, gestion);
        }

        /// <summary>
        /// obtiene las gestiones de una solicitud
        /// </summary>
        /// <param name="IdSolicitud"></param>
        /// <returns></returns>
        public List<RAGestionDC> ListarGestion(long IdSolicitud)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ListarGestion(IdSolicitud);
        }

        /// <summary>
        /// obtiene la informacion  de un item de gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public RAGestionDC ObtenerGestion(long idGestion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerGestion(idGestion);
        }

        /// <summary>
        /// Obtiene una plantilla de correo
        /// </summary>
        /// <param name="idPlantilla"></param>
        /// <returns></returns>
        public RAPantillaAccionCorreoDC ObtenerPantillaAccionCorreo(long idPlantilla)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerPantillaAccionCorreo(idPlantilla);
        }

        /// <summary>
        /// Crea una solicitud tarea con su respectiva parametrizacion
        /// </summary>
        /// <param name="solicitud"></param>
        /// <param name="adjuntos"></param>
        /// <param name="gestion"></param>
        /// <param name="parametrosParametrizacion"></param>
        /// <param name="parametrizacionRaps"></param>
        /// <param name="lstEscalonamiento"></param>
        /// <param name="lstTiempoEjecucion"></param>
        /// <param name="lstParametros"></param>
        /// <returns></returns>
        public long RegistrarSolicitudTarea(
            RASolicitudDC solicitud,
            List<RAAdjuntoDC> adjuntos,
            RAGestionDC gestion,
            Dictionary<string, object> parametrosParametrizacion,
            RAParametrizacionRapsDC parametrizacionRaps,
            List<RAEscalonamientoDC> lstEscalonamiento,
            List<RATiempoEjecucionRapsDC> lstTiempoEjecucion,
            List<RAParametrosParametrizacionDC> lstParametros,
            List<RAPersonaDC> lstPersonas)
        {

            return FabricaServicios.ServicioSolicitudesRaps.RegistrarSolicitudTarea(solicitud, adjuntos, gestion, parametrosParametrizacion, parametrizacionRaps, lstEscalonamiento, lstTiempoEjecucion, lstParametros, lstPersonas);
        }

        /// <summary>
        /// Crea una nueva solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        //public long CrearSolicitud(RASolicitudDC solicitud)
        //{
        //    return FabricaServicios.ServicioSolicitudesRaps.CrearSolicitud(solicitud);
        //}

        /// <summary>
        /// Listar solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RASolicitudDC> ListarSolicitud()
        {
            return FabricaServicios.ServicioSolicitudesRaps.ListarSolicitud();
        }


        #endregion

        #region creacion solicitudes

        /// <summary>
        /// lista los parametros Raps activos por tipo rap
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public IEnumerable<RAParametrizacionRapsDC> ListarParametroRapXTipoRapAct(int idTipoRap)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ListarParametroRapXTipoRapAct(idTipoRap);
        } 

        /// <summary>
        /// Crea una solicitus
        /// </summary>
        /// <param name="solicitud">objeto solicitud</param>
        /// <param name="adjunto">Objeto Ajuntos</param>
        /// <param name="gestion">Objeto Gestion</param>
        /// <returns></returns>
        public long RegistrarSolicitud(RASolicitudDC solicitud, List<RAAdjuntoDC> adjunto, RAGestionDC gestion, Dictionary<string, object> parametrosParametrizacion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.RegistrarSolicitud(solicitud, adjunto, gestion, parametrosParametrizacion);
        }
              
        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public bool CrearSolicitudAcumulativaPersonalizada(int idSistema, int idTipoNovedad, long numeroGuia, string idCiudad)
        {
            ContratoDatos.Admisiones.Mensajeria.ADGuia datosGuia;
            OUDatosMensajeroDC datosMensajero;

            try
            {
                /********************* VALIDA EXISTENCIA GUIA *************************/
                datosGuia = FabricaServicios.ServicioMensajeria.ObtenerGuiaXNumeroGuia(numeroGuia);

            }
            catch (Exception ex)
            {
                return false;
            }


            if (datosGuia.EsAutomatico)
            {
                /********************* ES AUTOMATICA *************************/

                if (datosGuia.IdMensajero != 0)
                {
                    /****************************** EMITIDA DESDE APP ********************************/
                    datosMensajero = ObtenerDatosMensajeros(datosGuia.IdMensajero);
                }
                else
                {
                    /****************************** EMITIDA DESDE OTROS SISTEMAS ********************************/
                    PUAgenciaDeRacolDC agencia = FabricaServicios.ServicioCentroServicios.ObtenerAgenciaResponsable(datosGuia.IdCentroServicioOrigen);
                    datosMensajero = new OUDatosMensajeroDC();
                    datosMensajero.IdCentroServicios = agencia.IdCentroServicio;
                    datosMensajero.Identificacion = agencia.IdResponsable.ToString();
                    datosMensajero.NombreMensajero = agencia.NombreCentroServicio;
                }
            }
            else
            {

                /********************* ES MANUAL *************************/
                var datosSuministros = FabricaServicios.ServicioSuministros.ObtenerResponsableSuministro(numeroGuia);
                datosMensajero = new OUDatosMensajeroDC();
                datosMensajero.IdCentroServicios = datosSuministros.IdCentroServicios;
                datosMensajero.Identificacion = datosSuministros.Identificacion.ToString();
                datosMensajero.NombreMensajero = string.Format("{0} {0}", datosSuministros.NombreResponsable, datosSuministros.PrimerApellido);
            }

            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();

            var administradorReglasRaps = new Comun.ReglasFallasRaps.AdministrarReglasPersonalizacionRAPS(idTipoNovedad, datosMensajero, datosGuia);

            parametrosParametrizacion = administradorReglasRaps.ObjtenerParametros();

            return FabricaServicios.ServicioSolicitudesRaps.CrearSolicitudAcumulativa(idSistema, idTipoNovedad, parametrosParametrizacion, idCiudad);
        }

        private OUDatosMensajeroDC ObtenerDatosMensajeros(long idMensajero)
        {
            var datosMensajero = FabricaServicios.ServicioOperacionUrbana.ObtenerDatosMensajero(idMensajero);
            return datosMensajero;
        }

        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        [System.Obsolete()]

        internal bool CrearSolicitudAcumulativa(int idSistema, int idTipoNovedad, Dictionary<string, object> parametros, string idCiudad)
        {

            return FabricaServicios.ServicioSolicitudesRaps.CrearSolicitudAcumulativa(idSistema, idTipoNovedad, parametros, idCiudad);
        }
            
        #endregion

        #region gestion

        public bool ResponderSolicitudRaps(RAGestionDC gestion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ResponderSolicitudRaps(gestion);
        }
        
        

        #endregion

        #region consultas
        public List<RAConteoEstadosSolicitante> ObtenerConteoEstadosSolicitudes(string idDocumentoSolicita)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerConteoEstadosSolicitudes(idDocumentoSolicita);
        }

        /// <summary>
        /// Metodo para obtener los parametro por una parametrizacion especifica
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RATiposDatosParametrizacionDC> ObtenerParamametroPorIdDeParametrizacion(long idParametrizacion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerParamametroPorIdDeParametrizacion(idParametrizacion);
        }

        /// <summary>
        /// Obtiene los tipod de novedad segun el sistema origen
        /// </summary>
        /// <param name="idSistemaOrigen"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTiposNovedad(int idSistemaOrigen, int idTipoNovedad)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerTiposNovedad(idSistemaOrigen, idTipoNovedad);
        }

        /// <summary>
        /// Retorna las veces que esta un tipo de novedad en parametrizaciones activas
        /// </summary>
        /// <param name="idTipoNovedad">Id de la novedad</param>
        /// <returns></returns>
        public RANovedadDC ObtenerCantidadTiposNovedad(long idTipoNovedad)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerCantidadTiposNovedad(idTipoNovedad);
        }

        /// <summary>
        /// Lista simplificada de las solicitudes por estado
        /// </summary>
        /// <param name="responsableSolicitud"></param>
        /// <param name="estadoSolicitud"></param>
        /// <returns></returns>
        public List<RASolicitudItemDC> ListarSolicitudes(string responsableSolicitud, RAEnumEstados estadoSolicitud)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ListarSolicitudes(responsableSolicitud, estadoSolicitud);
        }

        /// <summary>
        /// Consulta una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public RASolicitudConsultaDC ObtenerSolicitud(long idSolicitud)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerSolicitud(idSolicitud);
        }

        /// <summary>
        /// Obtiene las personas asociadas a una sucursal y un grupo especifico
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosPorGrupoYSucursal(int IdGrupo, int IdSucursal)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerEmpleadosPorGrupoYSucursal(IdGrupo, IdSucursal);
        }

        /// <summary>
        /// Obtiene el detalle de los parametros de una solicitud acumulativa
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAListaParametrosAcumulativasDC> ObtenerDetalleParametrosAcumulativas(long idsolicitud, long idParametrizacion)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerDetalleParametrosAcumulativas(idsolicitud, idParametrizacion);
        }

        /// <summary>
        /// Obtiene las fallas cometidas por un mensajero en el dia anterior al actual
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAFallaMensajeroDC> ObtenerReporteFallasPorMensajero(string idMensajero)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerReporteFallasPorMensajero(idMensajero);
        }
        #endregion

        #region FallasMensajero

        public List<RASolucitudesAutomaticasDC> ObtenerTodasFallasPorMensajero(string idresponsable, string idestado)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerTodasFallasPorMensajero(idresponsable, idestado);
        }

        public RADetalleFallasMensajeroDC ObtenerDetalleFallasMensajero(string idmensajero, string idresponsable, string idSucursal, int estado)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerDetalleFallasMensajero(idmensajero, idresponsable, idSucursal, estado);
        }

        /// <summary>
        /// Obtiene el detalle de la solicitud acumulativa a partir de la solicitud creada y la parametrizacion
        /// </summary>
        /// <param name="idsolicitud"></param>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RGDictionary> ObtenerDetalleSolicitudesAcumulativas(long idsolicitud, long idParametrizacion)
        {
            List<RGDictionary> solicitudes = FabricaServicios.ServicioSolicitudesRaps.ObtenerDetalleSolicitudesAcumulativas(idsolicitud, idParametrizacion);

            foreach (var item in solicitudes)
            {
                var indiceFoto = item.Value.IndexOf("Fotografia");
                if (indiceFoto > -1)
                {
                    string base64 = string.Empty;
                    List<string> info = item.Value.Split(',').ToList();
                    var atributo = info.Find(f => f.Contains("Fotografia"));

                    if (string.IsNullOrEmpty(atributo) || atributo.Split(':').Length < 2) continue;

                    base64 = Convert.ToBase64String(FabricaServicios.ServicioParametros.ObtenerArchivoDesdePath(atributo.Split(':')[1]));
                    item.Value = item.Value.Substring(0, indiceFoto + 11) + base64;
                }
            }

            return solicitudes;
        }



        #endregion


        #region integracionesraps
        /// <summary>
        /// Obtiene parametros visibles 
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="estadoOrigen"></param>
        /// <param name="idOringenRaps"></param>
        /// <returns></returns>
        public bool CrearFallaPersonalizadaRaps(RegistroSolicitudAppDC regSolicitud)
        {
            return FabricaServicios.ServicioSolicitudesRaps.CrearFallaPersonalizadaRaps(regSolicitud);
          //  return FabricaServicios.ServicioSolicitudesRaps.ObtieneParametrosVisibles(idTipoNovedad, numeroGuia, estadoOrigen, idOringenRaps);
        }
        #endregion
    }
}