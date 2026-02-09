using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Areas.Datos;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Servicios.ContratoDatos.Area;

namespace CO.Servidor.Areas
{
  /// <summary>
  /// Fachada para la publicación de lógica de Areas para otros módulos
  /// </summary>
  public class ARFachadaAreas : IARFachadaAreas
  {
    /// <summary>
    /// Método que retorna la información de una casa matriz a partir de su id
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    public ARCasaMatrizDC ObtenerCasaMatriz(int idCasaMatriz)
    {
      return ARAreas.Instancia.ObtenerCasaMatriz(idCasaMatriz);
    }

    /// <summary>
    /// Obtener los datos de la empresa (Interrapidisimo)
    /// </summary>
    /// <returns>Datos de la empresa</returns>
    public AREmpresaDC ObtenerDatosEmpresa()
    {
      return ARAreas.Instancia.ObtenerDatosEmpresa();
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
    /// Retorna todas las casas matriz activas
    /// </summary>
    /// <returns>Colección con todas las casas matrices</returns>
    public IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz()
    {
      return ARAreas.Instancia.ObtenerCasaTodosMatriz();
    }
  }
}