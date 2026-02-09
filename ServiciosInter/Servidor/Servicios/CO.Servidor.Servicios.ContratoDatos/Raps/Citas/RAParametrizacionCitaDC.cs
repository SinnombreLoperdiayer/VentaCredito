using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://controllogis.com")]
    public class RAParametrizacionCitaDC
    {
        [DataMember]
        public long IdParametrizacion { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public DateTime FechaFin { get; set; }

        [DataMember]
        public long IdPeriodoRepeticion { get; set; }

        [DataMember]
        public RAEnumEstadoCita IdEstado { get; set; }

    }
}
