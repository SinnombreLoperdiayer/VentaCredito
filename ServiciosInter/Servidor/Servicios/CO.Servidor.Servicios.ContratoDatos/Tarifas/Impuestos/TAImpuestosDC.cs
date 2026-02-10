using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase para configuración y consulta de impuestos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAImpuestosDC : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna el identificador del impuesto
    /// </summary>
    [DataMember]
    public short Identificador { get; set; }

    /// <summary>
    /// Retorna a asigna la descripción del impuesto
    /// </summary>
    [DataMember]
    [Filtrable("IMP_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Descripcion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("IMP_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescImpuesto")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(25, MinimumLength = 3, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Retorna o asigna el valor del impuesto asignado al servicio
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("IMP_Valor")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "TooltipValorImpuesto")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal Valor { get; set; }

    [DataMember]
    public TACuentaExternaDC CuentaExterna { get; set; }

    [DataMember]
    public bool Asignado { get; set; }

    [DataMember]
    public bool Actual { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "TooltipValorImpuesto")]
    public decimal LiquidacionImpuesto { get; set; }
  }
}