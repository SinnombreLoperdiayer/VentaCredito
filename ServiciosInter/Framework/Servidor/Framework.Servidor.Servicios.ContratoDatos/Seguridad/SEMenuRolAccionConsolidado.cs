using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEMenuRolAccionConsolidado : DataContractBase
  {
    [DataMember]
    public string IdRol { get; set; }

    [DataMember]
    public int IdMenuRol { get; set; }

    //Recibir los menus disponibles y los cambios
    [DataMember]
    public List<SEMenuAccion> Menus { get; set; }
  }
}