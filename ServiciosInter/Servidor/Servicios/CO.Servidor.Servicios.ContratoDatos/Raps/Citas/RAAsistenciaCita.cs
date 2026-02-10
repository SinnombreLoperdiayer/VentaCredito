using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAAsistenciaCita
    {
        [DataMember]
        public long IdCita { get; set; }

        [DataMember]
        public long DocumentoAsistente { get; set; }

        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public bool Asistio { get; set; }

        [DataMember]
        public string Nombre { get; set; }
    }
}
