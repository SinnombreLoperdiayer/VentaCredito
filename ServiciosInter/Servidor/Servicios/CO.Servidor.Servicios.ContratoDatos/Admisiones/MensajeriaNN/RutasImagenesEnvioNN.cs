using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RutasImagenesEnvioNN
    {  
        [DataMember]
        public long IdArchivo { get; set; }
        [DataMember]
        public string RutaArchivo { get; set; }

    }
}
