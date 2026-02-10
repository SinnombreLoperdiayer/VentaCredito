using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos
{
  /// <summary>
  /// Clase para desplegar la cantidad de pagos y el total de una agencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PGTotalPagosDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CantidadPagos", Description = "ToolTipCantidadPagos")]
    public int CantidadPagos { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ValorTotalPagos", Description = "ToolTipValorTotalPagos")]
    public decimal SumatoriaPagos { get; set; }
  }
}