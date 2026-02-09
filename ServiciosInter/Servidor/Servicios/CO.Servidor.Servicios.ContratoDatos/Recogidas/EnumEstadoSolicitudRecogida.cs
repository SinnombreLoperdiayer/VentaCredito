using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum EnumEstadoSolicitudRecogida : int
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Creado = 1,
        [EnumMember]
        Reservado = 2,
        [EnumMember]
        ParaForzar = 3,
        [EnumMember]
        CanceladoPorElCliente = 4,
        [EnumMember]
        Realizada = 5,
        [EnumMember]
        Cancelada = 6,
        [EnumMember]
        Telemercadeo = 8,
        [EnumMember]
        Forzada = 9,
    }
}