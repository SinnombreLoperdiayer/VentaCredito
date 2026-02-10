using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RANotificacionDC
    {
        [DataMember]
        public int idNotificacion { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public DateTime fechaGrabacion { get; set; }

        [DataMember]
        public Int64 idSolicitud { get; set; }
    }
}
