using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// Clase con el DataContract de una guia interna
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADGuiaInternaDC : DataContractBase
    {
        [DataMember]
        public long IdAdmisionGuia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuiaInterna", Description = "TooltipNumeroGuiaInterna")]
        public long NumeroGuia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AreaInternaOrigen", Description = "TooltipAreaInternaOrigen")]
        public ARGestionDC GestionOrigen { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AreaInternaDestino", Description = "TooltipAreaInternaDestino")]
        public ARGestionDC GestionDestino { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NombreNoValido")]
        [StringLength(250, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NombreRemitente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [StringLength(25, MinimumLength = 7, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string TelefonoRemitente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string DireccionRemitente { get; set; }

        [DataMember]
        public long IdCentroServicioOrigen { get; set; }

        [DataMember]
        public string NombreCentroServicioOrigen { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadOrigen", Description = "LocalidadOrigen")]
        public PALocalidadDC LocalidadOrigen { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadDestino", Description = "LocalidadDestino")]
        public PALocalidadDC LocalidadDestino { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Pais", Description = "Pais")]
        public PALocalidadDC PaisDefault { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NombreNoValido")]
        [StringLength(250, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NombreDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [StringLength(25, MinimumLength = 7, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string TelefonoDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string DireccionDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DiceContener", Description = "TooltipDiceContener")]
        [StringLength(250, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string DiceContener { get; set; }

        [DataMember]
        public long IdCentroServicioDestino { get; set; }

        [DataMember]
        public string NombreCentroServicioDestino { get; set; }

        [DataMember]
        public bool EsManual { get; set; }

        [DataMember]
        public bool EsOrigenGestion { get; set; }

        [DataMember]
        public bool EsDestinoGestion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public int TiempoEntrega { get; set; }

        [DataMember]
        public string GuidDeChequeo { get; set; }

        [DataMember]
        public string IdentificacionRemitente { get; set; }

        [DataMember]
        public string IdentificacionDestinatario { get; set; }
        [DataMember]
        public string TipoIdentificacionRemitente { get; set; }
        [DataMember]
        public string TipoIdentificacionDestinatario { get; set; }

        [DataMember]
        public string EmailRemitente { get; set; }

        [DataMember]
        public string EmailDestinatario { get; set; }

        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public ADTipoEntrega TipoEntrega { get; set; }

        [DataMember]
        public DateTime FechaEstimadaEntrega { get; set; }

        [DataMember]
        public string InfoCasillero { get; set; }
    }
}