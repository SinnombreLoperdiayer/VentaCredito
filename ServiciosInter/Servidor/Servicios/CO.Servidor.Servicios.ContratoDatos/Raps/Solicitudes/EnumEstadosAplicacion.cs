using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum EnumEstadosAplicacion
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        PorResolver = 1,

        [EnumMember]
        Rechazadas = 2,

        [EnumMember]
        Resueltas = 3,

        [EnumMember]
        VencidasSinRevision = 4,

        [EnumMember]
        Vencidas = 5,

        [EnumMember]
        Vigentes = 6,

        [EnumMember]
        Canceladas = 7,

        [EnumMember]
        CerradasExitosas = 8,

        [EnumMember]
        CerradasVencidas = 9,
    }
}
