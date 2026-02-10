using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCGuiaIdAuditoria : DataContractBase
    {
        [DataMember]
        public long IdAuditoriaGuia { get; set; }
        [DataMember]
        public long NumeroGuia { get; set; }
    }
}
