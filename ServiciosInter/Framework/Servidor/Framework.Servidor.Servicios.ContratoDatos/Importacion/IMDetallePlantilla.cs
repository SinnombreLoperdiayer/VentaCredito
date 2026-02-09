using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Importacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class IMDetallePlantilla
  {
    [DataMember]
    [Required(AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCampo", Description = "TooltipNombreCampo")]
    public string NombreCampo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoDato", Description = "TooltipTipoDato")]
    public IMTipoDato TipoDato { get; set; }

    [IgnoreDataMember]
    public List<IMTipoDato> Tipos { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroColumna", Description = "TooltipNumeroColumna")]
    public string NumeroColumna { get; set; }
  }
}