using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = ("http://contrologis.com"))]
    public enum RAEnumTipoAgrupamiento
    {
        [EnumMember]
        Pendientes = 0,

        [EnumMember]
        Resueltos = 1,

        [EnumMember]
        Vencidos = 2
    }
}
