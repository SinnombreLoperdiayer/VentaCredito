using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAGestionNotificacionMotorDC
    {
        [DataMember]
        public long IdGestion { get; set; }
        [DataMember]
        public string CorreoNotificado { get; set; }
        [DataMember]
        public string NumeroDocumento { get; set; }
        [DataMember]
        public int IdCargo { get; set; }

    }
}
