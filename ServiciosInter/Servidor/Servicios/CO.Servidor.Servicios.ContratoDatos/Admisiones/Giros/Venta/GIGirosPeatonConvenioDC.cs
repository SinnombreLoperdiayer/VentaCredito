using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros
{
  /// <summary>
  /// Clase que contiene la informacion cuando el giro de de un peaton a un convenio o
  /// de un convenio a un peaton
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIGirosPeatonConvenioDC : DataContractBase
  {
    /// <summary>
    /// Cliente Convenio (persona juridica)
    /// </summary>
    [DataMember]
    public CLClientesDC ClienteConvenio { get; set; }

    /// <summary>
    /// Cliente Contado (persona natural)
    /// </summary>
    [DataMember]
    public CLClienteContadoDC ClienteContado { get; set; }
  }
}