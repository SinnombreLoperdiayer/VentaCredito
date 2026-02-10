using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAHoraEscalarDC
    {
        [DataMember]
        public int IdHoraEscalar { get; set; }

        [DataMember]
        public int HoraEscalar { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    }
}
