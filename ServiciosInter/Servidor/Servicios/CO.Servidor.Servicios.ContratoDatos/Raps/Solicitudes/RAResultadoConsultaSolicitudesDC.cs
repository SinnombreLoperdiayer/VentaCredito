using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAResultadoConsultaSolicitudesDC
    {
        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public int IdCargoSolicita { get; set; }

        [DataMember]
        public int IdCargoResponsable { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public DateTime FechaVencimiento { get; set; }

        [DataMember]
        public int IdEstado { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public int IdResponsableInicial { get; set; }

        [DataMember]
        public DateTime FechaVencimientoInicial { get; set; }

        [DataMember]
        public long IdSolicitudPadre { get; set; }

        [DataMember]
        public string Solicita { get; set; }

        [DataMember]
        public string Proceso { get; set; }

        [DataMember]
        public string Tipo { get; set; }

        [DataMember]
        public long Registro { get; set; }

        [DataMember]
        public int TotalPaginas { get; set; }
    }
}
