using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Clase que contiene la informacion del manifiesto nacional
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONManifiestoOperacionNacional : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroManifiesto", Description = "NumeroManifiesto")]
        [Filtrable("MON_IdManifiestoOperacionNacio", "Número de manifiesto: ", COEnumTipoControlFiltro.TextBox)]
        public long IdManifiestoOperacionNacional { get; set; }

        /// <summary>
        /// Retorna o asigna el id racol de despacho
        /// </summary>
        [DataMember]
        public long IdRacolDespacho { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre del racol de despacho
        /// </summary>
        [DataMember]
        public string RacolDespacho { get; set; }

        /// <summary>
        /// Retorna o asigna la localidad origen del manifiesto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDespachoManifiesto", Description = "ToolTipCiudadDespachoManifiesto")]
        public PALocalidadDC LocalidadDespacho { get; set; }

        /// <summary>
        /// Retorna o asigna la localidad de destino del manifiesto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "CiudadDestino")]
        public PALocalidadDC LocalidadDestino { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RutaDespacho", Description = "ToolTipRutaDespacho")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public int IdRutaDespacho { get; set; }

        [DataMember]
        public int IdVehiculoDespacho { get; set; }

        [DataMember]
        [Filtrable("RUT_Nombre", "Nombre del operativo: ", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ruta", Description = "ToolTipRuta")]
        public string NombreRuta { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EmpresaTransportadora", Description = "ToolTipEmpresaTransportadora")]
        public int IdEmpresaTransportadora { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EmpresaTransportadora", Description = "ToolTipEmpresaTransportadora")]
        public string NombreEmpresaTransportadora { get; set; }

        [DataMember]
        public int IdTipoTransporte { get; set; }

        [DataMember]
        public string NombreTipoTransporte { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroManifiestoCarga", Description = "ToolTipNumManifiestoCarga")]
        public long NumeroManifiestoCarga { get; set; }

        [DataMember]
        public long IdCentroServiciosManifiesta { get; set; }

        [DataMember]
        public DateTime FechaCierre { get; set; }

        [DataMember]
        public bool EstaManifiestoCerrado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha", Description = "FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public int IdMedioTransporte { get; set; }

        [DataMember]
        public bool RutaGeneraManifiestoMinisterio { get; set; }

        [DataMember]
        public ONManifiestoOperacionNalTerrestre ManifiestoTerrestre { get; set; }

        /// <summary>
        /// Retorna o asigna los consolidados del manifiesto
        /// </summary>
        [DataMember]
        public ONConsolidado ConsolidadoManifiesto { get; set; }

        /// <summary>
        /// Retorna o asigna los envios sueltos del manifiesto
        /// </summary>
        [DataMember]
        public ONManifiestoGuia EnviosSueltosManifiesto { get; set; }

        /// <summary>
        /// Tipo de vehiculo utilizado en el manifiesto
        /// </summary>
        [DataMember]
        public short? IdTipoVehiculoManifiesto { get; set; }

        /// <summary>
        /// Es el id de la localidad de Origen de al Ruta
        /// </summary>
        [DataMember]
        public string IdLocalidadOrigenRuta { get; set; }

        [DataMember]
        public string DescripcionMedioTransporte { get; set; }

        /// <summary>
        /// Novedad a Grabar
        /// </summary>
        [DataMember]
        public ONNovedadEstacionRutaDC NovedadEstacionRutaAGrabar { get; set; }

        [DataMember]
        public DateTime? FechaSalidaSatrack { get; set; }

    }
}