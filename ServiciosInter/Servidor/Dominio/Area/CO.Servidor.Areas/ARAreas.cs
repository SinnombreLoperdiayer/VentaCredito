using System.Collections.Generic;
using CO.Servidor.Areas.Datos;
using CO.Servidor.Servicios.ContratoDatos.Area;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Areas
{
  /// <summary>
  /// Consultas sobre las areas
  /// </summary>
  internal class ARAreas : ControllerBase
  {
    #region Campos

    private static readonly ARAreas instancia = (ARAreas)FabricaInterceptores.GetProxy(new ARAreas(), COConstantesModulos.MODULO_AREAS);

    #endregion Campos

    #region ctor

    /// <summary>
    /// Retorna la instancia de la clase TARepositorio
    /// </summary>
    public static ARAreas Instancia
    {
      get { return ARAreas.instancia; }
    }

    private ARAreas()
    {
    }

    #endregion ctor

    #region Consultas

    /// <summary>
    /// Retorna todas las casas matriz activas
    /// </summary>
    /// <returns></returns>
    public IList<ARCasaMatrizDC> ObtenerCasaTodosMatriz()
    {
      return ARRepositorio.Instancia.ObtenerTodasLasCasaMatriz();
    }

    /// <summary>
    /// Método para obtener todas las gestiones
    /// </summary>
    /// <returns></returns>
    public IList<ARGestionDC> ObtenerGestiones()
    {
      return ARRepositorio.Instancia.ObtenerGestiones();
    }

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerTodasGestionesMacroproceso()
    {
      return ARRepositorio.Instancia.ObtenerTodasGestionesMacroproceso();
    }

    /// <summary>
    /// Obtiene los macroprocesos
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    public List<ARMacroprocesoDC> ObtenerTodosMacroprocesos()
    {
      return ARRepositorio.Instancia.ObtenerTodosMacroprocesos();
    }

    /// <summary>
    /// Retorna todos los procesos
    /// </summary>
    /// <returns>Lista de procesos</returns>
    public IList<ARProcesoDC> ObtenerTodosProcesos()
    {
      return ARRepositorio.Instancia.ObtenerTodosProcesos();
    }

    /// <summary>
    /// Método que retorna la información de una casa matriz a partir de su id
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    public ARCasaMatrizDC ObtenerCasaMatriz(int idCasaMatriz)
    {
      return ARRepositorio.Instancia.ObtenerCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Obtener los bancos configurados para una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz">Identificador de la casa matriz</param>
    /// <returns>Información de la casa matriz y sus bancos</returns>
    public ARCasaMatrizCuentaBancoDC ObtenerCuentaBancoCasaMatriz(short idCasaMatriz)
    {
      return ARRepositorio.Instancia.ObtenerCuentaBancoCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Obtener los datos de la empresa (Interrapidisimo)
    /// </summary>
    /// <returns>Datos de la empresa</returns>
    public AREmpresaDC ObtenerDatosEmpresa()
    {
      return ARRepositorio.Instancia.ObtenerDatosEmpresa();
    }

    /// <summary>
    /// Método para obtener todas las gestiones de todas las casas matrices
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerGestiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return ARRepositorio.Instancia.ObtenerGestiones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene los macroprocesos de una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    public List<ARMacroprocesoDC> ObtenerMacroprocesoCasaMatriz(int idCasaMatriz)
    {
      return ARRepositorio.Instancia.ObtenerMacroprocesoCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerGestionesMacroproceso(string idMacroproceso)
    {
      return ARRepositorio.Instancia.ObtenerGestionesMacroproceso(idMacroproceso);
    }

    /// <summary>
    /// Retorna los procesos de una gestion
    /// </summary>
    /// <param name="idGestion">Id de la gestión</param>
    /// <returns>Lista de procesos</returns>
    public IList<ARProcesoDC> ObtenerProcesosGestion(long idGestion)
    {
      return ARRepositorio.Instancia.ObtenerProcesosGestion(idGestion);
    }

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    public IList<ARProcesoDC> ObtenerProcesos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return ARRepositorio.Instancia.ObtenerProcesos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtener la casa matriz de un proceso
    /// </summary>
    /// <param name="idProceso">Id del proceso</param>
    /// <returns>Objeto casa matriz</returns>
    public ARCasaMatrizDC ObtenerCasaMatrizProceso(long idProceso)
    {
      return ARRepositorio.Instancia.ObtenerCasaMatrizProceso(idProceso);
    }

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    public IList<ARProcesoDC> ObtenerProcesosCasaMatriz(int casaMatriz)
    {
      return ARRepositorio.Instancia.ObtenerProcesosCasaMatriz(casaMatriz);
    }

    #endregion Consultas

    #region Configurar Casa Matriz

    /// <summary>
    /// Guardar la información de la casa matriz, esto incluye sus macroprocesos, gestiones y procesos
    /// </summary>
    /// <param name="casaMatrizInfo">Objeto con la información de la casa matriz</param>
    public void GuardarCasaMatriz(ARCasaMatrizConTodo casaMatrizInfo)
    {
      ARRepositorio.Instancia.Guardar(casaMatrizInfo);
    }

    #endregion Configurar Casa Matriz
  }
}