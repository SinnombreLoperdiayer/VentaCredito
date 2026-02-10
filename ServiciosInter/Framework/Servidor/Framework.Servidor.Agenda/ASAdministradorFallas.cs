using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Framework.Servidor.Agenda.Datos;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace Framework.Servidor.Agenda
{
  public class ASAdministradorFallas : System.MarshalByRefObject
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly ASAdministradorFallas instancia = (ASAdministradorFallas)FabricaInterceptores.GetProxy(new ASAdministradorFallas(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_AGENDA);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static ASAdministradorFallas Instancia
    {
      get { return ASAdministradorFallas.instancia; }
    }

    #endregion Instancia Singleton

    #region Métodos

    /// <summary>
    /// Consulta las Fallas asociadas a un módulo, que tengan actividades asociadas y en estado activo
    /// </summary>
    /// <param name="filtro">Filtro para aplicar a la consulta</param>
    /// <param name="totalRegistros">Total de registros que retorna la consulta</param>
    /// <param name="campoOrdenamiento">Campo por el cual se va a realizar el ordenamiento</param>
    /// <param name="indicePagina">Índie de página</param>
    /// <param name="registrosPorPagina">Cantidad de registros por página</param>
    /// <param name="esAscendente">Indica si el campo es ascendente</param>
    /// <returns>Lista de fallas</returns>
    public IEnumerable<ASFalla> ObtenerFallas(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return ASOperacionesFallas.Instancia.ObtenerFallas(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
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
      int totalRegistros = 0;
      return ASOperacionesTareas.Instancia.ObtenerTareasAsociadasFalla(idFalla, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente, out totalRegistros);
    }

    /// <summary>
    /// Retorna la lista de cargos disponibles
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ASCargo> ObtenerCargos()
    {
      return ASRepositorio.Instancia.ObtenerCargos();
    }

    /// <summary>
    /// Ingresa una nueva falla
    /// </summary>
    /// <param name="falla">Falla</param>
    public int AdicionarFalla(ASFalla falla)
    {
      int idfalla = new int();
      if (falla.EstadoRegistro == Comun.EnumEstadoRegistro.ADICIONADO)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          idfalla = ASOperacionesFallas.Instancia.AdicionarFalla(falla);
          if (falla.Tareas != null)
          {
            falla.Tareas.ToList().ForEach(t =>
              {
                t.IdFalla = idfalla;
                GuardarCambiosTarea(t);
              });
          }
          scope.Complete();
        }
      }
      return idfalla;
    }

    /// <summary>
    /// Realiza los cambios en las fallas
    /// </summary>
    /// <param name="falla">Falla</param>
    public void GuardarCambiosFalla(ASFalla falla)
    {
      if (falla.EstadoRegistro == Comun.EnumEstadoRegistro.MODIFICADO)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          ASOperacionesFallas.Instancia.EditarFalla(falla);
          scope.Complete();
        }
      }
      if (falla.EstadoRegistro == Comun.EnumEstadoRegistro.BORRADO)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          ASOperacionesFallas.Instancia.EliminarFalla(falla);
          scope.Complete();
        }
      }
    }

    /// <summary>
    /// Realiza los cambios en las tareas
    /// </summary>
    /// <param name="tarea">tarea</param>
    public void GuardarCambiosTarea(ASTarea tarea)
    {
      if (tarea.EstadoRegistro == Comun.EnumEstadoRegistro.ADICIONADO)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          ASRepositorio.Instancia.AdicionarTarea(tarea);
          scope.Complete();
        }
      }
      if (tarea.EstadoRegistro == Comun.EnumEstadoRegistro.MODIFICADO)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          ASRepositorio.Instancia.EditarTarea(tarea);
          scope.Complete();
        }
      }
      if (tarea.EstadoRegistro == Comun.EnumEstadoRegistro.BORRADO)
      {
        using (TransactionScope scope = new TransactionScope())
        {
          ASRepositorio.Instancia.EliminarTarea(tarea);
          scope.Complete();
        }
      }
    }

    /// <summary>
    /// Gestiona la tarea con base en la tarea asignada
    /// </summary>
    /// <param name="tareaAsignada">Tarea asignada</param>
    /// <param name="usuario">Usuario que realiza la operación</param>
    public void GestionarTarea(ASTareaAsignada tareaAsignada)
    {
      ASOperacionesTareas.Instancia.GestionarTarea(tareaAsignada);
    }

    /// <summary>
    /// Obtiene estados
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ASEstado> ObtenerEstados()
    {
      return ASRepositorio.Instancia.ObtenerEstados();
    }

    /// <summary>
    /// Consulta la lista de módulos del sistema
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VEModulo> ObtenerModulos()
    {
      return ASRepositorio.Instancia.ObtenerModulos();
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferior(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCargo)
    {
      return ASOperacionesTareas.Instancia.ObtenerUsuariosConCargoInferior(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCargo);
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferior(int idCargo, string idUsuario)
    {
      return ASOperacionesTareas.Instancia.ObtenerUsuariosConCargoInferior(idCargo, idUsuario);
    }

    /// <summary>
    /// Obtiene al lista de tareas en un estado
    /// </summary>
    /// <param name="usuario">Usuario que solicita la lista de tareas</param>
    /// <returns></returns>
    public IEnumerable<ASTareaAsignada> ObtenerTareasAsignadas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string usuario)
    {
      return ASOperacionesTareas.Instancia.ObtenerTareasAsignadas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, usuario);
    }

    /// <summary>
    /// Retorna la lista de histórico asignado de eventos asignados a una tarea
    /// </summary>
    /// <param name="idTarea">Identificador tarea</param>
    /// <returns></returns>
    public IEnumerable<ASEventoTareaAsignada> ObtenerHistoricoTareaAsignada(long idTarea)
    {
      return ASOperacionesTareas.Instancia.ObtenerHistoricoTareaAsignada(idTarea);
    }

    /// <summary>
    /// Obtiene un archivo adjunto por su id
    /// </summary>
    /// <param name="idArchivo"></param>
    /// <returns></returns>
    public string ObtenerArchivoAdjunto(long idArchivo)
    {
      return ASOperacionesTareas.Instancia.ObtenerArchivoAdjunto(idArchivo);
    }

    #endregion Métodos
  }
}