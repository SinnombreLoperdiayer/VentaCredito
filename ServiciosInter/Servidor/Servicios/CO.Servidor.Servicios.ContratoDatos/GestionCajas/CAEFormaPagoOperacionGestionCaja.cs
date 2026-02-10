using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Manejo de las formas de pago como se envían dinero entre las cajas de gestión de cajas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEFormaPagoOperacionGestionCaja
  {
    /// <summary>
    /// Envío en cheque
    /// </summary>
    [EnumMember]
    CHEQUE = 0,

    /// <summary>
    /// Envío en consignación
    /// </summary>
    [EnumMember]
    CONSIGNACION,

    /// <summary>
    /// Envío en efectivo
    /// </summary>
    [EnumMember]
    EFECTIVO
  }
}