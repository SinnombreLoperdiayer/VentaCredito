using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCRespuestaAuditoriaDC : DataContractBase
    {
        [DataMember]
        public string MensajeRespuesta { get; set; }
        [DataMember]
        public bool EstadoSolicitud { get; set; }
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public CCGuiaDC GuiaAuditoria { get; set; }

        [DataMember]
        public int CantidadPorAuditar { get; set; }
    }
}
