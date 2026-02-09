using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAResumenCierreCajaPrincipalFormaPagoDC
  {
    [DataMember]
    public long IdApertura { get; set; }

    [DataMember]
    public long IdCierreAsociado { get; set; }

    [DataMember]
    public int IdFormaPago { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago")]
    public string NombreFormaPago { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
    public int Cantidad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ingreso", Description = "ToolTipIngreso")]
    public decimal TotalValorIngreso { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Egreso", Description = "ToolTipEgreso")]
    public decimal TotalValorEgreso { get; set; }
  }
}