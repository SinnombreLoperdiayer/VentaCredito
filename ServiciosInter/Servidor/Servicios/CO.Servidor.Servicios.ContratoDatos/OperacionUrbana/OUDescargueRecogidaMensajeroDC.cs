using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUDescargueRecogidaMensajeroDC
    {
        [DataMember]
        public long IdRecogida { get; set; }
        [DataMember]
        public long IdAsignacion {get;set;}
        [DataMember]
        public OUMotivoDescargueRecogidasDC MotivoDescargue {get;set;}
        [DataMember]
        public string Novedad {get;set;}

    }
}
