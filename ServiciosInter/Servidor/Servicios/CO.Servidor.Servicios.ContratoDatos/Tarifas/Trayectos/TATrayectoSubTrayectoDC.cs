using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la informacion de TrayectoSubTrayecto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATrayectoSubTrayectoDC : DataContractBase
  {
    [DataMember]
    public int IdTrayectoSubTrayecto { get; set; }

    [DataMember]
    public string IdTipoTrayecto { get; set; }

    [DataMember]
    public string DescripcionTrayecto { get; set; }

    [DataMember]
    public string IdTipoSubTrayecto { get; set; }

    [DataMember]
    public string DescripcionTipoSubTrayecto { get; set; }
  }
}