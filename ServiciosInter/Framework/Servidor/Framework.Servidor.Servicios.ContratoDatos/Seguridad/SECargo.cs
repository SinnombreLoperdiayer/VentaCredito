using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SECargo : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("CAR_IdCargo")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cargo")]
    public int IdCargo { get; set; }

    [DataMember]
    [CamposOrdenamiento("CAR_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Filtrable("CAR_Descripcion", "Descripcion:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion")]
    public string DescripcionCargo { get; set; }

    [DataMember]
    [CamposOrdenamiento("CAR_IdCargoReporta")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCargoReporta")]
    public int? CarIdCargoReporta { get; set; }

    [DataMember]
    [CamposOrdenamiento("CAR_DescripcionReporta")]
    [Filtrable("CAR_DescripcionReporta", "Jefe:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DesCargoReporta")]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string DescripcionReporta { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    public List<SECargo> Subordinados { get; set; }
  }
}