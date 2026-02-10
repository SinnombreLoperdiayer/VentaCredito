using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUParametrosDC
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    [Filtrable("PEI_Identificacion", "Identificacion: ", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Valor { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdParametro")]
    public string IdParametro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion")]
    public string Descripcion { get; set; }
  }
}