using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RGEnumTipoRecogidaDC
    {
        [EnumMember]   
        NoDefinida = 0,
        [EnumMember]   
        FijaCliente = 1,
        [EnumMember]   
        Esporadica = 2 ,
        [EnumMember]
        FijaCentroServicio = 3
    }
}