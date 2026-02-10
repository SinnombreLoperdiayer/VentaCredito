
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RAEnumEstados
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Creada = 1,

        [EnumMember]
        Respuesta = 2,

        [EnumMember]
        Revisado = 3,

        [EnumMember]
        Escalado = 4,

        [EnumMember]
        Asignado = 5,

        [EnumMember]
        Cerrado = 6,

        [EnumMember]
        Rechazado = 7,

        [EnumMember]
        Cancelado = 8,

        [EnumMember]
        Vencido = 9,

        [EnumMember]
        Reasignado = 10
    }
}
