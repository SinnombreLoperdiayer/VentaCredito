using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros
{
    /// <summary>
    /// Clase que contiene la informacion de la tbl de Admision Giros
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class GIAdmisionGirosDC
    {
        #region VENTA DE GIROS

        [DataMember]
        [CamposOrdenamiento("ADG_IdAdmisionGiro")]
        public long? IdAdminGiro { get; set; }

        [DataMember]
        [CamposOrdenamiento("ADG_IdGiro")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGiro", Description = "ToolTipNumeroGiro")]
        public long? IdGiro { get; set; }

        [IgnoreDataMember]
        public string PrefijoIdGiro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoVerificacion", Description = "ToolTipCodigoVerificacion")]
        public string CodVerfiGiro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EstadoGiro", Description = "ToolTipEstadoGiro")]
        public string EstadoGiro { get; set; }

        [DataMember]
        public List<GISolicitudGiroDC> SolicitudGiro { get; set; }

        [DataMember]
        public PUCentroServiciosDC AgenciaOrigen { get; set; }

        [DataMember]
        public PUCentroServiciosDC AgenciaDestino { get; set; }

        [DataMember]
        public TAPrecioDC Precio { get; set; }

        [DataMember]
        public long? DeclaracionVoluntariaOrigenes { get; set; }

        [DataMember]
        public bool RequiereDeclaracionVoluntaria { get; set; }

        [DataMember]
        public string ArchivoDeclaracionVoluntariaOrigenes { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "Observaciones")]
        public string Observaciones { get; set; }

        [DataMember]
        public string GuidDeChequeo { get; set; }

        [DataMember]
        public GIGirosPeatonPeatonDC GirosPeatonPeaton { get; set; }

        [DataMember]
        public GIGirosPeatonConvenioDC GirosPeatonConvenio { get; set; }

        /// <summary>
        /// Fecha de grabacion del giro
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "FechaCreacion")]
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// Numero de giro de una agencia manual
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGiro", Description = "ToolTipNumeroGiro")]
        public long NumeroGiroAgenciaManual { get; set; }

        /// <summary>
        /// Id de la caja que realiza el giro
        /// </summary>
        [DataMember]
        public int IdCaja { get; set; }

        /// <summary>
        /// Es el id del usuario. Rafram 14-09-2012
        /// </summary>
        [DataMember]
        public long IdCodigoUsuario { get; set; }

        /// <summary>
        /// Id Tipo de giro
        /// </summary>
        [DataMember]
        public string IdTipoGiro { get; set; }

        /// <summary>
        /// Intentos de transmitir
        /// </summary>
        [DataMember]
        public ObservableCollection<GIIntentosTransmisionGiroDC> IntentosTransmitir { get; set; }

        /// <summary>
        /// true habilita la venta de giros
        /// </summary>
        [DataMember]
        public bool HabilitarVentaGiros { get; set; }

        /// <summary>
        /// indica si el texto que debe ir cuando  es una reimpresion del giro
        /// </summary>
        [DataMember]
        public string ReImpresion { get; set; }

        [DataMember]
        public bool RecibeNotificacionPago { get; set; }

        /// <summary>
        /// Es el Usuario que creo el giro
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        public string UsuarioCreacionGiro { get; set; }

        /// <summary>
        /// Verdadero cuando es un giro Automatico
        /// Falso Giro Manual
        /// </summary>
        [DataMember]
        public bool GiroAutomatico { get; set; }

        #endregion VENTA DE GIROS

        #region PAGOS DE GIROS

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaLimitePagoGiro")]
        public DateTime FechaLimitePagoGiro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Documento")]
        public string DocumentoDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre")]
        public string NombreDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono")]
        public string TelefonoDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email")]
        public string EmailDestinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Documento")]
        public string DocumentoRemitente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre")]
        public string NombreRemitente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono")]
        public string TelefonoRemitente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email")]
        public string EmailRemitente { get; set; }

        [DataMember]
        public bool EsTransmitido { get; set; }

        [DataMember]
        public PGPagoPorDevolucionDC infoPagoDevolucion { get; set; }

        #endregion PAGOS DE GIROS

        #region VALIDACIÓN

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Gestion")]
        public bool TieneGestion { get; set; }

        /// <summary>
        /// Esta es la caja de carton en donde se guarda el documento físico del giro. Ojo esta no es la caja del punto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Caja")]
        public long Caja { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Lote")]
        public int Lote { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Posicion")]
        public int Posicion { get; set; }

        [DataMember]
        public bool Aprobada { get; set; }

        [DataMember]
        public bool NoAprobada { get; set; }
        
        [DataMember]
        public string ObservacionesSolicitudes { get; set; }

        #endregion VALIDACIÓN

        // TODO:ID Este campo solo se utiliza para la integracion con 472
        [DataMember]
        public long IdEstadoGiro { get; set; }
        
        // TODO:ID Este campo solo se utiliza para la integracion con 472
        [DataMember]
        public DateTime FechaEstadoGiro { get; set; }

        // TODO:ID Este campo solo se utiliza para la integracion con 472
        [DataMember]
        public bool EstACT_yaTransmitido { get; set; }
    }

}