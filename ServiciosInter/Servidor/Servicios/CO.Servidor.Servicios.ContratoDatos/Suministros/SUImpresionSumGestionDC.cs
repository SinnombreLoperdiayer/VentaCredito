using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    /// <summary>
  /// Clase que contiene la informacion a ser preimpresa para los Suministros de una Gestion
  /// </summary>  
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUImpresionSumGestionDC : SUImpresionSumPadreDC
  {

    /// <summary>
    /// Informacion de la casa Matriz
    /// </summary>
    [DataMember]
    public ARCasaMatrizDC CasaMatriz { get; set; }

    /// <summary>
    /// Rango aprovisionado a la gestion
    /// </summary>
    [DataMember]
    public SURango Rango { get; set; }    
  }
}
