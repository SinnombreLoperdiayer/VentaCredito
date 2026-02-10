using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// Contiene información de un destino de un cliente
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADInfoClienteDestino
    {
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TipoIdentificacion")]
        public string TipoId { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
        [StringLength(25, MinimumLength = 4, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Identificacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        //[RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Cliente.Servicios.ContratoDatos.Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NombreNoValido")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Nombre { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "TelefonoNoValido")]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Telefono { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Direccion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]
        [RegularExpression(@"\b[a-zA-Z0-35._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}\b", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PaisDestino", Description = "TooltipPaisDestino")]
        [DataMember]
        public PALocalidadDC PaisDestino { get; set; }

        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "TooltipCiudadDestino")]
        [DataMember]
        public PALocalidadDC CiudadDestino
        {
            get;
            set;
        }
        [DataMember]
        public string TipoDestino { get; set; }
    }
}