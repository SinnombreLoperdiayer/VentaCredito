using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RANotificacionCitaDC
    {
        [DataMember]
        public long IdNotificacion { get; set; }

        [DataMember]
        public long IdCita { get; set; }

        [DataMember]
        public long IdPeriodoNotificacion { get; set; }

        [DataMember]
        public int TiempoRecordatorio { get; set; }

        [DataMember]
        public DateTime HoraNotificacion { get; set; }

    }
}
