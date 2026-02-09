using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAAdjuntoDC
    {
        [DataMember]
        public int IdAdjunto { get; set; }

        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public string NombreArchivo { get; set; }

        [DataMember]
        public long IdGestion { get; set; }

        [DataMember]
        public Decimal Tamaño { get; set; }

        [DataMember]
        public string Adjunto { get; set; }

        [DataMember]
        public string Extension { get; set; }

        [DataMember]
        public string UbicacionNombre { get; set; }

        [DataMember]
        public string AdjuntoBase64 { get; set; }

        [DataMember]
        public long IdCita { get; set; }

        [DataMember]
        public DateTime? FechaCreacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }
    }
}
