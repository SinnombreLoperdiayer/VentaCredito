using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de los descuentos por factura
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLFacturaDescuentoDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdFactura { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_IdDescuento")]
    [Filtrable("DEF_IdDescuento", "Id:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public short IdDescuento { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_Valor")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal Valor { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_Motivo")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(150, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Motivo { get; set; }

    [DataMember]
    [Filtrable("DEF_FechaAplicacion", "Fecha:", COEnumTipoControlFiltro.DatePicker)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public DateTime FechaAplicacion { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_FechaAplicacion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ano")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int Ano { get; set; }

    [DataMember]
    [CamposOrdenamiento("DEF_FechaAplicacion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Mes")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public short Mes { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}