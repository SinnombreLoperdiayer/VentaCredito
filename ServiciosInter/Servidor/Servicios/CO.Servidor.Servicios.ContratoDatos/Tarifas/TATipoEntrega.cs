using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
    /// <summary>
    /// Clase que contiene la información de los tipos de entrega
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class TATipoEntrega : DataContractBase
    {
        [DataMember]
        [Filtrable("TIE_IdTipoEntrega", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Codigo", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 3)]
        [CamposOrdenamiento("TIE_IdTipoEntrega")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo", Description = "TooltipIdTipoEntrega")]
        [StringLength(3, MinimumLength = 3, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string IdTipoEntrega { get; set; }

        [DataMember]
        [Filtrable("TIE_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Descripcion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
        [CamposOrdenamiento("TIE_Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDesTipoEntrega")]
        [StringLength(25, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}