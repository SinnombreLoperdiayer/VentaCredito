using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADRapiRadicado
  {
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroFolios", Description = "TooltipNumeroFolios")]
    [Range(1, 1000, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NumeroFoliosMayorRango")]
    [DataMember]
    public short NumeroFolios { get; set; }

    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoDestino", Description = "TooltipTipoDestino")]
    [DataMember]
    public TATipoDestino TipoDestino { get; set; }

    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoRapiRadicado", Description = "TooltipCodigoRapiRadicado")]
    [DataMember]
    public string CodigoRapiRadicado { get; set; }

    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PaisDestino", Description = "TooltipPaisDestino")]
    [DataMember]
    public PALocalidadDC PaisDestino { get; set; }

    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "TooltipCiudadDestino")]
    [DataMember]
    public PALocalidadDC CiudadDestino
    {
      get;
      set;
    }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TipoIdentificacion")]
    public string TipoIdentificacionDestinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public string IdDestinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    public string NombreDestinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellido")]
    public string Apellido1Destinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SegundoApellido", Description = "TooltipSegundoApellido")]
    public string Apellido2Destinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
    public string TelefonoDestinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    public string DireccionDestinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]
    public string EmailDestinatario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
    public long NumeroGuiaInterna { get; set; }

    /// <summary>
    /// Tipos de destino
    /// </summary>
    private ObservableCollection<TATipoDestino> tiposDestino;

    [IgnoreDataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiposDestino", Description = "TooltipTiposDestino")]
    public ObservableCollection<TATipoDestino> TiposDestino
    {
      get { return tiposDestino; }
      set { tiposDestino = value; }
    }

    [DataMember]
    public long IdRapiradicado { get; set; }

    [DataMember]
    public ADGuiaInternaDC GuiaInterna { get; set; }

    [DataMember]
    public ADGuia GuiaAdmision { get; set; }

   [DataMember]
    public  PUCentroServiciosDC  CentroServicioCreacion { get; set; }

   [DataMember]
    public ObservableCollection <ADArchivoRadicadoDC>  ListaArchivos { get; set; }
      
  }
}