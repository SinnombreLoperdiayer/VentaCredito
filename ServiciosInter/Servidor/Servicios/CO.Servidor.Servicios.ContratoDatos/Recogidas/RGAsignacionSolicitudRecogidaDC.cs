using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{	
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGAsignacionSolicitudRecogidaDC
    {
        [DataMember]
        public long IdAsignacion { get; set; }

        [DataMember]
        public string DocPersonaResponsable { get; set; }

        [DataMember]
        public long IdSolicitudRecogida { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public string PlacaVehiculo { get; set; }
    }

}
