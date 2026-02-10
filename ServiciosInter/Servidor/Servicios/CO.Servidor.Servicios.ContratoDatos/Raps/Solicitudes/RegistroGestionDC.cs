using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
   public class RegistroGestionDC
    {
        [DataMember]
        public List<RAAdjuntoDC> adjuntos { get; set; }

        [DataMember]
        public RAGestionDC infoGestion { get; set; }
    }
}
