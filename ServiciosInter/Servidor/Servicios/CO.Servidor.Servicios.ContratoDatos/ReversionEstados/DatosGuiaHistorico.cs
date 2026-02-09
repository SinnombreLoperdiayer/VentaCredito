using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ReversionEstados
{
    [DataContract]
    public class DatosGuiaHistorico
    {
        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public long IdAdmisioneMensajeria { get; set; }
        [DataMember]
        public long IdAuditoria { get; set; }
    }
}
