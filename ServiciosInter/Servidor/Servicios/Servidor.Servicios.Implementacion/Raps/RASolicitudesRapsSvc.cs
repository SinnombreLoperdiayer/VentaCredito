using CO.Servidor.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using System;
//using CO.Servidor.Raps.ReglasFallasRaps;

namespace CO.Servidor.Servicios.Implementacion.Raps
{
    /// <summary>
    /// Clase para los servicios de gestion de las solicitudes de Raps
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RASolicitudesRapsSvc : IRASolicitudesRaps
    {
        #region constructor

        public RASolicitudesRapsSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion

        //#region metodos      

        ///// <summary>
        ///// Crea una nueva solicitud
        ///// </summary>
        ///// <param name="solicitud"></param>
        ///// <returns></returns>
        //public long CrearSolicitud(RASolicitudDC solicitud)
        //{
        //    return RASolicitudes.Instancia.CrearSolicitud(solicitud);
        //}

        //#endregion

        #region Adjunto

        /// <summary>
        /// Crear adjunto
        /// </summary>
        /// <param name="adjunto"></param>
        /// <returns></returns>
        public bool CrearAdjunto(RAAdjuntoDC adjunto)
        {
            return RASolicitudes.Instancia.CrearAdjunto(adjunto);
        }
        #endregion

        #region creacion solicitudes       

        /// <summary>
        /// registra una solicitud
        /// </summary>
        /// <param name="solicitud">obg solicitud</param>
        /// <param name="adjunto">lista objs Adjuntos</param>
        /// <param name="gestion">obj Gestion</param>
        /// <returns></returns>
        public long RegistrarSolicitud(RASolicitudDC solicitud, List<RAAdjuntoDC> adjunto, RAGestionDC gestion, Dictionary<string, object> parametrosParametrizacion)
        {
            return RASolicitudes.Instancia.RegistrarSolicitud(solicitud, adjunto, gestion, parametrosParametrizacion);
        }

        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public bool CrearSolicitudAcumulativa(int idSistema, int idTipoNovedad, Dictionary<string, object> parametros, string idCiudad)
        {
            return RASolicitudes.Instancia.CrearSolicitudAcumulativa(idSistema, idTipoNovedad, parametros, idCiudad);
        }             

     
        #endregion

        #region Gestion
        public bool ResponderSolicitudRaps(RAGestionDC gestion)
        {
            return RASolicitudes.Instancia.ResponderSolicitudesRaps(gestion);
        }

        /// <summary>
        /// Crea un gestion de una solicitud
        /// </summary>
        /// <param name="gestion"></param>
        /// <returns></returns>
        public bool CrearGestion(List<RAAdjuntoDC> adjunto, RAGestionDC gestion)
        {
            return RASolicitudes.Instancia.CrearGestion(adjunto, gestion);
        }
        #endregion        

        #region MotorRaps
        /// <summary>
        /// Obtiene los horarios del empleado para el cual se realizara el escalamiento de un rap
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idSucursal"></param>
        public RACargoEscalarDC ObtenerHorariosEmpleadoEscalarPorCargoSucursal(RACargoEscalarDC cargoEscalar)
        {
            return RASolicitudes.Instancia.ObtenerHorariosEmpleadoEscalarPorCargoSucursal(cargoEscalar);

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
            return RASolicitudes.Instancia.RegistrarSolicitudTarea(
                solicitud,
                adjuntos,
                gestion,
                parametrosParametrizacion,
                parametrizacionRaps,
                lstEscalonamiento,
                lstTiempoEjecucion,
                lstParametros,
                lstPersonas);
        }

        #endregion

        #region consulta
        /// <summary>
        /// Metodo para obtener los parametro por una parametrizacion especifica
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RATiposDatosParametrizacionDC> ObtenerParamametroPorIdDeParametrizacion(long idParametrizacion)
        {
            return RASolicitudes.Instancia.ObtenerParamametroPorIdDeParametrizacion(idParametrizacion);
        }

        public List<RAObtenerListaSolicitudesRaps> ObtenerListaSolicitudesRaps(long DocumentoSolicita, int IdEstado)
        {
            return RASolicitudes.Instancia.ObtenerListaSolicitudesRaps(DocumentoSolicita, IdEstado);
        }

        /// <summary>
        /// lista los parametros raps activos por tipo raps
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public IEnumerable<RAParametrizacionRapsDC> ListarParametroRapXTipoRapAct(int idTipoRap)
        {
            return RASolicitudes.Instancia.ListarParametroRapXTipoRapAct(idTipoRap);
        }

        /// <summary>
        /// Listar solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RASolicitudDC> ListarSolicitud()
        {
            return RASolicitudes.Instancia.ListarSolicitud();
        }        

        /// <summary>
        /// obtiene las gestiones de una solicitud
        /// </summary>
        /// <param name="IdSolicitud"></param>
        /// <returns></returns>
        public List<RAGestionDC> ListarGestion(long IdSolicitud)
        {
            return RASolicitudes.Instancia.ListarGestion(IdSolicitud);
        }

        /// <summary>
        /// obtiene la informacion  de un item de gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public RAGestionDC ObtenerGestion(long idGestion)
        {
            return RASolicitudes.Instancia.ObtenerGestion(idGestion);
        }

        /// <summary>
        /// Obtiene una plantilla de correo
        /// </summary>
        /// <param name="idPlantilla"></param>
        /// <returns></returns>
        public RAPantillaAccionCorreoDC ObtenerPantillaAccionCorreo(long idPlantilla)
        {
            return RASolicitudes.Instancia.ObtenerPantillaAccionCorreo(idPlantilla);
        }

      

        /// <summary>
        /// Lista los adjuntos de una gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public List<RAAdjuntoDC> ListarAdjunto(long idGestion)
        {
            return RASolicitudes.Instancia.ListarAdjunto(idGestion);
        }

        /// <summary>
        /// Lista simplificada de las solicitudes por estado
        /// </summary>
        /// <param name="responsableSolicitud"></param>
        /// <param name="estadoSolicitud"></param>
        /// <returns></returns>
        public List<RASolicitudItemDC> ListarSolicitudes(string responsableSolicitud, RAEnumEstados estadoSolicitud)
        {
            return RASolicitudes.Instancia.ListarSolicitudes(responsableSolicitud, estadoSolicitud);
        }

        /// <summary>
        /// Obtener una adjunto
        /// </summary>
        /// <param name="idAdjunto"></param>
        /// <returns></returns>
        public RAAdjuntoDC ObtenerAdjunto(long idAdjunto)
        {
            return RASolicitudes.Instancia.ObtenerAdjunto(idAdjunto);
        }

        /// <summary>
        /// Consulta una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public RASolicitudConsultaDC ObtenerSolicitud(long idSolicitud)
        {
            return RASolicitudes.Instancia.ObtenerSolicitud(idSolicitud);
        }

        public List<RAConteoEstadosSolicitante> ObtenerConteoEstadosSolicitudes(string idDocumentoSolicita)
        {
            return RASolicitudes.Instancia.ObtenerConteoEstadosSolicitudes(idDocumentoSolicita);
        }

        /// <summary>
        /// Obtiene las fallas cometidas por un mensajero en el dia anterior al actual
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAFallaMensajeroDC> ObtenerReporteFallasPorMensajero(string idMensajero)
        {
            return RASolicitudes.Instancia.ObtenerReporteFallasPorMensajero(idMensajero);
        }

        /// <summary>
        /// Obtiene los tipod de novedad segun el sistema origen
        /// </summary>
        /// <param name="idSistemaOrigen"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTiposNovedad(int idSistemaOrigen, int idTipoNovedad)
        {
            return RASolicitudes.Instancia.ObtenerTiposNovedad(idSistemaOrigen, idTipoNovedad);
        }

        /// <summary>
        /// Retorna las veces que esta un tipo de novedad en parametrizaciones activas
        /// </summary>
        /// <param name="idTipoNovedad">Id de la novedad</param>
        /// <returns></returns>
        public RANovedadDC ObtenerCantidadTiposNovedad(long idTipoNovedad)
        {
            return RASolicitudes.Instancia.ObtenerCantidadTiposNovedad(idTipoNovedad);
        }

        /// <summary>
        /// Obtiene las personas asociadas a una sucursal y un grupo especifico
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosPorGrupoYSucursal(int IdGrupo, int IdSucursal)
        {
            return RASolicitudes.Instancia.ObtenerEmpleadosPorGrupoYSucursal(IdGrupo, IdSucursal);
        }
        #endregion

        #region FallasMensajero

        public List<RASolucitudesAutomaticasDC> ObtenerTodasFallasPorMensajero(string idresponsable, string idestado)
        {
            return RASolicitudes.Instancia.ObtenerTodasFallasPorMensajero(idresponsable, idestado);
        }
        public RADetalleFallasMensajeroDC ObtenerDetalleFallasMensajero(string idmensajero, string idresponsable, string idSucursal, int estado)
        {
            return RASolicitudes.Instancia.ObtenerDetalleFallasMensajero(idmensajero, idresponsable, idSucursal, estado);
        }
        /// <summary>
        /// Obtiene el detalle de la solicitud acumulativa a partir de la solicitud creada y la parametrizacion
        /// </summary>
        /// <param name="idsolicitud"></param>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RGDictionary> ObtenerDetalleSolicitudesAcumulativas(long idsolicitud, long idParametrizacion)
        {
            return RASolicitudes.Instancia.ObtenerDetalleSolicitudesAcumulativas(idsolicitud, idParametrizacion);
        }


        /// <summary>
        /// Obtiene el detalle de los parametros de una solicitud acumulativa
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAListaParametrosAcumulativasDC> ObtenerDetalleParametrosAcumulativas(long idsolicitud, long idParametrizacion)
        {
            return RASolicitudes.Instancia.ObtenerDetalleParametrosAcumulativas(idsolicitud, idParametrizacion);
        }

        #endregion

        #region IntegracionesRaps
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
            return RASolicitudes.Instancia.CrearFallaPersonalizadaRaps(regSolicitud);
        }
        #endregion
       


    }
}
