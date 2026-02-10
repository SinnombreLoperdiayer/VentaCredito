using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RASolicitudItemDC
    {
        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public DateTime FechaVencimiento { get; set; }

    }
}
