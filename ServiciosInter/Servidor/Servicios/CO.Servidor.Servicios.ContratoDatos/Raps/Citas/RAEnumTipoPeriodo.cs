using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RAEnumTipoPeriodo
    {
        [EnumMember]
        Manual = 0,

        [EnumMember]
        Diario = 1,

        [EnumMember]
        Semanal = 2,

        [EnumMember]
        Quincenal = 4,

        [EnumMember]
        Mensual = 5,

        [EnumMember]
        BiMensual = 6,

        [EnumMember]
        TriMestral = 7,

        [EnumMember]
        Semestral = 8,

        [EnumMember]
        Anual = 9,

        [EnumMember]
        Ninguno = 10

    }
}
