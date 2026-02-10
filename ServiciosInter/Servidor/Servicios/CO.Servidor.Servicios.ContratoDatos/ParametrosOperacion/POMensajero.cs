using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    /// <summary>
    /// Clase que contiene la informacion los mensajeros
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class POMensajero : DataContractBase
    {
        [DataMember]
        public long IdMensajero { get; set; }

        [DataMember]
        public int IdTipoMensajero { get; set; }

        [DataMember]
        public long IdAgencia { get; set; }

        [DataMember]
        [CamposOrdenamiento("CES_Nombre")]
        public string NombreAgencia { get; set; }

        [DataMember]
        public string Telefono2 { get; set; }

        [DataMember]
        public DateTime FechaIngreso { get; set; }

        [DataMember]
        public DateTime FechaTerminacionContrato { get; set; }

        [DataMember]
        public string NumeroPase { get; set; }

        [DataMember]
        public DateTime FechaVencimientoPase { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public bool EsContratista { get; set; }

        [DataMember]
        public string TipoContrato { get; set; }

        [DataMember]
        public PAPersonaInternaDC PersonaInterna { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        [Filtrable("PEI_Nombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Nombre", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
        [CamposOrdenamiento("PEI_Nombre")]
        public string Nombre { get; set; }

        [DataMember]
        [Filtrable("PEI_Identificacion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        [CamposOrdenamiento("PEI_Identificacion")]
        public string Identificacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
        public PALocalidadDC LocalidadMensajero { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "TipoContrato")]
        public POTipoContrato TipoContratoMensajero { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoMensajero", Description = "TipoMensajero")]
        public OUTipoMensajeroDC TipoMensajero { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "Estado")]
        public OUEstadosMensajeroDC EstadoMensajero { get; set; }

        /// <summary>
        /// Bandera de Mensajero Urbano
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MensajeroUrbano", Description = "ToolTipMensajeroUrbano")]
        public bool EsMensajeroUrbano { get; set; }
    }
}