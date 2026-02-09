
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;
namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IRAConfiguracionRapsSvc
    {
        /// <summary>
        /// Listar Acciones
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAAccionDC> ListarAccion();

        /// <summary>
        /// obtener una accion
        /// </summary>
        /// <param name="idAccion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAAccionDC ObtenerAccion(short idAccion);

        /// <summary>
        /// Crear una accion
        /// </summary>
        /// <param name="accion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearAccion(RAAccionDC accion);

        /// <summary>
        /// Crear accion plantilla parametrizacion raps
        /// </summary>
        /// <param name="accionPlantillaParametrizacionRaps"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearAccionPlantillaParametrizacionRaps(RAAccionPlantillaParametrizacionRapsDC accionPlantillaParametrizacionRaps);

        /// <summary>
        /// Listar acciones plantilla parametrizacion raps
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAAccionPlantillaParametrizacionRapsDC> ListarAccionPlantillaParametrizacionRaps();

        /// <summary>
        /// Obtiene una plantilla de parametrizacionRaps para una accion
        /// </summary>
        /// <param name="idAccionPlantilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAAccionPlantillaParametrizacionRapsDC ObtenerAccionPlantillaParametrizacionRaps(long idAccionPlantilla);



        /// <summary>
        /// Crea un nuevo cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearCargo(RACargoDC cargo);

        /// <summary>
        /// Listar los cargos
        /// </summary>
        /// <param name="idCargo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RACargoDC> ObtenerCargos(int idCargo);

        /// <summary>
        /// Crea una nueva clasificacion
        /// </summary>
        /// <param name="clasificacion"></param>
        /// <returns>Verdadero al grabar </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearClasificacion(RAClasificacionDC clasificacion);


        /// <summary>
        /// Lista las clasificaciones
        /// </summary>
        /// <returns>Lista la clasificacion</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAClasificacionDC> ListarClasificacion();

        /// <summary>
        /// Obtiene una clasificacion
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns>Clasificacion</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAClasificacionDC ObtenerClasificacion(int idClasificacion);

        /// <summary>
        /// Crea un registro de escalonamiento
        /// </summary>
        /// <param name="escalonamiento"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearEscalonamiento(RAEscalonamientoDC escalonamiento);

        /// <summary>
        /// Lista los registros de escalonamiento para una parametrizacion
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns>Lista de escalonamiento</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAEscalonamientoDC> ListarEscalonamiento(long idParametrizacionRap);

        /// <summary>
        /// consulta un item de escalonamiento
        /// </summary>
        /// <param name="idEscalonamiento"></param>
        /// <returns>Escalonamiento</returns>
        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //RAEscalonamientoDC ObtenerEscalonamiento(long idEscalonamiento);

        /// <summary>
        /// Crear estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearEstados(RAEstadosDC estado);

        /// <summary>
        /// Lista los estados existentes
        /// </summary>
        /// <returns>Lista estado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAEstadosDC> ListarEstados(bool esResponsable, int idEstadoActual, int idCargoSolicita, long idSolicitud);

        /// <summary>
        /// Consulta un estado
        /// </summary>
        /// <param name="IdEstado"></param>
        /// <returns>objeto estado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAEstadosDC ObtenerEstado(int IdEstado);

        /// <summary>
        /// Crea un nuevo item en la tabla Flujo Accion Estado
        /// </summary>
        /// <param name="flujoAccionEstado"></param>
        /// <returns>Verdaro si todo correcto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearFlujoAccionEstado(RAFlujoAccionEstadoDC flujoAccionEstado);

        /// <summary>
        /// lista los item de un flujo
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAFlujoAccionEstadoDC> ListarFlujoAccionEstado(int idFlujo);

        /// <summary>
        /// Obtiene un registro de flujo Accion Estado
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAFlujoAccionEstadoDC ObtenerFlujoAccionEstado(int idFlujo, byte idAccion, int idEstado, int idCargo);

        /// <summary>
        /// Crea un formato
        /// </summary>
        /// <param name="formato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearFormato(RAFormatoDC formato);

        /// <summary>
        /// lista los formatos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAFormatoDC> ListarFormato();

        /// <summary>
        /// obitiene un registro de formato
        /// </summary>
        /// <param name="idFormato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAFormatoDC ObtenerFormato(int idFormato);

        /// <summary>
        /// Crea un nuevo grupo usuario
        /// </summary>
        /// <param name="grupoUsuario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearGrupoUsuario(RAGrupoUsuarioDC grupoUsuario);

        /// <summary>
        /// Lista los grupos de usuario
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAGrupoUsuarioDC> ListarGrupoUsuario();

        /// <summary>
        /// Obtiene un registro de grupo usuario
        /// </summary>
        /// <param name="idGrupoUsuario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAGrupoUsuarioDC ObtenerGrupoUsuario(int idGrupoUsuario);

        /// <summary>
        /// Crea una nueva planilla de correo para una accion
        /// </summary>
        /// <param name="plantillaAccionCorreo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearPantillaAccionCorreo(RAPantillaAccionCorreoDC plantillaAccionCorreo);

        /// <summary>
        /// Lista las plantillas para una accion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAPantillaAccionCorreoDC> ListarPantillaAccionCorreo(byte idAccion);


        /// <summary>
        /// Lista de Parametrizaciones de solicitudes
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAParametrizacionRapsDC> ListarParametrizacionRaps();


        /// <summary>
        /// Crea un registro de proceso
        /// </summary>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearProceso(RAProcesoDC proceso);

        /// <summary>
        /// Lista los procesos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAProcesoDC> ListarProceso();

        /// <summary>
        /// Consulta un proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAProcesoDC ObtenerProceso(int idProceso);

        /// <summary>
        /// Crea un registro de quien cierra
        /// </summary>
        /// <param name="quienCierra"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearTipoCierre(RATipoCierreDC quienCierra);

        /// <summary>
        /// Lista registros quien cierra
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoCierreDC> ListarTipoCierre();

        /// <summary>
        /// consulta la informacion un registro Quien Cierra
        /// </summary>
        /// <param name="idQuienCierra"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RATipoCierreDC ObtenerTipoCierre(int idQuienCierra);

        /// <summary>
        /// crea un nuevo registro de sistema formato
        /// </summary>
        /// <param name="sistemaFormato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearSistemaFormato(RASistemaFormatoDC sistemaFormato);

        /// <summary>
        /// lista los registros de sistema formato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RASistemaFormatoDC> ListarSistemaFormato();

        /// <summary>
        /// Consulta un registro de sistema formato
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RASistemaFormatoDC ObtenerSistemaFormato(int idSistemaFormato);

        /// <summary>
        /// Crea una nueva subclasificacion
        /// </summary>
        /// <param name="subClasificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearSubClasificacion(RASubClasificacionDC subClasificacion);

        /// <summary>
        /// Lista las sub clasificaciones de una clasificacion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RASubClasificacionDC> ListarSubClasificacion(int idClasificacion);

        /// <summary>
        /// Consulta una subclasificacion
        /// </summary>
        /// <param name="IdSubclasificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RASubClasificacionDC ObtenerSubClasificacion(int IdSubclasificacion);

        /// <summary>
        /// Crea un nuevo tiempo de ejecucion
        /// </summary>
        /// <param name="tiempoEjecucionRaps"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearTiempoEjecucionRaps(RATiempoEjecucionRapsDC tiempoEjecucionRaps);

        /// <summary>
        /// Lista los tiempo de ejecucion para una parametrizacion de Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATiempoEjecucionRapsDC> ListarTiempoEjecucionRaps(long idParametrizacionRap);

        /// <summary>
        /// Consulta registro tiempo de ejecucion Raps
        /// </summary>
        /// <param name="idEjecucion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RATiempoEjecucionRapsDC ObtenerTiempoEjecucionRaps(long idEjecucion);

        /// <summary>
        /// Crea un tipo de hora
        /// </summary>
        /// <param name="tipoHora"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearTipoHora(RATipoHoraDC tipoHora);

        /// <summary>
        /// Lista los tipos de hora
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoHoraDC> ListarTipoHora();

        /// <summary>
        /// Consulta un tipo de hora
        /// </summary>
        /// <param name="idTipoHora"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RATipoHoraDC ObtenerTipoHora(int idTipoHora);

        /// <summary>
        /// Crea un nuevo tipo de periodo
        /// </summary>
        /// <param name="tipoPeriodo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearTipoPeriodo(RATipoPeriodoDC tipoPeriodo);

        /// <summary>
        /// Lista los tipos de periodos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoPeriodoDC> ListarTipoPeriodo();

        /// <summary>
        /// Consulta un tipo de periodo
        /// </summary>
        /// <param name="idTipoPeriodo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RATipoPeriodoDC ObtenerTipoPeriodo(int idTipoPeriodo);

        /// <summary>
        /// Crea un nuevo tipo de Rap
        /// </summary>
        /// <param name="tipoRap"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearTipoRap(RATipoRapDC tipoRap);

        /// <summary>
        /// Lista los tipos de Raps
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoRapDC> ListarTipoRap();

        /// <summary>
        /// Consulta un tipo de rap
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RATipoRapDC ObtenerTipoRap(int idTipoRap);

        /// <summary>
        /// Crear origen raps
        /// </summary>
        /// <param name="origenRaps"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearOrigenRaps(RAOrigenRapsDC origenRaps);

        /// <summary>
        /// Metodo para Obtener las territoriales
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATerritorialDC> ObtenerTerritoriales();

        /// <summary>
        /// Obtiene todas las regionales
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RARegionalSuculsalDC> ObtenerRegionales();


        /// <summary>
        /// Obtiene el personal con informacion novasoft
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAPersonaDC> ObtenerPersonal(string cargos, int idParametrizacion, int idSucursal);

        /// <summary>
        /// Inserta las notificaciones que no pudieron ser enviadas al usuario
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarNotificacionPendiente(string mensaje, long idUsuario, long idSolicitud);

        /// <summary>
        /// Obtiene los menus permitidos para cada usuario 
        /// </summary>
        /// <param name="modulosPermitidos"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAMenusPermitidosDC> ObtenerMenusPermitidos(List<RAModulosDC> modulosPermitidos);

        /// <summary>
        /// obtiene los procesos
        /// </summary>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAProcesoDC> ObtenerProcesos();

        /// <summary>
        /// Obtiene los procedimientos de determinado proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAProcedimientoDC> ObtenerProcedimientoPorproceso(int idProceso);

        /// <summary>
        /// Obtiene los procedimientos de determinados procesos
        /// </summary>
        /// <param name="procesos"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAProcedimientoDC> ObtenerProcedimientosPorprocesos(string procesos);

        /// <summary>
        /// crea el grupo con sus correspondientes cargos
        /// </summary>
        /// <param name="grupo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CrearGrupoCargo(RACargoGrupoDC grupo);

        /// <summary>
        /// Lista todos los grupos creados
        /// </summary>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RACargoGrupoDC> ListarGrupos();


        /// <summary>
        /// Obtiene el cargo con sus respectivos cargos
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="porPersona"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RACargoGrupoDC ObtenerDetallaGrupo(int idGrupo, bool porPersona);

        /// <summary>
        /// Elimina un grupo por su id
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool EliminarGrupo(int idGrupo);

        /// <summary>
        /// agrega n cargos a un grupo ya creado
        /// </summary>
        /// <param name="cargos"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarCargosDeGrupo(RACargoGrupoDC cargos);

        /// <summary>
        /// Edita la informacion de un grupo
        /// </summary>
        /// <param name="infoGrupo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarGrupo(RACargoGrupoDC infoGrupo);

        /// <summary>
        /// obtiene las notificaciones pendientes por usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RANotificacionDC> ListarNotificacionesPendientes(string idUsuario);

        /// <summary>
        /// Cambia de estato 0 a 1 las notificaciones vistas por un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarestadoNotificacion(string idUsuario);

        /// <summary>
        /// Obtiene las solicitudes que tienen como fecha de vencimiento hoy y/o mañana
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        List<RASolicitudDC> ObtenerSolicitudesProximasAVencer(string idUsuario);

        /// <summary>
        /// Listar Acciones
        /// </summary>
        /// <returns></returns>
        [System.Obsolete()]
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAParametrosPersonalizacionRapsDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad);



        #region activos

        /// <summary>
        /// Listar los tipos de raps activos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoRapDC> ListarTipoRapAct();

        /// <summary>
        /// listar los estados activos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAEstadosDC> ListarEstadosAct(bool esResponsable, int idEstadoActual);

        /// <summary>
        /// listar los origenes de raps
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAOrigenRapsDC> ListarOrigenRaps();

        /// <summary>
        /// Listar tipo cierres activos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoCierreDC> ListarTipoCierreAct();

        /// <summary>
        /// lista las clasificaciones
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAClasificacionDC> ListarClasificacionAct();

        /// <summary>
        /// lista las  subclasificaciones
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RASubClasificacionDC> ListarSubClasificacionAct(int idClasificacion);

        /// <summary>
        /// Listar sistemas formato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RASistemaFormatoDC> ListarSistemaFormatoAct();


        /// <summary>
        /// Listar formatos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAFormatoDC> ListarFormatoAct(int idSistemaFormato);

        /// <summary>
        /// Lista los grupos de usuario activos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAGrupoUsuarioDC> ListarGrupoUsuarioAct();

        /// <summary>
        /// Lista los cargos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RACargoDC> ListarCargos();

        /// <summary>
        /// Cambia el estado de una parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool CambiaEstadoParametrizacionRaps(long idParametrizacionRap, bool estado);

        /// <summary>
        /// lista los tipos de periodo activos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]

        List<RATipoPeriodoDC> ListarTipoPeriodoAct();

        /// <summary>
        /// Obtener origen Raps
        /// </summary>
        /// <param name="idOrigenRaps"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAOrigenRapsDC ObtenerOrigenRaps(int idOrigenRaps);


        /// <summary>
        /// Lista los tipos de Incumplimiento
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoIncumplimientoDC> ListarTipoIncumplimiento();


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RARegionalSuculsalDC> ListarRegionalCargo();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ContratoDatos.Raps.Consultas.RAPaginaParametrizacionRapsDC> ListarParametrizacionRapsPaginada(int pagina, int registrosXPagina, int tipoRap, string ordenaPor);

        #endregion

        #region parametrizacion

        /// <summary>
        /// retorna los tipos de datos disponibles
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RATipoDatoDC> ObtenerTiposDatos();

        #region Obtener Parametrización
        /// <summary>
        /// consulta la informacion de una parametrizacion de solicitudes 
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RAParametrizacionRapsDC ObtenerParametrizacionRaps(long idParametrizacionRap);
        #endregion


        #region Crear parametrizacion, Escalonamientos y Tiempos de Ejecución
        /// <summary>
        /// Insertar Parametrizacion, Escalonamiento y Tiempo de Ejecucion (esta es la firma, porque aunque es el mismo método varía en la cantidad de parametros que le envío)
        /// </summary>
        /// <param name="parametrizacionRaps"></param>
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CrearParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros);
        #endregion


        #region Modifcar parametrizacion, escalonamiento y tiempos de ejecucion de una raps
        ///// <summary>
        ///// Modifica la Parametrizacion, el Escalonamiento y Tiempos de Ejecución Raps.
        ///// </summary>
        ///// <param name="parametrizacionRaps"></param>
        ///// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //void ActualizarDetalleParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion);

        /// <summary>
        /// Actualizar Parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacion"></param>   
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros);

        /// <summary>
        /// Cambia el estado de una parametrizacion
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <param name="estaActivo"></param>
        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //void CambiarEstadoParametrizacion(int idParametrizacion, bool estaActivo);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RAParametrosParametrizacionDC> ListarParametros(long idParametrizacion);

        #endregion

        #endregion

        #region Fallas Interlogis / Web

        /// <summary>
        /// Metodo para obtener los tipos de novedades segun responsable 
        /// </summary>
        /// <param name="idClaseResponsable"></param>
        /// <param name="idSistemaFuente"></param>
        /// <returns></returns>
        List<RANovedadDC> ObtenerTipoNovedadSegunResponsable(int idClaseResponsable, int idSistemaFuente);

        #endregion
    }
}
