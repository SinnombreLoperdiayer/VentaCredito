using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Tipo de falla para su asignación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum ASETipoFalla
  {
    /// <summary>
    /// Automática (AUT)
    /// </summary>
    [EnumMember]
    Automática,

    /// <summary>
    /// Manual (MAN)
    /// </summary>
    [EnumMember]
    Manual
  }
}