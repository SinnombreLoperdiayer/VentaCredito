using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUEstadosMensajeroDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdEstado { get; set; }

    [DataMember]
    [CamposOrdenamiento("MEN_Estado")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
    public string Descripcion { get; set; }
  }
}