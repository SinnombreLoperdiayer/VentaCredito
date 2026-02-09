using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros
{
  /// <summary>
  /// Clase que contiene la información del tipo de remitente y destinatario
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GITipoRemitenteDestinatarioDC : DataContractBase
  {
    [DataMember]
    public string Identificador { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}