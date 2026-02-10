using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Clase que contiene la informacion a ser preimpresa para los Suministros de un Mensajero 
  /// </summary>  
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUImpresionSumMensajeroDC : SUImpresionSumPadreDC
  {
    /// <summary>
    /// Rango aprovisionado del centro de servicios
    /// </summary>
    [DataMember]
    public SURango Rango { get; set; }

    /// <summary>
    /// Informacion del mensajero a Imprimir
    /// </summary>
    [DataMember]
    public PAPersonaInternaDC InformacionMensajero { get; set; }

    /// <summary>
    /// Informacion del centro de servicios de un Mensajero
    /// </summary>
    [DataMember]
    public PUCentroServiciosDC CentroServicio { get; set; }
  }
}
