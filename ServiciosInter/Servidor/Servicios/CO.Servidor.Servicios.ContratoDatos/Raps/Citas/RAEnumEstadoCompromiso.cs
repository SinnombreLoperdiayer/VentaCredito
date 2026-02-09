using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://www.controllogis.com")]
    public enum RAEnumEstadoCompromiso
    {
        [EnumMember]
        Abierto = 0,

        [EnumMember]
        Cerrado = 1
    }
}
