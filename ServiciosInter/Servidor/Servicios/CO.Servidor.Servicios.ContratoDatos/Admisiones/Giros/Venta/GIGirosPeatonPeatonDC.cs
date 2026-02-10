using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros
{
  /// <summary>
  /// Clase que contiene la informacion de la tbl Giros Peaton Peaton
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIGirosPeatonPeatonDC : DataContractBase
  {
    [DataMember]
    public CLClienteContadoDC ClienteRemitente { get; set; }

    [DataMember]
    public CLClienteContadoDC ClienteDestinatario { get; set; }
  }
}