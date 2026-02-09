using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion  del responsable del servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAResponsableServicio : DataContractBase
  {
    [DataMember]
    [Required]
    public PAPersonaExterna PersonaExterna { get; set; }

    [DataMember]
    [CamposOrdenamiento("PRS_Fax")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Fax", Description = "TooltipFax")]
    [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Fax { get; set; }

    [DataMember]
    [CamposOrdenamiento("PEE_PrimerApellido")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Email", Description = "Tooltipemail")]
    [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Email { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [IgnoreDataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "TipoIdentificacion", Description = "TooltipTipoIdentificacion")]
    //[Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdTipoIdentificacion { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_Identificacion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    [Filtrable("PEE_Identificacion", typeof(Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
    public string Identificacion { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_PrimerNombre")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "PrimerNombre", Description = "TooltipPrimerNombre")]
    [Filtrable("PEE_PrimerNombre", typeof(Etiquetas), "PrimerNombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string PrimerNombre { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_PrimerApellido")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellidoPersona")]
    [Filtrable("PEE_PrimerApellido", typeof(Etiquetas), "PrimerApellido", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string PrimerApellido { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_Direccion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    [StringLength(250, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Direccion { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_Telefono")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Telefono { get; set; }

    private string municipio;

    [IgnoreDataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
    public string Municipio
    {
      get { return municipio; }
      set
      {
        if (!string.IsNullOrEmpty(value) && municipio != value)
          municipio = value;
      }
    }
  }
}