using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum SEEnumMensajesSeguridad
  {
    [EnumMember]
    ERROR,
    [EnumMember]
    EXITOSO,
    [EnumMember]
    USUARIONOEXISTE,
    [EnumMember]
    CLAVEINVALIDA,
    [EnumMember]
    DEBECAMBIARCLAVE,
    [EnumMember]
    BLOQUEADO,
    [EnumMember]
    DIRECTORIOACTIVONODISPONIBLE,
    [EnumMember]
    SERVIDORDOMINIOSINDNS,
    [EnumMember]
    CLAVEPORVENCER,
    [EnumMember]
    USUARIOSINCLAVE,
    [EnumMember]
    CLAVEINSEGURA,
    [EnumMember]
    USUARIONOSEPUDOCREAR,
    [EnumMember]
    NOSECREOELUSUARIO,
    [EnumMember]
    EMAILDUPLICADO,
    [EnumMember]
    NOMBREDEUSUARIODUPLICADO,
    [EnumMember]
    RESPUESTAINVALIDA,
    [EnumMember]
    EMAILINVALIDO,
    [EnumMember]
    PASSWORDINVALIDO,
    [EnumMember]
    PREGUNTAINVALIDA,
    [EnumMember]
    NOMBREDEUSUARIOINVALIDO,
    [EnumMember]
    ERRORDEPROVEEDORLDAP,
    [EnumMember]
    USUARIORECHAZADO,
    [EnumMember]
    ERRORCAMBIANDOPASSWORD
  }
}