using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using Framework.Servidor.Agenda;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using Framework.Servidor.Servicios.Contratos;

namespace Framework.Servidor.Servicios.Implementacion.Agenda
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ASAdministradorFallaSvc : IASAdministradorFallaSvc
  {
    public ASAdministradorFallaSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
    }

    /// Registra falla en el sistema

    /// <summary>
    /// Consulta las Fallas registradas en el sistema, que tengan actividades asociadas y en estado activo
    /// </summary>
    /// <param name="filtro">Expresión de filtrado de la consulta</param>
    /// <param name="totalRegistros">Total de registros que retorna la consulta</param>
    /// <param name="campoOrdenamiento">Campo por el cual se va a realizar el ordenamiento</param>
    /// <param name="indicePagina">Índie de página</param>
    /// <param name="registrosPorPagina">Cantidad de registros por página</param>
    /// <param name="esAscendente">Indica si el campo es ascendente</param>
    /// <returns>Lista de fallas</returns>
    public GenericoConsultasFramework<ASFalla> ObtenerFallas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      int totalRegistros = 0;
      return new GenericoConsultasFramework<ContratoDatos.Agenda.ASFalla>()
      {
        Lista = ASAdministradorFallas.Instancia.ObtenerFallas(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Consulta las tareas asociadas a una falla. Valida que se encuentren en estado 'ACT', es decir, activas.
    /// </summary>
    /// <param name="idFalla">Identificador de la falla</param>
    /// <param name="campoOrdenamiento">Campo por el cual se desea hacer el ordenamiento</param>
    /// <param name="esAscendente">Indica si el ordenamiento es ascendente</param>
    /// <param name="indicePagina">Índice de página</param>
    /// <param name="registrosPorPagina">Número de registros a mostrar por página</param>
    /// <returns>Lista de tareas</returns>
    public IEnumerable<ASTarea> ObtenerTareasAsociadasFalla(int idFalla, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return ASAdministradorFallas.Instancia.ObtenerTareasAsociadasFalla(idFalla, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Retorna la lista de cargos disponibles
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ASCargo> ObtenerCargos()
    {
      return ASAdministradorFallas.Instancia.ObtenerCargos();
    }

    /// <summary>
    /// Ingresa una nueva falla
    /// </summary>
    /// <param name="falla">Falla</param>
    public int AdicionarFalla(ASFalla falla)
    {
      return ASAdministradorFallas.Instancia.AdicionarFalla(falla);
    }

    /// <summary>
    /// Realiza los cambios en las fallas
    /// </summary>
    /// <param name="falla">Falla</param>
    public void GuardarCambiosFalla(ASFalla falla)
    {
      ASAdministradorFallas.Instancia.GuardarCambiosFalla(falla);
    }

    /// <summary>
    /// Realiza los cambios en las tareas
    /// </summary>
    /// <param name="tarea">tarea</param>
    public void GuardarCambiosTarea(ASTarea tarea)
    {
      ASAdministradorFallas.Instancia.GuardarCambiosTarea(tarea);
    }

    /// <summary>
    /// Hace gestión de una tarea asignada
    /// </summary>
    /// <param name="tareaAsignada">La tarea a realizar gestión</param>
    public void GestionarTarea(ASTareaAsignada tareaAsignada)
    {
      ASAdministradorFallas.Instancia.GestionarTarea(tareaAsignada);
    }

    /// <summary>
    /// Obtiene estados
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ASEstado> ObtenerEstados()
    {
      return ASAdministradorFallas.Instancia.ObtenerEstados();
    }

    /// <summary>
    /// Retorna la lista de módulos de la aplicación
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VEModulo> ObtenerModulos()
    {
      return ASAdministradorFallas.Instancia.ObtenerModulos();
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    public GenericoConsultasFramework<ASUsuario> ObtenerUsuariosConCargoInferior(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCargo)
    {
      int totalRegistros = 0;
      return new GenericoConsultasFramework<ASUsuario>()
      {
        Lista = ASAdministradorFallas.Instancia.ObtenerUsuariosConCargoInferior(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCargo),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferiorSinPaginar(int idCargo)
    {
      return ASAdministradorFallas.Instancia.ObtenerUsuariosConCargoInferior(idCargo, ControllerContext.Current.Usuario);
    }

    /// <summary>
    /// Obtiene al lista de tareas en un estado
    /// </summary>
    /// <returns></returns>
    public GenericoConsultasFramework<ASTareaAsignada> ObtenerTareasAsignadas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, string usuario)
    {
      int totalRegistros = 0;
      return new GenericoConsultasFramework<ASTareaAsignada>()
      {
        Lista = ASAdministradorFallas.Instancia.ObtenerTareasAsignadas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, usuario),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Retorna la lista de histórico asignado de eventos asignados a una tarea
    /// </summary>
    /// <param name="idTarea">Identificador tarea</param>
    /// <returns></returns>
    public IEnumerable<ASEventoTareaAsignada> ObtenerHistoricoTareaAsignada(long idTarea)
    {
      return ASAdministradorFallas.Instancia.ObtenerHistoricoTareaAsignada(idTarea);
    }

    /// <summary>
    /// Obtiene un archivo adjunto por su id
    /// </summary>
    /// <param name="idArchivo"></param>
    /// <returns></returns>
    public string ObtenerArchivoAdjunto(long idArchivo)
    {
      return ASAdministradorFallas.Instancia.ObtenerArchivoAdjunto(idArchivo);
    }

    /// <summary>
    /// Hace asignación manual de una tarea
    /// </summary>
    /// <param name="tarea">Tarea a asignar</param>
    /// <param name="comentarios">Comentarios relacionados con la asignación</param>
    /// <param name="eventoAsignacion"></param>
    /// <param name="archivos">Archivos a adjuntar a la tarea</param>
    /// <returns></returns>
    public void AsignarTareaManual(List<ASTarea> tareas, string comentarios, ASEEventoAsignacion eventoAsignacion, List<string> archivos)
    {
      ASAsignadorTarea.Instancia.AsignarTareaManual(tareas, comentarios, eventoAsignacion, ControllerContext.Current.Usuario, archivos);
    }

    /// <summary>
    /// Registra asignación de tarea manual por agenda
    /// </summary>
    /// <param name="tarea"></param>
    /// <returns></returns>
    public void AsignarTareaManualPorAgenda(ASTareaPorAgenda tarea)
    {
      ASAsignadorTarea.Instancia.AsignarTareaManualPorAgenda(tarea, ControllerContext.Current.Usuario);
    }

    /// <summary>
    /// Reasignar tareas
    /// </summary>
    /// <param name="tareaAsignada"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public void ReasignarTarea(ASTareaAsignada tareaAsignada)
    {
      ASAsignadorTarea.Instancia.ReasignarTarea(tareaAsignada, ControllerContext.Current.Usuario);
    }
  }
}