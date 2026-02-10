using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class GITipoGiroDC : DataContractBase
  {
    [DataMember]
    public string IdTipoGiro { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}