using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCNovedadCambioTipoServicio : ADNovedadGuiaDC
  {
    [DataMember]
    public decimal NuevoValorTotal { get; set; }

    [DataMember]
    public decimal NuevoValorTransporte { get; set; }

    [DataMember]
    public decimal NuevoValorPrima { get; set; }

    [DataMember]
    public int IdServicio { get; set; }
  }
}