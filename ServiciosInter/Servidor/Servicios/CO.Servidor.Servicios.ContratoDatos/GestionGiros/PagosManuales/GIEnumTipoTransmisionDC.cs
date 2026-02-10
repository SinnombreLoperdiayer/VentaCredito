using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum GIEnumTipoTransmisionDC
  {
    /// <summary>
    /// Intento Transmitido
    /// </summary>
    [EnumMember]
    TRA,

    /// <summary>
    /// Intento Fallido
    /// </summary>
    [EnumMember]
    FAL
  }
}