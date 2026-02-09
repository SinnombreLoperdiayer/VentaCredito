using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Clase con el DataContract de los archivos de logistica inversa
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIPlanillaCertificacionesDC
    {
        [DataMember]
        [Filtrable("PLC_IdPlanillaCertificaciones", "Planilla:", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
        public long NumeroPlanilla { get; set; }

        [DataMember]
        [Filtrable("PLC_FechaGrabacion", "Fecha:", COEnumTipoControlFiltro.DatePicker)]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaPlanilla", Description = "FechaPlanilla")]
        public DateTime FechaPlanilla { get; set; }

        [DataMember]
        public PUCentroServiciosDC CentroServiciosPlanilla { get; set; }

        [DataMember]
        public PURegionalAdministrativa RegionalAdmPlamilla { get; set; }

        [DataMember]
        public ADNotificacion GuiaPlanilla { get; set; }

        [DataMember]
        public ADGuiaInternaDC GuiaInterna { get; set; }

        [DataMember]
        public List<ADNotificacion> LstGuiasPlanilla { get; set; }

        [DataMember]
        public PALocalidadDC PaisDefecto { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
        public long NumeroGuiaCertificacion { get; set; }

        [DataMember]
        public bool SeImprime { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue", Description = "FechaDescargue")]
        public bool Descargada { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue", Description = "FechaDescargue")]
        public DateTime FechaDescargue { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "HoraDescargue", Description = "HoraDescargue")]
        public DateTime HoraDescargue { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "PersonaRecibe", Description = "ToolTipPersonaRecibe")]
        public string PersonaQueRecibe { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public LIEnumTipoPlanillaNotificacion TipoPlanilla { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public long IdDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Destinatario", Description = "Destinatario")]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string NombreDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string DireccionDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string TelefonoDestinatario { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public PALocalidadDC LocalidadDestino { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool Consolidado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool Devolucion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue", Description = "FechaDescargue")]
        public bool Cerrada { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "TipoPlanilla", Description = "TipoPlanilla")]
        public string TipoPlanillaDescripcion { get; set; }

        [DataMember]
        public string NombreClienteCredito { get; set; }
        
    }
}