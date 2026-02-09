using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de los tipos de envío
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATipoEnvio : DataContractBase
  {
    [DataMember]
    public short IdTipoEnvio { get; set; }

    [DataMember]
    [Filtrable("TEN_Nombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Nombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("TEN_Nombre")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombreTipoEnvio")]
    [StringLength(25, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Nombre { get; set; }

    [DataMember]
    [Filtrable("TEN_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Descripcion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 100)]
    [CamposOrdenamiento("TEN_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescripcionTipoEnvio")]
    [StringLength(100, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    [DataMember]
    [CamposOrdenamiento("TEN_PesoMinimo")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoMinimo", Description = "TooltipPesoMinimoTipoEnvio")]
    public decimal PesoMinimo { get; set; }

    [DataMember]
    [CamposOrdenamiento("TEN_PesoMaximo")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoMaximo", Description = "TooltipPesoMaximoTipoEnvio")]
    public decimal PesoMaximo { get; set; }

    [DataMember]
    [Filtrable("TEN_CodigoMinisterio", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "CodigoMinisterio", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("TEN_CodigoMinisterio")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoMinisterio", Description = "TooltipCodMinTipoEnvio")]
    public decimal CodigoMinisterio { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}