using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    /// <summary>
    /// Clase que contiene la informacion los vehiculo
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class POVehiculo : DataContractBase
    {

        public event EventHandler OnCambioMarcaVehiculo;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Placa", Description = "ToolTipPlaca")]
        public int IdVehiculo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoVehiculo", Description = "ToolTipTipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdTipoVehiculo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cilindraje", Description = "ToolTipCilindraje")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int Cilindraje { get; set; }

        [DataMember]
        [StringLength(6, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [CamposOrdenamiento("VEH_Placa")]
        [Filtrable("VEH_Placa", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Placa", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 6)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Placa", Description = "ToolTipPlaca")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Placa { get; set; }

        private int idMarcaVehiculo;
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Marca", Description = "ToolTipMarcaVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdMarcaVehiculo
        {
            get { return idMarcaVehiculo; }
            set
            {
                idMarcaVehiculo = value;
                if (OnCambioMarcaVehiculo != null)
                    OnCambioMarcaVehiculo(null, null);
            }
        }

        [DataMember]
        [Filtrable("MVH_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Marca", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
        [CamposOrdenamiento("MVH_Descripcion")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Marca", Description = "ToolTipMarcaVehiculo")]
        public string NombreMarcaVehiculo { get; set; }

        [DataMember]
        [CamposOrdenamiento("VEH_Modelo")]
        [Filtrable("VEH_Modelo", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Modelo", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 4)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modelo", Description = "ToolTipModeloVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal Modelo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Color", Description = "ToolTipColorVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdColorVehiculo { get; set; }

        [DataMember]
        [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroMotor", Description = "ToolTipNumeroMotor")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string NumeroMotor { get; set; }

        [DataMember]
        [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroSerie", Description = "ToolTipNumeroSerie")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string NumeroSerie { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadExpedicionPlaca", Description = "ToolTipCiudadExpedicionPlaca")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public PALocalidadDC CiudadExpedicionPlaca { get; set; }

        [DataMember]
        public PALocalidadDC PaisCiudad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadUbicacion", Description = "ToolTipCiudadUbicacion")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public PALocalidadDC CiudadUbicacion { get; set; }

        [DataMember]
        [CamposOrdenamiento("LOC_NombreUBI")]
        [Filtrable("LOC_NombreUBI", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "CiudadUbicacion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 100)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadUbicacion", Description = "ToolTipCiudadUbicacion")]
        public string NombreLocalidadUbicacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Capacidad", Description = "ToolTipCapacidadVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal Capacidad { get; set; }

        [DataMember]
        public string TipoVehiculo { get; set; }


        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "ToolTipTipoContratoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdTipoContrato { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "ToolTipEstadoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Estado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "ToolTipEstadoVehiculo")]
        public bool EstadoBool { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Linea", Description = "ToolTipLineaVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdLineaVehiculo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ModeloRepotenciado", Description = "ToolTipModeloRepotenciado")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal ModeloRepotenciado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoCarroceria", Description = "ToolTipTipoCarroceria")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdTipoCarroceria { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoCombustible2", Description = "ToolTipTipoCombustible2")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int? IdTipoCombustible { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoBrutoTaraVacio", Description = "ToolTipPesoBrutoTaraVacio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int PesoBrutoTaraVacio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegistroNacionalCarga", Description = "ToolTipRegistroNacionalCarga")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int RegistroNacionalCarga { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TablaConfiguracionVehiculo", Description = "ToolTipTablaConfiguracionVehiculo")]
        public int IdTablaConfiguracionCarro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TablaConfiguracionVehiculo", Description = "ToolTipTablaConfiguracionVehiculo")]
        public string DescripcionTablaConfiguracionCarro { get; set; }

        [DataMember]
        public POPropietarioVehiculo PropietarioVehiculo { get; set; }

        [DataMember]
        public POTenedorVehiculo TenedorVehiculo { get; set; }

        [DataMember]
        public POPolizaSeguroVehiculo PolizaSeguroSoat { get; set; }

        [DataMember]
        public POPolizaSeguroVehiculo PolizaSeguroTodoRiesgo { get; set; }

        [DataMember]
        public PORevisionMecanica RevisionMecanica { get; set; }

        [DataMember]
        public List<POMensajero> Mensajeros { get; set; }

        [DataMember]
        public List<PURegionalAdministrativa> Racol { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        [CamposOrdenamiento("PEE_Identificacion")]
        [Filtrable("PEE_Identificacion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "IdentificacionPropietario", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
        public string IdentificacionPropietario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombrePropietario", Description = "TooltipPrimerNombre")]
        [CamposOrdenamiento("PEE_PrimerNombre")]
        [Filtrable("PEE_PrimerNombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "NombrePropietario", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
        public string NombrePropietario { get; set; }

        public event EventHandler OnCiudadTenedorCambio;
        public event EventHandler OnCiudadPropietarioCambio;

        private PALocalidadDC ciudadTenedor;

        [IgnoreDataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudadTenedor")]
        public PALocalidadDC CiudadTenedor
        {
            get { return ciudadTenedor; }
            set
            {
                ciudadTenedor = value;
                if (OnCiudadTenedorCambio != null)
                    OnCiudadTenedorCambio(null, null);
            }
        }

        private PALocalidadDC ciudadPropietario;

        [IgnoreDataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudadPropietario")]
        public PALocalidadDC CiudadPropietario
        {
            get { return ciudadPropietario; }
            set
            {
                ciudadPropietario = value;
                if (OnCiudadPropietarioCambio != null)
                {
                    OnCiudadPropietarioCambio(null, null);
                }
            }
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ReportarSatrack", Description = "ToolTipReportarSatrack")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool ReportarSatrack { get; set; }
    }
}