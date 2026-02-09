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
  public class SERolMenu : DataContractBase
  {
    [DataMember]
    public string IdRol { get; set; }

    [DataMember]
    public List<SEMenu> MenuAsignado { get; set; }

    [DataMember]
    public List<SEMenu> MenuSinAsignar { get; set; }

    [DataMember]
    public List<SEMenu> Menus { get; set; }

    [DataMember]
    public int IdRolMenu { get; set; }

    [DataMember]
    public List<SEAccion> AccionesAsignadas { get; set; }

    [DataMember]
    public List<SEAccion> AccionesSinAsignar { get; set; }

    [DataMember]
    public List<SEAccion> Acciones { get; set; }
  }
}