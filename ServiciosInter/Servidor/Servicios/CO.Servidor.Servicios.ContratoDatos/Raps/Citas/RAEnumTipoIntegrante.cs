using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RAEnumTipoIntegrante
    {
        [EnumMember]
        Presidente = 1,

        [EnumMember]
        Secretario = 2,

        [EnumMember]
        Asistente = 3,

        [EnumMember]
        Invitado = 4,

        [EnumMember]
        Moderador = 5

    }
}
