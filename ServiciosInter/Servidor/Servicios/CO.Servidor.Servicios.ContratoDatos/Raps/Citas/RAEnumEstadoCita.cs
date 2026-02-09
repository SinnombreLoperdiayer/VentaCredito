using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://controllogis.com")]
    public enum RAEnumEstadoCita
    {
        [EnumMember]
        Activa = 1,
        [EnumMember]
        Inactiva = 2
    }
}
