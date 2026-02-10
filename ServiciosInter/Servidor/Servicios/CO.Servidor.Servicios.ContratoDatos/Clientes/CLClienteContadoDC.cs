using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
    /// <summary>
    /// Clase con el DataContract de clientes Contado
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CLClienteContadoDC : DataContractBase
    {
        private bool ocupacionVisible = true;

        private bool tipoIdentificacionReclamoVisible = true;

        private bool estaEnListasRestrictivas = true;

        [DataMember]
        public long IdClienteContado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TipoIdentificacion")]
        public string TipoId { get; set; }
        [DataMember]
        public int TipoIdentificacion
        {
            get
            {
                return IdentificaTipoIdentificacion();
            }
            set { }
        }

        private int IdentificaTipoIdentificacion()
        {
            int resultado = 0;
            switch (this.TipoId)
            {
                case "CC":
                    resultado = 1;
                    break;
                case "CE":
                    resultado = 2;
                    break;
                case "NI":
                    resultado = 3;
                    break;
                case "TI":
                    resultado = 4;
                    break;
            }
            return resultado;
        }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
        [StringLength(25, MinimumLength = 4, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
       public string Identificacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NombreNoValido")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Nombre { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellido")]
        [RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "Apellido1NoValido")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Apellido1 { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SegundoApellido", Description = "TooltipSegundoApellido")]
        [RegularExpression(@"^([a-zA-ZñÑáéíóúÁÉÍÓÚ0-9]+\s?)*$", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "Apellido2NoValido")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Apellido2 { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "TelefonoNoValido")]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Telefono { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, MinimumLength = 2, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Direccion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]

        //[RegularExpression(@"\b[a-zA-Z0-35._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}\b", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
        [RegularExpression(@"\b[a-zA-Z0-35_%-]+(\\.[_A-Za-z0-35-]+)*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}\b", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
        public string Email { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
        public PAOcupacionDC Ocupacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacionReclama", Description = "TipoIdentificacionReclamoGiro")]
        public string TipoIdentificacionReclamoGiro { get; set; }

        [DataMember]
        public IList<CLDestinatarioFrecuenteDC> DestinatariosFrecuentes { get; set; }

        [IgnoreDataMember]
        public long? UltimaCedulaEscaneada { get; set; }

        public string NombreYApellidos
        {
            get
            {
                if (Nombre == null)
                    Nombre = "";
                if (Apellido1 == null)
                    Apellido1 = "";
                if (Apellido2 == null)
                    Apellido2 = "";
                return Nombre + Apellido1 + Apellido2;
            }
        }

        /// <summary>
        /// True: los datos del cliente fueron modificado y es necesario actualizarlos
        /// </summary>
        [DataMember]
        public bool ClienteModificado { get; set; }

        [IgnoreDataMember]
        public bool OcupacionVisible
        {
            get { return ocupacionVisible; }
            set { ocupacionVisible = value; }
        }

        [IgnoreDataMember]
        public bool TipoIdentificacionReclamoVisible
        {
            get { return tipoIdentificacionReclamoVisible; }
            set { tipoIdentificacionReclamoVisible = value; }
        }

        [IgnoreDataMember]
        public bool EstaEnListasRestrictivas
        {
            get { return estaEnListasRestrictivas; }
            set { estaEnListasRestrictivas = value; }
        }

        private bool? requiereConsulta = null;

        /// <summary>
        /// Campo utilizado cuando se setea un clientecontado al control CLClienteContadoControlUC indica que no debe hacer consulta a la base de datos porque dicha información ya se tiene
        /// </summary>
        [IgnoreDataMember]
        public bool? RequiereConsulta
        {
            get
            {
                return requiereConsulta;
            }
            set
            {
                requiereConsulta = value;
            }
        }
    }
}