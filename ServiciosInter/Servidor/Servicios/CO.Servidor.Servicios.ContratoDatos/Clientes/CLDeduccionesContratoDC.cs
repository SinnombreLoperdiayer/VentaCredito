using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de las deducciones por contrato
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLDeduccionesContratoDC : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("DEF_IdDeduccion")]
    [Filtrable("DEF_IdDeduccion", "Id:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id", Description = "TooltipIdDeduccion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdDeduccion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdContrato { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_Descripcion")]
    [Filtrable("DEF_Descripcion", "Descripción:", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(150, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescripcionDeduccion")]
    public string DescripcionDeduccion { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_ValorFijo")]
    [Filtrable("DEF_ValorFijo", "Valor fijo:", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorFijo", Description = "ToolTipValorFijo")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal ValorFijo { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_TarifaPorcentual")]
    [Filtrable("DEF_TarifaPorcentual", "Porcentaje:", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TarifaPorcentual", Description = "TooltipTarifaPorcentual")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal TarifaPorcentual { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_ValorPorMonto")]
    [Filtrable("DEF_ValorPorMonto", "Valor por monto:", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorMonto", Description = "TooltipValorMonto")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal ValorPorMonto { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}