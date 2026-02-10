using CO.Servidor.Servicios.ContratoDatos.Area;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IARAreasSvc
  {
    /// <summary>
    /// Retorna todas las casas matriz activas
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARCasaMatrizDC> ObtenerCasaTodosMatriz();

    /// <summary>
    /// Método para obtener todas las gestiones
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARGestionDC> ObtenerGestiones();

    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    ARCasaMatrizCuentaBancoDC ObtenerCuentaBancoCasaMatriz(short idCasaMatriz);

    /// <summary>
    /// Método para obtener todas las gestiones de todas las casas matrices
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<ARGestionDC> ObtenerGestionesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

    /// <summary>
    /// Obtiene los macroprocesos de una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<ARMacroprocesoDC> ObtenerMacroprocesoCasaMatriz(int idCasaMatriz);

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARGestionDC> ObtenerGestionesMacroproceso(string idMacroproceso);

    /// <summary>
    /// Retorna los procesos de una gestion
    /// </summary>
    /// <param name="idGestion">Id de la gestión</param>
    /// <returns>Lista de procesos</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARProcesoDC> ObtenerProcesosGestion(long idGestion);

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<ARProcesoDC> ObtenerProcesos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

    /// <summary>
    /// Guardar la información de la casa matriz, esto incluye sus macroprocesos, gestiones y procesos
    /// </summary>
    /// <param name="casaMatrizInfo">Objeto con la información de la casa matriz</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GuardarCasaMatriz(ARCasaMatrizConTodo casaMatrizInfo);

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARGestionDC> ObtenerTodasGestionesMacroproceso();

    /// <summary>
    /// Retorna todos los procesos
    /// </summary>
    /// <returns>Lista de procesos</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARProcesoDC> ObtenerTodosProcesos();

    /// <summary>
    /// Obtiene los macroprocesos
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<ARMacroprocesoDC> ObtenerTodosMacroprocesos();

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<ARProcesoDC> ObtenerProcesosCasaMatriz(int casaMatriz);
  }
}