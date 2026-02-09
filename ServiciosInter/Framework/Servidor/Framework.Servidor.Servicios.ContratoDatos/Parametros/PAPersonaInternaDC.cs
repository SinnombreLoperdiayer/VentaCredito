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
  /// Clase que contiene la informacion de una persona externa
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAPersonaInternaDC : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("PEI_IdPersonaInterna")]
    [Filtrable("PEI_IdPersonaInterna", typeof(Etiquetas), "Id", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Id", Description = "ToolTipId")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public long IdPersonaInterna { get; set; }

    [DataMember]
    [CamposOrdenamiento("PEI_Nombre")]
    [Filtrable("PEI_Nombre", typeof(Etiquetas), "Nombre", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombreCompleto { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "TipoIdentificacion", Description = "TooltipTipoIdentificacion")]
    public string IdTipoIdentificacion { get; set; }

    [DataMember]
    [CamposOrdenamiento("PEI_Identificacion")]
    [Filtrable("PEI_Identificacion", typeof(Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
    public string Identificacion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Cargo", Description = "TooltipCargo")]
    public int IdCargo { get; set; }

    [DataMember]
    [CamposOrdenamiento("PEI_Nombre")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "PrimerNombre", Description = "TooltipPrimerNombre")]
    public string Nombre { get; set; }

    [DataMember]
    [CamposOrdenamiento("PEI_PrimerApellido")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellidoPersona")]
    public string PrimerApellido { get; set; }

    [DataMember]
    [CamposOrdenamiento("PEI_SegundoApellido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "SegundoApellido", Description = "TooltipSegundoApellido")]
    public string SegundoApellido { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string Direccion { get; set; }

    [DataMember]
    [CamposOrdenamiento("LOC_Nombre")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
    public string Municipio { get; set; }

    [DataMember]
    [CamposOrdenamiento("LOC_Nombre")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
    public string NombreMunicipio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    public string Telefono { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Celular", Description = "TooltipCelular")]
    public string NumeroCelular { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "RegionalResponsable", Description = "ToolTipRegionalResponsable")]
    public long IdRegionalAdministrativa { get; set; }

    [DataMember]
    [CamposOrdenamiento("REA_Descripcion")]
    [Filtrable("REA_Descripcion", typeof(Etiquetas), "Regional", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Regional")]
    public string NombreRegional { get; set; }

    [DataMember]
    [CamposOrdenamiento("CAR_Descripcion")]
    [Filtrable("CAR_Descripcion", typeof(Etiquetas), "Cargo", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Cargo", Description = "ToolitipCargo")]
    public string NombreCargo { get; set; }

    [DataMember]
    public int idContrato { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    /// <summary>
    /// retorna o asigna el correo de la persona
    /// </summary>
    [DataMember]
    [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Email", Description = "Tooltipemail")]
    [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Email { get; set; }
  }
}