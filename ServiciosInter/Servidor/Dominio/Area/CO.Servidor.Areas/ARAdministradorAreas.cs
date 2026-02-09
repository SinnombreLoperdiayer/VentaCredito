using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Area;

namespace CO.Servidor.Areas
{
  /// <summary>
  /// Clase administradora para areas
  /// </summary>
  public class ARAdministradorAreas
  {
    private static readonly ARAdministradorAreas instancia = new ARAdministradorAreas();

    /// <summary>
    /// Retorna la instancia de la clase TARepositorio
    /// </summary>
    public static ARAdministradorAreas Instancia
    {
      get { return ARAdministradorAreas.instancia; }
    }

    private ARAdministradorAreas()
    {
    }

    #region Consultas

    /// <summary>
    /// Retorna todas las casas matriz activas
    /// </summary>
    /// <returns></returns>
    public IList<ARCasaMatrizDC> ObtenerCasaTodosMatriz()
    {
      return ARAreas.Instancia.ObtenerCasaTodosMatriz();
    }

    /// <summary>
    /// Método para obtener todas las gestiones
    /// </summary>
    /// <returns></returns>
    public IList<ARGestionDC> ObtenerGestiones()
    {
      return ARAreas.Instancia.ObtenerGestiones();
    }

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerTodasGestionesMacroproceso()
    {
      return ARAreas.Instancia.ObtenerTodasGestionesMacroproceso();
    }

    /// <summary>
    /// Retorna todos los procesos
    /// </summary>
    /// <returns>Lista de procesos</returns>
    public IList<ARProcesoDC> ObtenerTodosProcesos()
    {
      return ARAreas.Instancia.ObtenerTodosProcesos();
    }

    /// <summary>
    /// Obtiene los macroprocesos
    /// </summary>
    /// <returns></returns>
    public List<ARMacroprocesoDC> ObtenerTodosMacroprocesos()
    {
      return ARAreas.Instancia.ObtenerTodosMacroprocesos();
    }

    /// <summary>
    /// Obtener los bancos configurados para una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz">Identificador de la casa matriz</param>
    /// <returns>Información de la casa matriz y sus bancos</returns>
    public ARCasaMatrizCuentaBancoDC ObtenerCuentaBancoCasaMatriz(short idCasaMatriz)
    {
      return ARAreas.Instancia.ObtenerCuentaBancoCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Método para obtener todas las gestiones de todas las casas matrices
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerGestiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return ARAreas.Instancia.ObtenerGestiones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene los macroprocesos de una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    public List<ARMacroprocesoDC> ObtenerMacroprocesoCasaMatriz(int idCasaMatriz)
    {
      return ARAreas.Instancia.ObtenerMacroprocesoCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerGestionesMacroproceso(string idMacroproceso)
    {
      return ARAreas.Instancia.ObtenerGestionesMacroproceso(idMacroproceso);
    }

    /// <summary>
    /// Retorna los procesos de una gestion
    /// </summary>
    /// <param name="idGestion">Id de la gestión</param>
    /// <returns>Lista de procesos</returns>
    public IList<ARProcesoDC> ObtenerProcesosGestion(long idGestion)
    {
      return ARAreas.Instancia.ObtenerProcesosGestion(idGestion);
    }

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    public IList<ARProcesoDC> ObtenerProcesos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return ARAreas.Instancia.ObtenerProcesos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtener la casa matriz de un proceso
    /// </summary>
    /// <param name="idProceso">Id del proceso</param>
    /// <returns>Objeto casa matriz</returns>
    public ARCasaMatrizDC ObtenerCasaMatrizProceso(long idProceso)
    {
      return ARAreas.Instancia.ObtenerCasaMatrizProceso(idProceso);
    }

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    public IList<ARProcesoDC> ObtenerProcesosCasaMatriz(int casaMatriz)
    {
      return ARAreas.Instancia.ObtenerProcesosCasaMatriz(casaMatriz);
    }

    #endregion Consultas

    #region Configuración Casa Matriz

    /// <summary>
    /// Guardar la información de la casa matriz, esto incluye sus macroprocesos, gestiones y procesos
    /// </summary>
    /// <param name="casaMatrizInfo">Objeto con la información de la casa matriz</param>
    public void GuardarCasaMatriz(ARCasaMatrizConTodo casaMatrizInfo)
    {
      ARAreas.Instancia.GuardarCasaMatriz(casaMatrizInfo);
    }

    #endregion Configuración Casa Matriz
  }
}