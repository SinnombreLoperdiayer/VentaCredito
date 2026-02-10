using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAFormatoCalendarioDC
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public DateTime Start { get; set; }

        [DataMember]
        public DateTime End { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int Estado { get; set; }
    }
}
