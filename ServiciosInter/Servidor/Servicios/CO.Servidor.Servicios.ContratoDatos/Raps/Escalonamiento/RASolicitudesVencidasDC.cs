using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RASolicitudesVencidasDC
    {
        [DataMember]
        public RASolicitudDC Solicitud { get; set; }
        [DataMember]
        public RACargoDC Cargo { get; set; }
        [DataMember]
        public List<RAEscalonamientoDC> Escalonamiento { get; set; }

    }
}
