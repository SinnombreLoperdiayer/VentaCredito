using System.Collections.Generic;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;

namespace CO.Servidor.ParametrosOperacion
{
  public class POFachadaParametrosOperacion : IPOFachadaParametrosOperacion
  {
    /// <summary>
    /// Instancia Singleton
    /// </summary>
    private static readonly POFachadaParametrosOperacion instancia = new POFachadaParametrosOperacion();

    #region Propiedades

    /// <summary>
    /// Retorna una instancia de la fabrica de Dominio
    /// </summary>
    public static POFachadaParametrosOperacion Instancia
    {
      get { return POFachadaParametrosOperacion.instancia; }
    }

    #endregion Propiedades

    public bool VerificaMensajeroSoatTecnoMecanica(long idMensajero)
    {
      return POParametrosOperacion.Instancia.VerificaMensajeroSoatTecnoMecanica(idMensajero);
    }

    /// Obtiene información básica del mensajero dado su identificador
    /// </summary>
    /// <param name="idMensajero"></param>
    /// <returns></returns>
    public POMensajero ObtenerMensajero(long idMensajero)
    {
      return POParametrosOperacion.Instancia.ObtenerMensajero(idMensajero);
    }

    /// <summary>
    /// <summary>
    /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <returns></returns>
    public IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo)
    {
      return POAdministradorParametrosOperacion.Instancia.ObtenerConductoresActivosVehiculos(idVehiculo);
    }

    /// <summary>
    /// Obtiene todos los conductores Activos
    /// </summary>
    public IList<POConductores> ObtenerTodosConductores()
    {
      return POAdministradorParametrosOperacion.Instancia.ObtenerTodosConductores();
    }

    /// <summary>
    /// Obtiene  los vehiculos
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <returns>Lista con los vehiculos</returns>
    public IList<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return POAdministradorParametrosOperacion.Instancia.ObtenerVehiculos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene  los conductores
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <returns>Lista con los conductores</returns>
    public IList<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return POAdministradorParametrosOperacion.Instancia.ObtenerConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obteners el primer conductor de un vehiculo por placa
    /// </summary>
    /// <param name="placa">placa del vehiculo a consultar</param>
    public ONRutaConductorDC ObtenerConductoresPorVehiculo(string placa)
    {
      return POParametrosOperacion.Instancia.ObtenerConductoresPorVehiculo(placa);
    }

   


    /// <summary>
    /// Obtiene los mensajeros y conductores configurados
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                          int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return POParametrosOperacion.Instancia.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtene los datos del mensajero de la agencia.
    /// </summary>
    /// <param name="idAgencia">Es el id agencia.</param>
    /// <returns>la lista de mensajeros de una agencia</returns>
    public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
    {
      return POParametrosOperacion.Instancia.ObtenerMensajerosAgencia(idAgencia);
    }
  }
}