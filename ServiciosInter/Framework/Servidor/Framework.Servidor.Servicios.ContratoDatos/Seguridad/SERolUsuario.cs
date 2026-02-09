using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SERolUsuario : DataContractBase
  {
    [DataMember]
    public string IdUsuario { get; set; }

    [DataMember]
    public List<SERol> RolesAsignados { get; set; }

    [DataMember]
    public List<SERol> RolesSinAsignar { get; set; }

    [DataMember]
    public List<SERol> Roles { get; set; }
  }
}