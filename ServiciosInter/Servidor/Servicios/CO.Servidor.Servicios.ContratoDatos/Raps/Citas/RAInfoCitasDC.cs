using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://controllogis.com")]
    public class RAInfoCitasDC
    {
        [DataMember]
        public long IdParametrizacionCita { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public long IdCita { get; set; }
    }
}
