using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de los servicios por factura
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLOrigenTipoDC : DataContractBase
  {
    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public bool Tipo { get; set; }
  }
}