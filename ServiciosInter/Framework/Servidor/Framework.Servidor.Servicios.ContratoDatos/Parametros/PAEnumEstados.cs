using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum PAEnumEstados
  {
    /// <summary>
    /// Activo
    /// </summary>
    [EnumMember]
    ACT,

    /// <summary>
    /// Inactivo
    /// </summary>
    [EnumMember]
    INA,

    /// <summary>
    /// Anulado
    /// </summary>
    [EnumMember]
    ANU,
  }
}