using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
  /// Clase que contiene la informacion de un propietario de un centro de servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUPropietario : DataContractBase
  {

    
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdPropietario", Description = "TooltipIdPropietario")]
    [CamposOrdenamiento("CON_IdPropietario")]
    public int IdPropietario { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RepresentanteLegal", Description = "TooltipRepresentanteLegal")]
    public Int64 IdRepresentanteLegal { get; set; }

    //private string nombreRepresentanteLegal;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RepresentanteLegal", Description = "TooltipRepresentanteLegal")]
    [CamposOrdenamiento("PEE_PrimerNombre")]
    public string NombreRepresentanteLegal { get; set; }

    [DataMember]
    [StringLength(50, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Filtrable("CON_RazonSocial", "Razón Social: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("CON_RazonSocial")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RazonSocial", Description = "TooltipRazonSocial")]
    public string RazonSocial { get; set; }

    [DataMember]
    [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Filtrable("CON_Nit", "Nit: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("CON_Nit")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nit", Description = "TooltipNit")]
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NitNoValido")]
    public string Nit { get; set; }

    [DataMember]
    [StringLength(1, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DigitoVerificacion", Description = "TooltipDigitoVerificacion")]    
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "DVError")]
    public string DigitoVerificacion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSociedad", Description = "TooltipTipoSociedad")]
    public int IdTipoSociedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSociedad", Description = "TooltipTipoSociedad")]
    [CamposOrdenamiento("TIS_Descripcion")]
    public string NombreTipoSociedad { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ActividadEconomica", Description = "TooltipActividadEconomica")]
    public int IdActividad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ActividadEconomica", Description = "TooltipActividadEconomica")]
    [CamposOrdenamiento("TAE_Descripcion")]
    public string NombreActividad { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegimenContributivo", Description = "TooltipRegimenContributivo")]
    public int IdRegimenContributivo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegimenContributivo", Description = "TooltipRegimenContributivo")]
    [CamposOrdenamiento("TRC_Descripcion")]
    public string NombreRegimenContributivo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaConstitucion", Description = "TooltipFechaConstitucion")]
    [CamposOrdenamiento("CON_FechaConstitucion")]
    public DateTime FechaConstitucion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PagaPorIntergiro", Description = "TooltipPagaPorIntergiro")]
    public bool PagaPorInterGiro { get; set; }

    [DataMember]
    [StringLength(250, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string Direccion { get; set; }

    [DataMember]
    [StringLength(25, MinimumLength = 7, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    public string Telefono { get; set; }

    [DataMember]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Barrio", Description = "TooltipBarrio")]
    public string Barrio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
    public string IdMunicipio { get; set; }

    [DataMember]
    public string IdDepto { get; set; }

    [DataMember]
    public string NombreDepto { get; set; }

    [DataMember]
    public string IdPais { get; set; }

    [DataMember]
    public string NombrePais { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
    public string NombreMunicipio { get; set; }

    [DataMember]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]
    public string Email { get; set; }

    [DataMember]
    [StringLength(25, MinimumLength = 7, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fax", Description = "TooltipFax")]
    public string Fax { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ArchivosCentroServicios", Description = "ToolTipArchivosCentroServicios")]
    public string ArchivosRequeridos { get; set; }

    [DataMember]
    public ObservableCollection<PUArchivosPropietario> ArchivosPropietarios { get; set; }

    private PALocalidadDC ciudadUbicacion;
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]    
    public PALocalidadDC CiudadUbicacion 
    {
      get { return ciudadUbicacion; }
      set
      {
        if (value != ciudadUbicacion)
        {
          ciudadUbicacion = value;
          IdMunicipio = ciudadUbicacion.IdLocalidad;         
        }
      }
    }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}