using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  /// <summary>
  /// Contiene información de un motivo por el cual no se usó una bolsa de seguridad
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADMotivoNoUsoBolsaSeguridad
  {
    /// <summary>
    /// Identificador del motivo de no uso de bolsa de seguridad
    /// </summary>
    [DataMember]
    public short Id { get; set; }

    /// <summary>
    /// Descripción del motivo de no uso de bolsa de seguridad
    /// </summary>
    [DataMember]
    public string Descripcion { get; set; }
  }
}