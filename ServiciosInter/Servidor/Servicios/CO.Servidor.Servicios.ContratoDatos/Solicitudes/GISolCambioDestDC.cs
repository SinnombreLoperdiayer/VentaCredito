using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la informacion de la tbl Solicitud Cambio Destinatario
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GISolCambioDestDC : DataContractBase
  {
    [DataMember]
    public long? IdGiroCambioDest { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TooltipIdentificacion")]
    public string TipDocActual { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TooltipIdentificacion")]
    public string TipDocNuevo { get; set; }

    [DataMember]
    public PATipoIdentificacion TipoDocumentoNuevo { get; set; }

    [DataMember]
    public PATipoIdentificacion TipoDocumentoReclamaNuevo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public string IdentActual { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public string IdentNuevo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombreActual { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombreNuevo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellido")]
    public string Apellido1Actual { get; set; }

    private string apellido1Nuevo;
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]    
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellido")]    
    public string Apellido1Nuevo
    {
        get { return apellido1Nuevo; }
        set { apellido1Nuevo = value; }
    }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SegundoApellido", Description = "TooltipSegundoApellido")]
    public string Apellido2Actual { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SegundoApellido", Description = "TooltipSegundoApellido")]
    public string Apellido2Nuevo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    public string TelefonoActual { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    public string TelefonoNuevo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string DirActual { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string DirNuevo { get; set; }

    [DataMember]
    public string MailActual { get; set; }

    [DataMember]
    public string MailNuevo { get; set; }

    /// <summary>
    /// es el Campo que se cambio la creacion de la
    /// solicitud
    /// </summary>
    [DataMember]
    public string CampoModificado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentReclama", Description = "ToolTipTipoIdentReclama")]
    public string TipIdReclamaActual { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentReclama", Description = "ToolTipTipoIdentReclama")]
    public string TipIdReclamaNuevo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
    public string OcupacionActual { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
    public string OcupacionNuevo { get; set; }

    [DataMember]
    public ObservableCollection<PATipoIdentificacion> LstTipoIdentificacion { get; set; }

    [DataMember]
    public ObservableCollection<PATipoIdentificacion> LstTipoIdentificacionReclama { get; set; }
  }
}