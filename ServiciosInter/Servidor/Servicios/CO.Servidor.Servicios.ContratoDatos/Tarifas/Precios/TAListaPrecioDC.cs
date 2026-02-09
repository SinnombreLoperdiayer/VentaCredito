using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
    /// <summary>
    /// Clase que contiene la información de las listas de precio
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class TAListaPrecioDC : DataContractBase
    {
        [DataMember]
        public int IdListaPrecio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNomListaPrecio")]
        [Filtrable("LIP_Nombre", "Nombre: ", COEnumTipoControlFiltro.TextBox)]
        [CamposOrdenamiento("LIP_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Nombre { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicio", Description = "TooltipFechaInicio")]
        [CamposOrdenamiento("LIP_Inicio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [DataType(DataType.DateTime)]
        public DateTime Inicio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaFin", Description = "TooltipFechaFin")]
        [CamposOrdenamiento("LIP_Inicio")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [DataType(DataType.DateTime)]
        public DateTime Fin { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstListaPrecio")]
        [CamposOrdenamiento("LIP_Estado")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(3, MinimumLength = 3, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Estado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        [CamposOrdenamiento("LIP_Estado")]
        public string EstadoDescripcion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TarifaPlena", Description = "TooltipTarifaPlena")]
        [CamposOrdenamiento("LIP_EsTarifaPlena")]
        public Boolean TarifaPlena { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ListaPrecioBase", Description = "ToolTipListaPrecioBase")]
        public int? IdListaPrecioBase { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Moneda", Description = "TooltipNombreMoneda")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string IdMoneda { get; set; }

        [DataMember]
        public bool EsListaCliente { get; set; }

        [DataMember]
        public IEnumerable<TAListaPrecioServicio> ServiciosAsignados { get; set; }

        [DataMember]
        public IEnumerable<TAServicioDC> ServiciosDisponibles { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public string PrimaSeguro { get; set; }
    }
}