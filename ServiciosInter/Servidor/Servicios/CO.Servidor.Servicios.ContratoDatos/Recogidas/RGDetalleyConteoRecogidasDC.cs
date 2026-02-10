using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Recogidas.Recogidas;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGDetalleyConteoRecogidasDC
    {
        [DataMember]
        public List<RGDetalleConteoAdminRecogidasDC> DetalleRecogidas { get; set; }

        [DataMember]
        public RGConteoRecogidasDC Conteos { get; set; }
    }
}
