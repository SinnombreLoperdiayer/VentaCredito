using System.Collections.Generic;
using System.ServiceModel;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace Framework.Servidor.Servicios.Contratos
{
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IASAdministradorFallaSvc
  {
    /// Consulta las Fallas registradas en el sistema, que tengan actividades asociadas y en estado activo
    /// </summary>
    /// <param name="filtro">Expresión de filtrado de la consulta</param>
    /// <param name="totalRegistros">Total de registros que retorna la consulta</param>
    /// <param name="campoOrdenamiento">Campo por el cual se va a realizar el ordenamiento</param>
    /// <param name="indicePagina">Índie de página</param>
    /// <param name="registrosPorPagina">Cantidad de registros por página</param>
    /// <param name="esAscendente">Indica si el campo es ascendente</param>
    /// <returns>Lista de fallas</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<ASFalla> ObtenerFallas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

    /// <summary>
    /// Consulta las tareas asociadas a una falla. Valida que se encuentren en estado 'ACT', es decir, activas.
    /// </summary>
    /// <param name="idFalla">Identificador de la falla</param>
    /// <param name="campoOrdenamiento">Campo por el cual se desea hacer el ordenamiento</param>
    /// <param name="esAscendente">Indica si el ordenamiento es ascendente</param>
    /// <param name="indicePagina">Índice de página</param>
    /// <param name="registrosPorPagina">Número de registros a mostrar por página</param>
    /// <returns>Lista de tareas</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<ASTarea> ObtenerTareasAsociadasFalla(int idFalla, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

    /// <summary>
    /// Retorna la lista de cargos disponibles
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<ASCargo> ObtenerCargos();

    /// <summary>
    /// Ingresa una nueva falla
    /// </summary>
    /// <param name="falla">Falla</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    int AdicionarFalla(ASFalla falla);

    /// <summary>
    /// Realiza los cambios en las fallas
    /// </summary>
    /// <param name="falla">Falla</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GuardarCambiosFalla(ASFalla falla);

    /// <summary>
    /// Realiza los cambios en las tareas
    /// </summary>
    /// <param name="tarea">tarea</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GuardarCambiosTarea(ASTarea tarea);

    /// <summary>
    /// Hace gestión de una tarea asignada
    /// </summary>
    /// <param name="tareaAsignada">La tarea a realizar gestión</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GestionarTarea(ASTareaAsignada tareaAsignada);

    /// <summary>
    /// Obtiene estados
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<ASEstado> ObtenerEstados();

    /// <summary>
    /// Retorna la lista de módulos de la aplicación
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<VEModulo> ObtenerModulos();

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<ASUsuario> ObtenerUsuariosConCargoInferior(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCargo);

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferiorSinPaginar(int idCargo);

    /// <summary>
    /// Obtiene al lista de tareas en un estado
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<ASTareaAsignada> ObtenerTareasAsignadas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, string usuario);

    /// <summary>
    /// Retorna la lista de histórico asignado de eventos asignados a una tarea
    /// </summary>
    /// <param name="idTarea">Identificador tarea</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<ASEventoTareaAsignada> ObtenerHistoricoTareaAsignada(long idTarea);

    /// <summary>
    /// Obtiene un archivo adjunto por su id
    /// </summary>
    /// <param name="idArchivo"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    string ObtenerArchivoAdjunto(long idArchivo);

    /// <summary>
    /// Hace asignación manual de una tarea
    /// </summary>
    /// <param name="tarea">Tarea a asignar</param>
    /// <param name="comentarios">Comentarios relacionados con la asignación</param>
    /// <param name="eventoAsignacion"></param>
    /// <param name="archivos">Archivos a adjuntar a la tarea</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void AsignarTareaManual(List<ASTarea> tareas, string comentarios, ASEEventoAsignacion eventoAsignacion, List<string> archivos);

    /// <summary>
    /// Registra asignación de tarea manual por agenda
    /// </summary>
    /// <param name="tarea"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void AsignarTareaManualPorAgenda(ASTareaPorAgenda tarea);

    /// <summary>
    /// Reasignar tareas
    /// </summary>
    /// <param name="tareaAsignada"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void ReasignarTarea(ASTareaAsignada tareaAsignada);
  }
}