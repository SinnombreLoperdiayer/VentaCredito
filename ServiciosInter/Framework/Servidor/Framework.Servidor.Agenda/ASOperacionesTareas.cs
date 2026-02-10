using System;
using System.Collections.Generic;
using Framework.Servidor.Agenda.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace Framework.Servidor.Agenda
{
  /// <summary>
  /// Hace operaciones relacionadas con la consulta de tareas
  /// </summary>
  public class ASOperacionesTareas : MarshalByRefObject
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly ASOperacionesTareas instancia = (ASOperacionesTareas)FabricaInterceptores.GetProxy(new ASOperacionesTareas(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_AGENDA);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static ASOperacionesTareas Instancia
    {
      get { return ASOperacionesTareas.instancia; }
    }

    #endregion Instancia Singleton

    /// <summary>
    /// Obtiene al lista de tareas en un estado
    /// </summary>
    /// <param name="usuario">Usuario que solicita la lista de tareas</param>
    /// <returns></returns>
    public IEnumerable<ASTareaAsignada> ObtenerTareasAsignadas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string usuario)
    {
      return ASRepositorio.Instancia.ObtenerTareasAsignadas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, usuario);
    }

    /// <summary>
    /// Consulta las tareas asociadas a una falla. Valida que se encuentren en estado 'ACT', es decir, activas.
    /// </summary>
    /// <param name="idFalla">Identificador de la falla</param>
    /// <param name="campoOrdenamiento">Campo por el cual se desea hacer el ordenamiento</param>
    /// <param name="esAscendente">Indica si el ordenamiento es ascendente</param>
    /// <param name="indicePagina">Índice de página</param>
    /// <param name="registrosPorPagina">Número de registros a mostrar por página</param>
    /// <param name="totalRegistros">Retorna el total de registros que cumplen con la consulta</param>
    /// <returns>Lista de tareas</returns>
    public IEnumerable<ASTarea> ObtenerTareasAsociadasFalla(int idFalla, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente, out int totalRegistros)
    {
      return ASRepositorio.Instancia.ObtenerTareasAsociadasFalla(idFalla, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente, out totalRegistros);
    }

    /// <summary>
    /// Retorna la lista de histórico asignado de eventos asignados a una tarea
    /// </summary>
    /// <param name="idTarea">Identificador tarea</param>
    /// <returns></returns>
    public IEnumerable<ASEventoTareaAsignada> ObtenerHistoricoTareaAsignada(long idTarea)
    {
      return ASRepositorio.Instancia.ObtenerHistoricoTareaAsignada(idTarea);
    }

    /// <summary>
    /// Obtiene un archivo adjunto por su id
    /// </summary>
    /// <param name="idArchivo"></param>
    /// <returns></returns>
    public string ObtenerArchivoAdjunto(long idArchivo)
    {
      return ASRepositorio.Instancia.ObtenerArchivoAdjunto(idArchivo);
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferior(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCargo)
    {
      return ASRepositorio.Instancia.ObtenerUsuariosConCargoInferior(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCargo);
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferior(int idCargo, string idUsuario)
    {
      return ASRepositorio.Instancia.ObtenerUsuariosConCargoInferior(idCargo, idUsuario);
    }

    /// <summary>
    /// Gestiona la tarea con base en la tarea asignada
    /// </summary>
    /// <param name="tareaAsignada">Tarea asignada</param>
    /// <param name="usuario">Usuario que realiza la operación</param>
    public void GestionarTarea(ASTareaAsignada tareaAsignada)
    {
      ASRepositorio.Instancia.GestionarTarea(tareaAsignada);
    }
  }
}