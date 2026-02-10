
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RAEnumAccion
    {

        [EnumMember]
        None = 0,

        [EnumMember]
        Crear = 1,

        [EnumMember]
        Gestionar = 2,

        [EnumMember]
        Revisar = 3,

        [EnumMember]
        Escalar = 4,

        [EnumMember]
        Asignar = 5,

        [EnumMember]
        Cerrar = 6,

        [EnumMember]
        Vencer = 7
    }
}
