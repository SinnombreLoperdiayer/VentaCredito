using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    /// <summary>
    /// Clase que contiene la informacion de los centros de servicios
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUCentroServiciosDC : DataContractBase
    {
        public event EventHandler OnCambioMunicipio;

        public event EventHandler OnCambioTipoCentroServicio;

        public event EventHandler OnCambioTipoAgenciaCentroServicio;

        public event EventHandler OnCambioTipoPropiedadCentroServicio;

        public event EventHandler OnCambioidTerritorialCentroServicio;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCentroServicio", Description = "TooltipIdCentroServicio")]
        [Filtrable("CES_IdCentroServicios", "Id Centro Servicio:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 100)]
        [CamposOrdenamiento("CES_IdCentroServicios")]
        public long IdCentroServicio { get; set; }

        private string tipo;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Tipo", Description = "TipoCentroServicio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("CES_Tipo")]
        public string Tipo
        {
            get { return tipo; }
            set
            {
                tipo = value;
                if (OnCambioTipoCentroServicio != null)
                    OnCambioTipoCentroServicio(null, null);
            }
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Tipo", Description = "TipoCentroServicio")]
        public string TipoSubtipo { get; set; }

        [DataMember]
        public string TipoOriginal { get; set; }

        private string idTipoAgencia;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoAgencia", Description = "ToolTipTipoAgencia")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdTipoAgencia
        {
            get { return idTipoAgencia; }
            set
            {
                idTipoAgencia = value;
                if (OnCambioTipoAgenciaCentroServicio != null)
                    OnCambioTipoAgenciaCentroServicio(null, null);
            }
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ColApoyo", Description = "ToolTipColApoyo")]
        public long? IdColRacolApoyo { get; set; }

        [DataMember]
        public string DigitoVerificacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdPropietario", Description = "TooltipIdPropietario")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdPropietario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Propietario", Description = "ToolTipPropietario")]
        public string NombrePropietario { get; set; }

        [DataMember]
        public string IdentificacionPropietario { get; set; }

        [DataMember]
        public long IdRepresentanteLegalPropietario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombreCentroServicio")]
        [Filtrable("CES_Nombre", "Nombre Centro Servicio:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 250)]
        [CamposOrdenamiento("CES_Nombre")]
        public string Nombre { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("CES_Telefono1")]
        public string Telefono1 { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono2", Description = "TooltipTelefono2")]
        public string Telefono2 { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fax", Description = "TooltipFax")]
        public string Fax { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("CES_Direccion")]
        public string Direccion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Barrio", Description = "TooltipBarrio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Barrio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Zona", Description = "TooltipZona")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdZona { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Zona", Description = "TooltipZona")]
        public string NombreZona { get; set; }

        [DataMember]
        public bool HabilitadoColApoyo { get; set; }

        private string idMunicipio;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdMunicipio
        {
            get { return idMunicipio; }
            set
            {
                if (value != idMunicipio)
                {
                    idMunicipio = value;

                    //if (OnCambioMunicipio != null)
                    //  OnCambioMunicipio(this, null);
                }
            }
        }

        [DataMember]
        public string IdDepto { get; set; }

        [DataMember]
        public string NombreDepto { get; set; }

        [DataMember]
        public string IdPais { get; set; }

        [DataMember]
        public string NombrePais { get; set; }

        [DataMember]
        public PALocalidadDC PaisCiudad { get; set; }

        private PALocalidadDC ciudadUbicacion;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public PALocalidadDC CiudadUbicacion
        {
            get { return ciudadUbicacion; }
            set
            {
                if (value != ciudadUbicacion)
                {
                    ciudadUbicacion = value;
                    IdMunicipio = ciudadUbicacion.IdLocalidad;
                    if (OnCambioMunicipio != null && ciudadUbicacion != null)
                        OnCambioMunicipio(this, null);
                }
            }
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
        public string NombreMunicipio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("CES_Estado")]
        public string Estado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
        [CamposOrdenamiento("CES_Estado")]
        public bool EstadoBool { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ResponsableServicio", Description = "TooltipresponsebleServicio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public long IdPersonaResponsable { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "GiroPorTrasmitir")]
        public long Cantidad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ResponsableServicio", Description = "TooltipresponsebleServicio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string NombrePersonaResponsable { get; set; }

        [DataMember]
        public string IdentificacionPersonaResponsable { get; set; }

        
        [DataMember]
        public string CelularPersonaResponsable { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Latitud", Description = "ToolTipLatitud")]
        [Range(-90.9999999999, 90.9999999999, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "RangoLatitud")]
        public decimal? Latitud { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Longitud", Description = "ToolTipLongitud")]
        [Range(-180.9999999999, 180.9999999999, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "RangoLongitud")]
        public decimal? Longitud { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Email", Description = "Tooltipemail")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Email { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Sistematizado", Description = "ToolTipSistematizado")]
        public bool Sistematizado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CentroCostos", Description = "ToolTipCentroCostos")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdCentroCostos { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSociedad", Description = "TooltipTipoSociedad")]
        public int TipoSociedad { get; set; }

        private int idTipoPropiedad;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoPropiedad", Description = "TooltipTipoPropiedad")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdTipoPropiedad
        {
            get { return idTipoPropiedad; }
            set
            {
                idTipoPropiedad = value;
                if (OnCambioTipoPropiedadCentroServicio != null)
                    OnCambioTipoPropiedadCentroServicio(null, null);
            }
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoMaximo", Description = "TooltipPesoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal PesoMaximo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "VolumenMaximo", Description = "ToolTipVolumenMaximo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal VolumenMaximo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AdmiteFormaPagoAlCobro", Description = "ToolTipAdmiteFormaPagoAlCobro")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool AdmiteFormaPagoAlCobro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "VendePrepago", Description = "ToolTipVendePrepago")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool VendePrepago { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CentroServicioAdministrador", Description = "ToolTipCentroServicioAdministrador")]
        public string CentroServiciosAdministrador { get; set; }

        [DataMember]
        public IList<PUHorariosCentroServicios> HorariosCentroServicios { get; set; }

        [DataMember]
        public IList<PUHorarioRecogidaCentroSvcDC> HorariosRecogidasCentroServicio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "ToolTipDescripcionRacol")]
        public string DescripcionRacol { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CasaMatriz", Description = "ToolTipCasaMatriz")]
        public int IdCasaMatriz { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoNovaSoft", Description = "ToolTipCodigoNovaSoft")]
        public string CodRacolExterno { get; set; }
        
        [DataMember]
        public ObservableCollection<PUArchivoCentroServicios> ArchivosCentroServicios { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ArchivosCentroServicios", Description = "ToolTipArchivosCentroServicios")]
        public string ArchivosRequeridos { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PagaGiros", Description = "PagaGiros")]
        public bool PagaGiros { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RecibeGiros", Description = "RecibeGiros")]
        public bool RecibeGiros { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TopeMaximoGiros", Description = "TooltipTopeMaximoGiros")]
        public decimal TopeMaximoGiros { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TopeMaximoPagos", Description = "TooltipTopeMaximoPagos")]
        public decimal TopeMaximoPagos { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoBodega", Description = "ToolTipCodigoBodega")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string CodigoBodega { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ClasificadorCanalVenta", Description = "ToolTipClasificadorCanalVenta")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdClasificadorCanalVenta { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoPostal", Description = "ToolTipCodigoPostal")]
        [StringLength(12, MinimumLength = 6, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "LongitudCadena")]
        public string CodigoPostal { get; set; }

        //[DataMember]
        //public string CodigoPostal { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseInicial", Description = "ToolTipBaseInicial")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal BaseInicialCaja { get; set; }

        [DataMember]
        public DateTime Fecha { get; set; }

        [DataMember]
        public long IdRegionalAdministrativa { get; set; }

        [DataMember]
        public bool Habilitado { get; set; }

        [DataMember]
        public PAPersonaExterna PersonaResponsable { get; set; }

        [DataMember]
        public PUClasificacionPorIngresosDC ClasificacionPorIngresos { get; set; }

        [DataMember]
        public PUObservacionCentroServicioDC ObservacionCentroServicio { get; set; }

        [DataMember]
        public PUAgenciaDeRacolDC infoResponsable { get; set; }

        [DataMember]
        public string AgenciasBarrio
        {
            get
            {
                return string.Format("{0} | {1}", Nombre, Barrio);
            }
            set
            {
                agenciasBarrio = value;
            }
        }

        private string agenciasBarrio;
        public bool? AplicaPAM;

        /// <summary>
        /// es el codigo de servicios postales nacionales
        /// suministrado por 4-72, por el cual ellos tienen
        /// identificados nuestros puntos de recepción de puntos y agencias
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoSPN", Description = "ToolTipCodigoSPN")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string CodigoSPN { get; set; }
        
        [DataMember]
        public string NombreCodigo { get; set; }

        public string NoComprobante { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ReclameEnOficina", Description = "ToolTipReclameEnOficina")]
        public bool ReclameEnOficina { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreAMostrar", Description = "ToolTipNombreAMostrar")]
        public string NombreAMostrar { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo472", Description = "TooltipCodigo472")]
        public string Codigo472 { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaApertura", Description = "ToolTipFechaApertura")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public DateTime? FechaApertura { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCierre", Description = "TooltipFechaCierre2")]        
        public DateTime? FechaCierre { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Operacional", Description = "ToolTipOperacional")]
        public bool Operacional { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoPAM", Description = "ToolTipPuntoPAM")]
        public bool? PuntoPAM { get; set; }

        [DataMember]
        [Display(Name = "Territorial")]
        public int? IdTerritorial { get; set; }


        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public int IdTipoCiudad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Biometrico", Description = "ToolTipBiometrico")]
        public bool Biometrico { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Recaudo", Description = "ToolTipRecaudo")]
        public bool Recaudo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MisPendientes", Description = "ToolMisPendientes")]
        public bool MisPendientes { get; set; }

    }
}