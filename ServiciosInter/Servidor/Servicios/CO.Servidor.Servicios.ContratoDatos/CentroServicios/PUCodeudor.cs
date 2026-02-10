using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de un codeudor
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUCodeudor : DataContractBase
  {
    [DataMember]
    public PAPersonaExterna PersonaExterna { get; set; }

    [DataMember]
    public long idCentroServicios { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fax", Description = "TooltipFax")]
    [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Fax { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Email { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Ocupacion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EmpresaEmpleador", Description = "TooltipEmpresaEmpleador")]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string EmpresaEmpleador { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ingresos", Description = "TooltipIngresosActuales")]
    public decimal IngresosEmpleoActual { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PoseeFincaRaiz", Description = "TooltipPoseeFincaRaiz")]
    public bool PoseeFincaRaiz { get; set; }

    [DataMember]
    [CamposOrdenamiento("PRL_IdPersonaExterna")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombre")]
    public string NombreCompuesto { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [IgnoreDataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TooltipTipoIdentificacion")]
    //[Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdTipoIdentificacion { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_Identificacion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    [Filtrable("PEE_Identificacion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
    public string Identificacion { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_PrimerNombre")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerNombre", Description = "TooltipPrimerNombre")]
    [Filtrable("PEE_PrimerNombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "PrimerNombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    [StringLength(50, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string PrimerNombre { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_PrimerApellido")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellidoPersona")]
    //[Filtrable("PEE_PrimerApellido", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "PrimerApellido", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    //[StringLength(50, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string PrimerApellido { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_Direccion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    [StringLength(250, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Direccion { get; set; }

    [IgnoreDataMember]
    [CamposOrdenamiento("PEE_Telefono")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Telefono { get; set; }

    private string municipio;

    [IgnoreDataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
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