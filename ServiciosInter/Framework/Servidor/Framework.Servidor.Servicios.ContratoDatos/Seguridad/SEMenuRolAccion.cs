using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEMenuRolAccion : SEMenuRolAccionConsolidado
  {
    // Recibir los menus asignados a un Rol
    [DataMember]
    public List<SEMenuAccion> MenusAsignados { get; set; }
  }
}