using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
    /// <summary>
    /// Clase que contiene la informacion de la tbl Solicitud Giro
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class GISolicitudGiroDC : DataContractBase
    {
        [DataMember]
        [CamposOrdenamiento("SOG_IdSolicitudGiro")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdSolicitud", Description = "ToolTipIdSolicitud")]
        [Filtrable("SOG_IdSolicitudGiro", "Id Solicitud", COEnumTipoControlFiltro.TextBox)]
        public long? IdSolicitud { get; set; }

        [DataMember]
        [CamposOrdenamiento("SOG_Estado")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EstadoSolic", Description = "ToolTipEstadoSolicitud")]
        public string EstadoSol { get; set; }

        [DataMember]
        [CamposOrdenamiento("SOG_DescripcionTipoSolicitud")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DescripTipoSolicitud", Description = "ToolTipDescripTipoSolicitud")]
        public string TipoSolDesc { get; set; }

        [DataMember]
        [CamposOrdenamiento("SOG_FechaGrabacion")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "ToolTipFechaGrabSolicitud")]
        [Filtrable("SOG_FechaGrabacion", "Fecha Creacion", COEnumTipoControlFiltro.DatePicker)]
        public DateTime? FGrabacionSol { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdRegional", Description = "ToolTipIdRegional")]
        public long IdRegionalAdmin { get; set; }

        [DataMember]
        [CamposOrdenamiento("ADG_IdGiro")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGiro", Description = "ToolTipNumeroGiro")]
        public long IdGiro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdRegional", Description = "ToolTipIdRegional")]
        public string RegionalAdminDescripcion { get; set; }

        [DataMember]
        public long? IdCentroSolicita { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TootipObsevSolicitud")]
        public string ObservSolicitud { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DescripMotivoSolicitud", Description = "ToolTipDescripMotivSolicitud")]
        public string DescrMotivoSol { get; set; }

        private GITipoSolicitudDC tipoSolicitud;

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSolicitud", Description = "TooltipTipoSolicitud")]
        public GITipoSolicitudDC TipoSolicitud
        {
            get { return this.tipoSolicitud; }
            set { this.tipoSolicitud = value; }
        }

        [DataMember]
        [Display(Name="Usuario Atendió")]
        public string UsuarioAtendio { get; set; }

        [DataMember]
        [Display(Name = "Fecha Atención")]
        public DateTime? FechaAtencion { get; set; }

        [DataMember]
        [Display(Name = "Observaciones Rechazo")]
        public string ObservacionesAtencion { get; set; }

        [IgnoreDataMember]
        public List<PUAgencia> Agencias { get; set; }

        [IgnoreDataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "TooltipMotivo")]
        public List<GIMotivoSolicitudDC> MotivosSolicitudesSolicitudSeleccionada { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        public GIMotivoSolicitudDC MotivoSolicitud { get; set; }

        [IgnoreDataMember]
        public List<GIMotivoSolicitudDC> MotivosSolicitudes { get; set; }

        [DataMember]
        public GIAdmisionGirosDC AdmisionGiro { get; set; }

        [DataMember]
        public GISolCambioDestDC CambioDestinatario { get; set; }

        [DataMember]
        public GISolAtendidasDC SolicitudAtendida { get; set; }

        [DataMember]
        public GIEnumEstadosGirosDC EstadosGiro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EstadoSolic", Description = "ToolTipEstadoSolicitud")]
        public GIEnumEstadosSolGirosDC EstadosSolGiro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AgenciaSolicitud", Description = "TooltipAgencia")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        public PUCentroServiciosDC CentroServicioOrigen { get; set; }

        [IgnoreDataMember]
        public ObservableCollection<PUCentroServiciosDC> CentrosServicios { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegionalAdministrativa", Description = "TooltipRegionalAdministrativa")]
        public PURegionalAdministrativa RACOL { get; set; }

        [IgnoreDataMember]
        public ObservableCollection<PURegionalAdministrativa> RACOLs { get; set; }

        [DataMember]
        public GIArchSolicitudDC ArchivoAsociado { get; set; }

        [DataMember]
        public List<GIArchSolicitudDC> ArchivosAsociados { get; set; }

        [DataMember]
        public List<GIArchivosAdjuntosDC> ArchivosAdjuntos { get; set; }

        [DataMember]
        public PURegionalAdministrativa RegionalAdministrativa { get; set; }

        [IgnoreDataMember]
        public List<PURegionalAdministrativa> RegionalesAdministrativas { get; set; }

        /// <summary>
        /// Es el Usuario que realiza la Transaccion
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioSolicita")]
        public string Usuario { get; set; }

        /// <summary>
        /// Informa si el giro se asigno a una Agencia Manual.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [giro transmitido]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "GiroTransmitido")]
        public string GiroTransmitido { get; set; }

        /// <summary>
        /// Es el id del centro de servicio destino al que
        /// va dirigido el giro.
        /// </summary>
        /// <value>
        /// The id centro servicio destino giro.
        /// </value>
        [DataMember]
        public long idCentroServicioDestinoGiro { get; set; }

        /// <summary>
        /// Es el nombre del centro de servicio de destino al que va
        /// dirigido el giro
        /// </summary>
        /// <value>
        /// The nombre centro destino giro.
        /// </value>
        [DataMember]
        public string NombreCentroDestinoGiro { get; set; }

        /// <summary>
        /// Se guarda la data del cambio de la Agencia solicitado por el
        /// "CentroQueSolicita"  que crea la solcitud, esto es para las agencias que
        /// solicitan un cambio de agencia que no es ella mismas
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AgenciaDestino", Description = "ToolTipAgenciaDestino")]
        public PUCentroServiciosDC CambioAgenciaPorAgencia { get; set; }

        /// <summary>
        /// Es el centro de Servicio que hace la solicitud
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AgenciaSolicitud", Description = "TooltipAgencia")]
        public PUCentroServiciosDC CentroQueSolicita { get; set; }

        [DataMember]
        [CamposOrdenamiento("SOG_NombreCentroServicioSolicita")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CentroSolicitaSol", Description = "ToolTipCentroSolicitaSol")]
        public string CentroSolicita { get; set; }

        /// <summary>
        /// Es el id del formato de la Solicitud
        /// </summary>
        [DataMember]
        public long idConsecutivoFormato { get; set; }

        /// <summary>
        /// Valida el Numero de la Solicitud
        /// si fue digitado Manualmente
        /// </summary>
        [DataMember]
        public bool ValidoSuministroManual { get; set; }

        /// <summary>
        /// Es el tipo de Suministro para el IdSolicitud
        /// digitado "Manualmente"
        /// </summary>
        [DataMember]
        public string TipoSuministroIdSolicitud { get; set; }

        /// <summary>
        /// Es el valor sobre el cual se vendió el giro,
        /// este valor puede ser diferente al registrado
        /// en el sistema y a éste valor se quiere
        /// llegar haciendo el ajuste.
        /// </summary>
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorRealGiro", Description = "ToolTipValorRealGiro")]
        [DataMember]
        public decimal ValorRealGiro { get; set; }

        /// <summary>
        /// Diferencia entre lo registrado en el
        /// sistema como valor del giro y el valor real
        /// </summary>
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorAjusteGiro", Description = "ToolTipValorAjusteGiro")]
        [DataMember]
        public decimal AjusteAlGiro { get; set; }

        /// <summary>
        /// Valor del porte calculado por la venta
        /// del giro en el sistema cuando se vendió
        /// el giro inicialmente, este valor se
        /// guarda como parte del registro de
        /// trazabiliad del ajuste de valor
        /// </summary>
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorAjustePorte", Description = "ToolTipValorAjustePorte")]
        [DataMember]
        public decimal AjusteAlPorte { get; set; }

        /// <summary>
        /// Esta la solicitud rechazada
        /// </summary>
        [DataMember]
        public bool SolicitudRechazada { get; set; }

        /// <summary>
        /// Indica si la solicitud es de un racol
        /// </summary>
        [DataMember]
        public bool EsRacol { get; set; }

        /// <summary>
        /// Nuevos Valores de Ajuste al Giro,
        /// Valor, Servicio, Porte
        /// </summary>
        [DataMember]
        public TAPrecioDC AjustesGiroNvasTarifas { get; set; }

        /// <summary>
        /// Bandera que marca la impresion de una Solicitud
        /// </summary>
        [DataMember]
        public bool SeImprimioSolicitud { get; set; }
    }
}