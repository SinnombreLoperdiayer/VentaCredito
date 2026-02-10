using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de los requisitos por factura
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLFacturaRequisitosDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdFactura { get; set; }

    [DataMember]
    [CamposOrdenamiento("RRF_IdRequisito")]
    [Filtrable("RRF_IdRequisito", "Id:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public short IdRequisito { get; set; }

    [DataMember]
    [CamposOrdenamiento("RRF_Descripcion")]
    [Filtrable("RRF_Descripcion", "Descripción:", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(250, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}