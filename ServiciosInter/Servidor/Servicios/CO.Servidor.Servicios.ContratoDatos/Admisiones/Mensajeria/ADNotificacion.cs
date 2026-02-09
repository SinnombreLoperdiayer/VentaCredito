using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADNotificacion
    {
        private TATipoDestino tipoDestino;

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

        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoDestino", Description = "TooltipTipoDestino")]
        [DataMember]
        public TATipoDestino TipoDestino
        {
            get
            {
                return tipoDestino;
            }
            set
            {
                tipoDestino = value;
            }
        }

        /// <summary>
        /// Tipos de destino
        /// </summary>
        private ObservableCollection<TATipoDestino> tiposDestino;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiposDestino", Description = "TooltipTiposDestino")]
        public ObservableCollection<TATipoDestino> TiposDestino
        {
            get { return tiposDestino; }
            set { tiposDestino = value; }
        }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TipoIdentificacion")]
        public string TipoIdentificacionDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        public string IdDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
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
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        public string DireccionDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]
        public string EmailDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool ReclamaEnOficina { get; set; }

        [DataMember]
        public ADGuiaInternaDC GuiaInterna { get; set; }

        [DataMember]
        public ADGuia GuiaAdmision { get; set; }

        [DataMember]
        public LIRecibidoGuia RecibidoGuia { get; set; }

        [DataMember]
        public ADEstadoGuia EstadoGuia { get; set; }

        [DataMember]
        public int TotalRegistros { get; set; }

        [DataMember]
        public long IdRecibidoGuia { get; set; }

        [DataMember]
        public long IdArchivoGuia { get; set; }

        [DataMember]
        public bool EstaDevuelta { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public long IdPlanillaCertificacionGuia { get; set; }

        [DataMember]
        public PUCentroServiciosDC CentroServicioDestino { get; set; }

        [DataMember]
        public string FormaPago { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string IdentificacionSolicitante { get; set; }


        [DataMember]
        public string NombreSolicitante { get; set; }

        [DataMember]
        public string TelefonoSolicitante { get; set; }

        [DataMember]
        public string NombreCiudadDestino { get; set; }

        [DataMember]
        public DateTime FechaImpresion { get; set; }

        [DataMember]
        public LICertificacionWebDC TipoCertificacion { get; set; }

        [DataMember]
        public SUNumeradorPrefijo NumeroSuministro { get; set; }

        [DataMember]
        public long ExisteGuiaAuditoria { get; set; }

    }
}