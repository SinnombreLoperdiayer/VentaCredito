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
    public class PAPersonaExterna : DataContractBase
    {
        [DataMember]
        public long IdPersonaExterna { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "TipoIdentificacion", Description = "TooltipTipoIdentificacion")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdTipoIdentificacion { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEE_Identificacion")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        [Filtrable("PEE_Identificacion", typeof(Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
        [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
        public string Identificacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        [StringLength(1, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "DVError")]
        public string DigitoVerificacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "FechaExpedicionDocumento", Description = "TooltipFechaExpeDocumento")]
        public DateTime FechaExpedicionDocumento { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEE_PrimerNombre")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "PrimerNombre", Description = "TooltipPrimerNombre")]
        [Filtrable("PEE_PrimerNombre", typeof(Etiquetas), "PrimerNombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string PrimerNombre { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "SegundoNombre", Description = "TooltipSegundoNombre")]
        public string SegundoNombre { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEE_PrimerApellido")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellidoPersona")]
        [Filtrable("PEE_PrimerApellido", typeof(Etiquetas), "PrimerApellido", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string PrimerApellido { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "SegundoApellido", Description = "TooltipSegundoApellido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string SegundoApellido { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEE_Direccion")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Direccion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
        public string Municipio { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdDepto { get; set; }

        [DataMember]
        public string NombreDepto { get; set; }

        [DataMember]
        public string IdPais { get; set; }

        [DataMember]
        public string NombrePais { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
        public string NombreMunicipio { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEE_Telefono")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Telefono { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEE_NumeroCelular")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Celular", Description = "TooltipCelular")]
        [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NumeroCelular { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "NombreCompleto", Description = "TooltipNombreCompleto")]
        [CamposOrdenamiento("PEE_PrimerNombre")]
        public string NombreCompleto { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}