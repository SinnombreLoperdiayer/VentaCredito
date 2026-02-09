using System.Collections.Generic;
using System.Transactions;
using Framework.Servidor.Agenda.Datos;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace Framework.Servidor.Agenda
{
  /// <summary>
  /// Contiene las operaciones relacionas con las Fallas
  /// </summary>
  public class ASOperacionesFallas : System.MarshalByRefObject
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly ASOperacionesFallas instancia = (ASOperacionesFallas)FabricaInterceptores.GetProxy(new ASOperacionesFallas(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_AGENDA);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static ASOperacionesFallas Instancia
    {
      get { return ASOperacionesFallas.instancia; }
    }

    #endregion Instancia Singleton

    /// <summary>
    /// Consulta una falla
    /// </summary>
    /// <param name="idFalla"></param>
    public ASFalla ConsultarFalla(int idFalla)
    {
      return ASRepositorio.Instancia.ObtenerFalla(idFalla);
    }

    /// <summary>
    /// Consulta las Fallas asociadas a un módulo, que tengan actividades asociadas y en estado activo
    /// </summary>
    /// <param name="filtro">Filtro a aplicar sobre la consulta</param>
    /// <param name="totalRegistros">Total de registros que retorna la consulta</param>
    /// <param name="campoOrdenamiento">Campo por el cual se va a realizar el ordenamiento</param>
    /// <param name="indicePagina">Índie de página</param>
    /// <param name="registrosPorPagina">Cantidad de registros por página</param>
    /// <param name="esAscendente">Indica si el campo es ascendente</param>
    /// <returns>Lista de fallas</returns>
    public IEnumerable<ASFalla> ObtenerFallas(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      return ASRepositorio.Instancia.ObtenerFallas(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
    }

    /// <summary>
    /// Registra falla en el sistema
    /// </summary>
    /// <param name="falla">Falla a registrar</param>
    /// <param name="usuario">Usuario que hace la operación</param>
    public int AdicionarFalla(ASFalla falla)
    {
      return ASRepositorio.Instancia.AdicionarFalla(falla);
    }

    /// <summary>
    /// Registra falla en el sistema
    /// </summary>
    /// <param name="falla">Falla a registrar</param>
    /// <param name="usuario">Usuario que hace la operación</param>
    public void EditarFalla(ASFalla falla)
    {
      ASRepositorio.Instancia.EditarFalla(falla);
    }

    /// <summary>
    /// Elimina falla, cambia estado de activo a inactivo en la falla
    /// </summary>
    /// <param name="falla"></param>
    public void EliminarFalla(ASFalla falla)
    {
      ASRepositorio.Instancia.EliminarFalla(falla);
    }
  }
}