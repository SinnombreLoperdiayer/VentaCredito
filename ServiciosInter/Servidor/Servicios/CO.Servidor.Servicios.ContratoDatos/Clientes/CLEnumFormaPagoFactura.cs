using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Enumeración para manejo de formas de pago para configuración de fecturas de cliente
  /// </summary>
  /// <remarks>Esta forma de pago no tiene relación con las formas de pago de tarifas</remarks>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CLEnumFormaPagoFactura : short
  {
    /// <summary>
    /// Forma pago en efectivo
    /// </summary>
    [EnumMember]
    EFECTIVO = 0,

    /// <summary>
    /// Forma pago en cheque
    /// </summary>
    [EnumMember]
    CHEQUE,

    /// <summary>
    /// Forma pago en transferencia
    /// </summary>
    [EnumMember]
    TRANSFERENCIA
  }
}
