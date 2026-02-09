using System.ComponentModel;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LICertificacionWebDC : short
    {
        [EnumMember]
        SinCertificacion = 0,
        [EnumMember]
        Entrega = 1,
        [EnumMember]
        Devolucion = 2
    }
}
