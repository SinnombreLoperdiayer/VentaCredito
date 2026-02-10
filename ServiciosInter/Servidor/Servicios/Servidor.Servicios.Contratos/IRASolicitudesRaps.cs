using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IRASolicitudesRaps
    {

        #region metodos

        /// <summary>
        /// Crea un gestion de una solicitud
        /// </summary>
        /// <param name="gestion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearGestion(List<RAAdjuntoDC> adjunto, RAGestionDC gestion);

        /// <summary>
        /// obtiene las gestiones de una solicitud
        /// </summary>
        /// <param name="IdSolicitud"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAGestionDC> ListarGestion(long IdSolicitud);

        /// <summary>
        /// obtiene la informacion  de un item de gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAGestionDC ObtenerGestion(long idGestion);

        /// <summary>
        /// Obtiene una plantilla de correo
        /// </summary>
        /// <param name="idPlantilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAPantillaAccionCorreoDC ObtenerPantillaAccionCorreo(long idPlantilla);

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
        long RegistrarSolicitudTarea(
            RASolicitudDC solicitud,
            List<RAAdjuntoDC> adjuntos,
            RAGestionDC gestion,
            Dictionary<string, object> parametrosParametrizacion,
            RAParametrizacionRapsDC parametrizacionRaps,
            List<RAEscalonamientoDC> lstEscalonamiento,
            List<RATiempoEjecucionRapsDC> lstTiempoEjecucion,
            List<RAParametrosParametrizacionDC> lstParametros,
            List<RAPersonaDC> lstPersonas);

        ///// <summary>
        ///// Crea una nueva solicitud
        ///// </summary>
        ///// <param name="solicitud"></param>
        ///// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //long CrearSolicitud(RASolicitudDC solicitud);


        /// <summary>
        /// Listar solicitudes
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RASolicitudDC> ListarSolicitud();

        #endregion

        #region adjunto
        /// <summary>
        /// Crear adjunto
        /// </summary>
        /// <param name="adjunto"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearAdjunto(RAAdjuntoDC adjunto);

        /// <summary>
        /// Lista los adjuntos de una gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAAdjuntoDC> ListarAdjunto(long idGestion);

        /// <summary>
        /// Obtener una adjunto
        /// </summary>
        /// <param name="idAdjunto"></param>
        /// <returns></returns>
        RAAdjuntoDC ObtenerAdjunto(long idAdjunto);
        #endregion

        #region creacion solicitudes

        /// <summary>
        /// lista los parametros raps activos
        /// </summary>
        /// <param name="idTipoRap">Tipo de Rap</param>
        /// <returns>lista parametros raps</returns>        
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<RAParametrizacionRapsDC> ListarParametroRapXTipoRapAct(int idTipoRap);

        /// <summary>
        /// registra una solicitud
        /// </summary>
        /// <param name="solicitud">obg solicitud</param>
        /// <param name="adjunto">lista objs Adjuntos</param>
        /// <param name="gestion">obj Gestion</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long RegistrarSolicitud(RASolicitudDC solicitud, List<RAAdjuntoDC> adjunto, RAGestionDC gestion, Dictionary<string, object> parametrosParametrizacion);


        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        [System.Obsolete()]
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearSolicitudAcumulativa(int idSistema, int idTipoNovedad, Dictionary<string, object> parametros, string idCiudad);


        #endregion

        #region Gestion
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ResponderSolicitudRaps(RAGestionDC gestion);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAConteoEstadosSolicitante> ObtenerConteoEstadosSolicitudes(string idDocumentoSolicita);



        #endregion

        #region consultas solicitudes
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATiposDatosParametrizacionDC> ObtenerParamametroPorIdDeParametrizacion(long idParametrizacion);

        /// <summary>
        /// Obtiene los tipod de novedad segun el sistema origen
        /// </summary>
        /// <param name="idSistemaOrigen"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RANovedadDC> ObtenerTiposNovedad(int idSistemaOrigen, int idTipoNovedad);

        /// <summary>
        /// Retorna las veces que esta un tipo de novedad en parametrizaciones activas
        /// </summary>
        /// <param name="idTipoNovedad">Id de la novedad</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RANovedadDC ObtenerCantidadTiposNovedad(long idTipoNovedad);

        /// <summary>
        /// Lista simplificada de las solicitudes por estado
        /// </summary>
        /// <param name="responsableSolicitud"></param>
        /// <param name="estadoSolicitud"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RASolicitudItemDC> ListarSolicitudes(string responsableSolicitud, RAEnumEstados estadoSolicitud);

        /// <summary>
        /// Consulta una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RASolicitudConsultaDC ObtenerSolicitud(long idSolicitud);

        /// <summary>
        /// Obtiene las personas asociadas a una sucursal y un grupo especifico
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAIdentificaEmpleadoDC> ObtenerEmpleadosPorGrupoYSucursal(int IdGrupo, int IdSucursal);

        /// <summary>
        /// Obtiene las fallas cometidas por un mensajero en el dia anterior al actual
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAFallaMensajeroDC> ObtenerReporteFallasPorMensajero(string idMensajero);
        #endregion

        #region MotorRaps
        /// <summary>
        /// Obtiene los horarios del empleado para el cual se realizara el escalamiento de un rap
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idSucursal"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RACargoEscalarDC ObtenerHorariosEmpleadoEscalarPorCargoSucursal(RACargoEscalarDC cargoEscalar);



        #endregion

    }
}
