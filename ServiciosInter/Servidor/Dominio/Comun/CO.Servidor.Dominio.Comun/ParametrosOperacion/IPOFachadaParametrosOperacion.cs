using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;

namespace CO.Servidor.Dominio.Comun.ParametrosOperacion
{
  public interface IPOFachadaParametrosOperacion
  {
    /// <summary>
    /// Verifica que los vehiculos asociados a un mensajero tengan vencido el soat y la revision tecnomencánica
    /// </summary>
    /// <param name="idMensajero">Id del mensajero</param>
    /// <returns></returns>
    bool VerificaMensajeroSoatTecnoMecanica(long idMensajero);

    /// <summary>
    /// Obtiene información básica del mensajero dado su identificador
    /// </summary>
    /// <param name="idMensajero"></param>
    /// <returns></returns>
    POMensajero ObtenerMensajero(long idMensajero);

    /// <summary>
    /// <summary>
    /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
    /// </summary>
    /// <param name="idVehiculo"></param>
    /// <returns></returns>
    IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo);

    /// <summary>
    /// Obtiene todos los conductores Activos
    /// </summary>
    IList<POConductores> ObtenerTodosConductores();

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
    IList<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

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
    IList<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

    /// <summary>
    /// Obteners el primer conductor de un vehiculo por placa
    /// </summary>
    /// <param name="placa">placa del vehiculo a consultar</param>
    ONRutaConductorDC ObtenerConductoresPorVehiculo(string placa);

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
    IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                          int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

    /// <summary>
    /// Obtene los datos del mensajero de la agencia.
    /// </summary>
    /// <param name="idAgencia">Es el id agencia.</param>
    /// <returns>la lista de mensajeros de una agencia</returns>
    IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia);
  }
}