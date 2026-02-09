using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas.Transacciones
{
  /// <summary>
  /// Contrato de datos con la respuesta despues de registrar una transacción en caja
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAResultadoRegistroTransaccionDC:  DataContractBase
  {
    [DataMember]
    public long IdTransaccion { get; set; }

    [DataMember]
    public string NumeroComprobante { get; set; }
  }
}
