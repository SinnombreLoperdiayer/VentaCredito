using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://controllogis.com")]
    public enum RAEnumTipoEliminacion
    {
        [EnumMember]
        Todas = 1,

        [EnumMember]
        Futuras = 2
    }
}
