using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUMensajeroDC : DataContractBase
    {
        /// <summary>
        /// retorna o asigna el estado para un control
        /// </summary>
        [DataMember]
        public bool Habilitado { get; set; }

        /// <summary>
        /// retorna o asigna el id del mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
        public long IdMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el id del tipo de mensajero
        /// </summary>
        [DataMember]
        public short IdTipoMensajero { get; set; }

        /// <summary>
        /// retorna o asigna la persona interna
        /// </summary>
        [DataMember]
        public OUPersonaInternaDC PersonaInterna { get; set; }

        /// <summary>
        /// retorna o asigna nombre y apellido
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Apellidos", Description = "Apellidos")]
        public string Apellidos { get; set; }

        /// <summary>
        /// retorna o asigna nombre y apellido
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreMensajero", Description = "NombreMensajero")]
        [Filtrable("NombreCompleto", "Nombre: ", COEnumTipoControlFiltro.TextBox)]
        [CamposOrdenamiento("NombreCompleto")]
        public string NombreCompleto { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoMensajero", Description = "TipoMensajero")]
        public string TipoMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el estado del mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        public OUEstadosMensajeroDC Estado { get; set; }

        /// <summary>
        /// retorna o asigna el numero del pase
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPase", Description = "NumeroPase")]
        public string NumeroPase { get; set; }

        /// <summary>
        /// retorna o asigna la fecha de vencimiento del pase
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaVencimientoPase", Description = "FechaVencimientoPase")]
        [DataType(DataType.DateTime)]
        public DateTime FechaVencimientoPase { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaIngreso", Description = "ToolTipFechaIngreso")]
        [DataType(DataType.DateTime)]
        public DateTime FechaIngreso { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaTerminacionContrato", Description = "ToolTipFechaTerminacionContrato")]
        [DataType(DataType.DateTime)]
        public DateTime FechaTerminacionContrato { get; set; }

        /// <summary>
        /// retorna o asigna si el mensajero es contratista
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EsContratista", Description = "EsContratista")]
        public bool EsContratista { get; set; }

        /// <summary>
        /// retorna o asigna si el mensajero es contratista
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EsMensajeroPAM", Description = "EsMensajeroPAM")]
        public bool EsMensajeroPAM { get; set; }

        /// <summary>
        /// retorna o asigna el telefono2 del mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono2", Description = "Telefono2")]
        public string Telefono2 { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoMensajero", Description = "TipoMensajero")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public OUTipoMensajeroDC TipMensajeros { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// retorna o asigna el id de la agencia del mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoAgencia", Description = "TipoAgencia")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public long Agencia { get; set; }

        /// <summary>
        /// retorna o asigna el nombre de la agencia
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string NombreAgencia { get; set; }

        /// <summary>
        /// retorna o asigna la fecha del sistema
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "FechaCreacion")]
        public DateTime FechaActual { get; set; }

        /// <summary>
        /// retorna o asigna si el tipo del mensajero es vehicular
        /// </summary>
        [DataMember]
        public bool EsVehicular { get; set; }

        /// <summary>
        /// retorna o asigna el usuario autorizado para realizar la asignacion
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }

        /// <summary>
        /// retorna o asigna la localidad destino del mensajero
        /// </summary>
        
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
        public PALocalidadDC LocalidadMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el cargo del mensajero
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cargo", Description = "Cargo")]
        public SECargo CargoMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de contrato del mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "TipoContrato")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public POTipoContrato TipoContrato { get; set; }

        /// <summary>
        /// Bandera de Mensajero Urbano
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MensajeroUrbano", Description = "ToolTipMensajeroUrbano")]
        public bool EsMensajeroUrbano { get; set; }

        /// <summary>
        /// Bandera de Mensajero Urbano
        /// </summary>
        [DataMember]        
        public long IdCentroServicio { get; set; }

    }
}