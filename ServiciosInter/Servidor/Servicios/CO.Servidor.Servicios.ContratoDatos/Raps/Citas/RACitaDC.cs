using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RACitaDC
    {
        [DataMember]
        public long IdCita { get; set; }

        [DataMember]
        public DateTime FechaInicioCita { get; set; }

        [DataMember]
        public DateTime FechaFinCita { get; set; }

        [DataMember]
        public RAEnumEstadoCita IdEstado { get; set; }

        [DataMember]
        public long IdParametrizacion { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string Titulo { get; set; }

        [DataMember]
        public List<RAIntegranteCitaDC> Integrantes { get; set; }

        [DataMember]
        public RAPeriodoRepeticionDC PeriodoRepeticion { get; set; }

        [DataMember]
        public RAParametrizacionCitaDC ParametrizacionCita { get; set; }

        [DataMember]
        public List<RANotificacionCitaDC> Notificacion { get; set; }

        [DataMember]
        public RAEnumTipoEliminacion IdTipoEliminacion { get; set; }

        [DataMember]
        public string OrdenDia { get; set; }

        [DataMember]
        public string Desarrollo { get; set; }

        [DataMember]
        public List<DateTime> FechasNoSecuenciales { get; set; }

        [DataMember]
        public List<RAAdjuntoDC> Adjuntos { get; set; }

        [DataMember]
        public string LugarCita { get; set; }

        [DataMember]
        public List<RACompromisoDC> Compromisos { get; set; }

        [DataMember]

        public List<RAAsistenciaCita> Asistencia { get; set; }
    }
}
