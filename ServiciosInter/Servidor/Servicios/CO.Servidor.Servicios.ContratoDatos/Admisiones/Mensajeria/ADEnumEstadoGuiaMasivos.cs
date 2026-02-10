using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum ADEnumEstadoGuiaMasivos : short
    {
        [EnumMember]
        SinEstado = 0,

        [EnumMember]
        EnZona = 1,

        [EnumMember]
        Devuelto = 2,

        [EnumMember]
        Entregado = 3,

        [EnumMember]
        Intento_Etrega = 4
    }
}