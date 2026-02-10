using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Clase que contiene la informacion a ser preimpresa para los Suministros Manuales
  /// de un Centro de Servicio
  /// </summary>  
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUImpresionSumCentroServicioDC : SUImpresionSumPadreDC
  {
    /// <summary>
    /// Informacion del centro de servicios aprovisionado
    /// </summary>
    [DataMember]
    public PUCentroServiciosDC CentroServicio { get; set; }

    /// <summary>
    /// Rango aprovisionado del centro de servicios
    /// </summary>
    [DataMember]
     public SURango Rango { get; set; }
  }
}
