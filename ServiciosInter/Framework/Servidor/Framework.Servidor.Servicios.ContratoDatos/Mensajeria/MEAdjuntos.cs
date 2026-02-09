using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
     [DataContract(Namespace = "http://contrologis.com")]
    public class MEAdjuntos
    {        
        [DataMember]
        public string RutaAdjunto{get;set;}

        [DataMember]
        public byte[] ArchivoAdjunto { get; set; }

        [DataMember]
        public string ExtensionArchivoAdjunto { get; set; }
    }
}
