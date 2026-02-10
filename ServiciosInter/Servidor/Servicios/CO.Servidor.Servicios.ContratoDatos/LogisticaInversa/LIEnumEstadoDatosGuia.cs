using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum LIEnumEstadoDatosGuia : short
  {
    [EnumMember]
    LEGIBLE,
    [EnumMember]
    ILEGIBLE,
    [EnumMember]
    INCOMPLETA
  }
}