using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Destinatario frecuente de un Cliente contado
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLDestinatarioFrecuenteDC : DataContractBase
  {
    /// <summary>
    /// Informacion completa del Cliente Contado
    /// </summary>
    [DataMember]
    public CLClienteContadoDC ClienteContado { get; set; }

    /// <summary>
    /// Identificación último centro de servicio a donde se le envió un giro a este destinatario frecuente
    /// </summary>
    [DataMember]
    public long IdUltimoCentroServDestino { get; set; }
  }
}