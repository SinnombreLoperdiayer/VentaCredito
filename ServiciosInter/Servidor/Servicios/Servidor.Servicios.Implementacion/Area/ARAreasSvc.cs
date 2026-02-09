using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using CO.Servidor.Areas;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.Implementacion.Area
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ARAreasSvc : IARAreasSvc
  {
    public ARAreasSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
    }

    #region consultas

    public IList<ARCasaMatrizDC> ObtenerCasaTodosMatriz()
    {
      return ARAdministradorAreas.Instancia.ObtenerCasaTodosMatriz();
    }

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerTodasGestionesMacroproceso()
    {
      return ARAdministradorAreas.Instancia.ObtenerTodasGestionesMacroproceso();
    }

    /// <summary>
    /// Retorna todos los procesos
    /// </summary>
    /// <returns>Lista de procesos</returns>
    public IList<ARProcesoDC> ObtenerTodosProcesos()
    {
      return ARAdministradorAreas.Instancia.ObtenerTodosProcesos();
    }

    /// <summary>
    /// Obtiene los macroprocesos
    /// </summary>
    /// <returns></returns>
    public List<ARMacroprocesoDC> ObtenerTodosMacroprocesos()
    {
      return ARAdministradorAreas.Instancia.ObtenerTodosMacroprocesos();
    }

    /// <summary>
    /// Método para obtener todas las gestiones
    /// </summary>
    /// <returns></returns>
    public IList<ARGestionDC> ObtenerGestiones()
    {
      return ARAdministradorAreas.Instancia.ObtenerGestiones();
    }

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    public IList<ARProcesoDC> ObtenerProcesosCasaMatriz(int casaMatriz)
    {
      return ARAdministradorAreas.Instancia.ObtenerProcesosCasaMatriz(casaMatriz);
    }

    /// <summary>
    /// Obtener los bancos configurados para una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz">Identificador de la casa matriz</param>
    /// <returns>Información de la casa matriz y sus bancos</returns>
    public ARCasaMatrizCuentaBancoDC ObtenerCuentaBancoCasaMatriz(short idCasaMatriz)
    {
      return ARAdministradorAreas.Instancia.ObtenerCuentaBancoCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Método para obtener todas las gestiones de todas las casas matrices
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public GenericoConsultasFramework<ARGestionDC> ObtenerGestionesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ARGestionDC>()
      {
        Lista = ARAdministradorAreas.Instancia.ObtenerGestiones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Obtiene los macroprocesos de una casa matriz
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    public List<ARMacroprocesoDC> ObtenerMacroprocesoCasaMatriz(int idCasaMatriz)
    {
      return ARAdministradorAreas.Instancia.ObtenerMacroprocesoCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Método para obtener las gestiones por macroproceso
    /// </summary>
    /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
    public IList<ARGestionDC> ObtenerGestionesMacroproceso(string idMacroproceso)
    {
      return ARAdministradorAreas.Instancia.ObtenerGestionesMacroproceso(idMacroproceso);
    }

    /// <summary>
    /// Retorna los procesos de una gestion
    /// </summary>
    /// <param name="idGestion">Id de la gestión</param>
    /// <returns>Lista de procesos</returns>
    public IList<ARProcesoDC> ObtenerProcesosGestion(long idGestion)
    {
      return ARAdministradorAreas.Instancia.ObtenerProcesosGestion(idGestion);
    }

    /// <summary>
    /// Método para obtener todos los procesos de todas las casas matrices
    /// </summary>
    /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
    public GenericoConsultasFramework<ARProcesoDC> ObtenerProcesos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ARProcesoDC>()
      {
        Lista = ARAdministradorAreas.Instancia.ObtenerProcesos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
        TotalRegistros = totalRegistros
      };
    }

    #endregion consultas

    /// <summary>
    /// Guardar la información de la casa matriz, esto incluye sus macroprocesos, gestiones y procesos
    /// </summary>
    /// <param name="casaMatrizInfo">Objeto con la información de la casa matriz</param>
    public void GuardarCasaMatriz(ARCasaMatrizConTodo casaMatrizInfo)
    {
      ARAdministradorAreas.Instancia.GuardarCasaMatriz(casaMatrizInfo);
    }
  }
}