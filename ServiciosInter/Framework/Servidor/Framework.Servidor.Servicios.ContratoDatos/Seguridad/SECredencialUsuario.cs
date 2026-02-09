using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    /// <summary>
    /// Clase que contiene la informacion del usuario
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class SECredencialUsuario : DataContractBase
    {
        [DataMember]
        [CamposOrdenamiento("USU_IdUsuario")]
        [Filtrable("USU_IdUsuario", "Usuario:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 20)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(20, MinimumLength = 5, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Usuario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string TipoIdentificacion { get; set; }

        [DataMember]
        [Filtrable("PEI_Identificacion", typeof(Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
        [CamposOrdenamiento("PEI_Identificacion")]
        [StringLength(12, MinimumLength = 3, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Identificacion { get; set; }

        [DataMember]
        [Filtrable("PEI_Nombre", "Nombre:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 20)]
        [CamposOrdenamiento("PEI_Nombre", "PEI_PrimerApellido", "PEI_SegundoApellido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Nombre { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEI_PrimerApellido", "PEI_Nombre", "PEI_SegundoApellido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Apellido1")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Apellido1 { get; set; }

        [DataMember]
        [CamposOrdenamiento("PEI_SegundoApellido", "PEI_Nombre", "PEI_PrimerApellido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Apellido2")]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Apellido2 { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Direccion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(20, MinimumLength = 7, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Telefono { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PasswordNuevo")]
        [StringLength(20, MinimumLength = 8, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]

        //[RegularExpression(@"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z])(?=.*[@#$%^&+=]).*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "PasswordNocumple")]
        public string PasswordNuevo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulos")]
        public List<SEModulo> Modulos { get; set; }

        [DataMember]
        [Display(Name = "ID maquina")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool RequiereIdentificadorMaquina { get; set; }

        [DataMember]
        [Display(Name = "Carga masiva")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool AutorizaCargaMasiva { get; set; }

        [DataMember]
        [Display(Name = "Carga masiva ICA")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool AutorizaCargaMasivaICA { get; set; }

        [DataMember]
        public bool CambiarClave { get; set; }

        [DataMember]
        public string PassServicios { get; set; }

        [DataMember]
        public string UserServicios { get; set; }

        public short CantidadIntentosFallidos { get; set; }

        public DateTime? InicioIntentosFallidos { get; set; }

        [DataMember]
        [Display(Name = "Clave bloqueada")]
        public bool ClaveBloqueada { get; set; }

        public string FormatoClave { get; set; }

        [DataMember]
        public string PasswordAnterior { get; set; }

        public int DiasVencimiento { get; set; }

        public DateTime FechaUltimoCambioClave { get; set; }

        [DataMember]
        [CamposOrdenamiento("CAR_Descripcion")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cargo")]
        public string Cargo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCargo")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdCargo { get; set; }

        [DataMember]
        [Filtrable("REA_Descripcion", "Regional:", COEnumTipoControlFiltro.TextBox)]
        [CamposOrdenamiento("REA_IdRegionalAdm")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Regional")]
        public string Regional { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdRegional")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public long IdRegional { get; set; }

        [DataMember]
        [CamposOrdenamiento("USU_TipoUsuario")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoUsuario")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string TipoUsuario { get; set; }


        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(200, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Comentarios { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Estado { get; set; }

        /// <summary>
        /// Indica si el usuario corresponde a un usuario interno, de no ser así, corresponde a un Servidor
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EsUsuarioInterno")]
        public bool EsUsuarioInterno { get; set; }

        [IgnoreDataMember]
        public SETipoAutenticacion TipoAutenticacion { get; set; }

        /// <summary>
        /// Contiene el identificador de la credencial en la  base de datos
        /// </summary>
        [DataMember]
        public long IdCodigoUsuario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad")]
        public PALocalidadDC CiudadUsuario { get; set; }

        [DataMember]

        // [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad")]
        public string Municipio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Pais")]
        public PALocalidadDC PaisUsuario { get; set; }

        [DataMember]
        public long IdCodUsuarioSeg { get; set; }

        [DataMember]
        public long IdPersonaInterna { get; set; }

        [DataMember]
        public SEEnumMensajesSeguridad Mensaje { get; set; }

        /// <summary>
        /// Listado de puntos/agencias/cols/racols/gestiones o sucursales autorizadas al usuario
        /// </summary>
        [DataMember]
        public List<SEUbicacionAutorizada> LocacionesAutorizadas { get; set; }

        /// <summary>
        /// Listado de centros de servicio autorizados para la autenticación del usuario
        /// </summary>
        [DataMember]
        public ObservableCollection<SECentroServicio> CentrosDeServicioAutorizados { get; set; }

        /// <summary>
        /// Listado de gestiones autorizadas para la autenticación del usuario
        /// </summary>
        [DataMember]
        public ObservableCollection<SEGestion> GestionesAutorizadas { get; set; }

        /// <summary>
        /// Listado de sucursales autorizadas para la autenticación del usuario
        /// </summary>
        [DataMember]
        public ObservableCollection<SESucursal> SucursalesAutorizadas { get; set; }

        /// <summary>
        /// define si es persona Interna o Externa
        /// true si es persona Interna
        /// </summary>
        [DataMember]
        public bool EsPersonaInterna { get; set; }

        /// <summary>
        /// define el Servidor al que pertenece el usuario, cuando es un usuario de un Servidor credito
        /// </summary>
        [DataMember]
        public int IdServidorCredito { get; set; }


        /// <summary>
        /// define si es persona Interna o Externa
        /// true si es persona Interna
        /// </summary>
        [DataMember]
        [Display(Name = "PAM")]        
        public bool AplicaPAM { get; set; }


        /// <summary>
        /// Token de sesion para el usuario
        /// </summary>
        [DataMember]
        public string TokenSesion { get; set; }

        [DataMember]
        public string IdentificadorMaquina { get; set; }

        [DataMember]
        public int IdAplicativoOrigen { get; set; }

        /// <summary>
        /// define el cliente al que pertenece el usuario, cuando es un usuario de un cliente credito
        /// </summary>
        [DataMember]
        public int IdClienteCredito { get; set; }
    }
}