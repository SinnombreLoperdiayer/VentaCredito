using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADGuiaFormaPago
  {
    [DataMember]
    public short IdFormaPago { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "ToolTipValorFormaPago")]
    public decimal Valor { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago", Description = "FormaPago")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Es el numero asociado a la forma de pago.
    /// Pricipalmente para el PinPrepago y
    /// Cheque.
    /// </summary>
    /// <value>
    /// numero asociado
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroAsociadoFormaPago", Description = "ToolTipNumeroAsociado")]
    public string NumeroAsociadoFormaPago { get; set; }
  }
}