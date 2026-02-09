using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Area;

namespace CO.Servidor.Dominio.Comun.Area
{
  /// <summary>
  /// Fachada para la publicación de lógica de Areas para otros módulos
  /// </summary>
  public interface IARFachadaAreas
  {
    /// <summary>
    /// Obtener los datos de la empresa (Interrapidisimo)
    /// </summary>
    /// <returns>Datos de la empresa</returns>
    AREmpresaDC ObtenerDatosEmpresa();

    /// <summary>
    /// Método que retorna la información de una casa matriz a partir de su id
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns></returns>
    ARCasaMatrizDC ObtenerCasaMatriz(int idCasaMatriz);

    /// <summary>
    /// Obtener la casa matriz de un proceso
    /// </summary>
    /// <param name="idProceso">Id del proceso</param>
    /// <returns>Objeto casa matriz</returns>
    ARCasaMatrizDC ObtenerCasaMatrizProceso(long idProceso);

    /// <summary>
    /// Retorna todas las casas matriz activas
    /// </summary>
    /// <returns>Colección con todas las casas matrices</returns>
    IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz();
  }
}