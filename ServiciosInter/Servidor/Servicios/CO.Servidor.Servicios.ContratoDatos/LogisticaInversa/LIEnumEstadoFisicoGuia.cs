using System;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum LIEnumEstadoFisicoGuia
  {
    [EnumMember]
    BUENO,
    [EnumMember]
    REGULAR,
    [EnumMember]
    MALO
  }
}