using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de las areas de la empresa
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAGestionDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
   // [Display(ResourceType = typeof(Etiquetas), Name = "AreaInterna", Description = "TooltipAreaInterna")]
    public long IdGestion { get; set; }

    [DataMember]
 //   [Display(ResourceType = typeof(Etiquetas), Name = "AreaInterna", Description = "TooltipAreaInterna")]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string DescripcionGestion { get; set; }
  }
}