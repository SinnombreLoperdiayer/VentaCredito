using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
    /// <summary>
    /// Clase con el DataContract del cliente
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CLClientesDC : DataContractBase
    {
        [DataMember]
        [CamposOrdenamiento("CLI_IdCliente")]
        [Filtrable("CLI_IdCliente", "Codigo:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo")]
        public int IdCliente { get; set; }

        [DataMember]
        [CamposOrdenamiento("CLI_Nit")]
        [Filtrable("CLI_Nit", "Nit:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(20, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "NitNoValido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nit", Description = "TooltipNit")]
        public string Nit { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DigitoVerificacion", Description = "TooltipDigitoVerificacion")]
        [StringLength(3, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "DVError")]
        public string DigitoVerificacion { get; set; }

        [DataMember]
        [CamposOrdenamiento("CLI_RazonSocial")]
        [Filtrable("CLI_RazonSocial", "Razon Social:", COEnumTipoControlFiltro.TextBox)]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RazonSocial", Description = "TooltipRazonSocial")]
        [StringLength(250, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string RazonSocial { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSociedad", Description = "TooltipTipoSociedad")]
        public short TipoSociedad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSociedad", Description = "TooltipTipoSociedad")]
        public string NombreTipoSociedad { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public long IdRepresentanteLegal { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaConstitucion", Description = "TooltipFechaConstitucion")]
        public DateTime FechaConstitucion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        public string Localidad { get; set; }

        [DataMember]
        [CamposOrdenamiento("LOC_Nombre")]
        [Filtrable("LOC_Nombre", "Ciudad:", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
        public string NombreLocalidad { get; set; }

        [DataMember]
        public string IdDepto { get; set; }

        [DataMember]
        public string NombreDepto { get; set; }

        [DataMember]
        public string IdPais { get; set; }

        [DataMember]
        public string NombrePais { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public string NombreCiudad { get; set; }

        [DataMember]
        public string CodigoPostal { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
        public string NombreMunicipio { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreGerente", Description = "TooltipNombreGerente")]
        [StringLength(150, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NombreGerente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Direccion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [StringLength(20, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(ConstantesFramework.REGEX_VALIDACION_TELEFONO, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "TelefonoNoValido")]
        public string Telefono { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fax", Description = "TooltipFax")]
        [StringLength(20, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        [RegularExpression(ConstantesFramework.REGEX_VALIDACION_TELEFONO, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "FaxInvalido")]
        public string Fax { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegimenContributivo", Description = "TooltipRegimenContributivo")]
        public short RegimenContributivo { get; set; }

        [DataMember]
        public string NombreRegimenContributivo { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SegmentoMercado", Description = "TooltipSegmentoMercado")]
        public short SegmentoMercado { get; set; }

        [DataMember]
        public string NombreSegmentoMercado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaVinculacion", Description = "TooltipFechaVinculacion")]
        public DateTime FechaVinculacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TopeMaximoCredito", Description = "TooltipTopeMaximoCredito")]
        public decimal TopeMaximoCredito { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
        public string NombreEstado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
        [StringLength(3, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Estado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RepresentanteLegal", Description = "TooltipRepresentanteLegal")]
        public string NombreRepresentanteLegal { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSectorCliente", Description = "ToolTipTipoSectorCliente")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdTipoSectorCliente { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public CLEstadosClienteDC EstadoCliente { get; set; }

        [DataMember]
        public ObservableCollection<CLArchivosDC> ArchivosCliente { get; set; }

        [DataMember]
        public int ListaPrecioVigente { get; set; }

        [DataMember]
        public List<CLContratosDC> Contratos { get; set; }

        /// <summary>
        /// Contrato seleccionado para realizar la venta del giro
        /// </summary>
        [DataMember]
        public CLContratosDC Contrato { get; set; }

        /// <summary>
        /// Contiene la lista de sucursales asociados al cliente crédito
        /// </summary>
        [DataMember]
        public List<CLSucursalDC> Sucursales { get; set; }

        [DataMember]
        public List<CLConvenioLocalidadDC> LocalidadesConvenio { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ActividadEconomica", Description = "TooltipActividadEconomica")]
        public PATipoActEconomica ActividadEconomica { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ConvenioNacional", Description = "TooltipConvenioNacional")]
        public bool ClienteConvenio { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(Name = "Tipo Cliente Credito", Description = "Asigna el tipo de cliente credito")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdTipoClienteCredito { get; set; }
    }
}