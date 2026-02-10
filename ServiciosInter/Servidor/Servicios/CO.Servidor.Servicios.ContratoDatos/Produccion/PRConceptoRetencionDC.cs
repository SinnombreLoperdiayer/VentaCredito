using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PRConceptoRetencionDC : DataContractBase
    {
        [DataMember]
        public PREnumTipoConcepRetencionDC TipoConceptoRetencion;

        [DataMember]
        public string IdConcepto;
    }
}
