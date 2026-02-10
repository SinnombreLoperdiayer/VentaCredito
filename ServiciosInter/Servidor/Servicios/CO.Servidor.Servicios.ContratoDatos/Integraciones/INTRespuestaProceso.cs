using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "")]
    public class INTRespuestaProceso
    {
        [DataMember]
        public int Codigo { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
        [DataMember]
        public  int idtransaccion { get; set; }
        [DataMember]
        public string Token { get; set; }
    }
}
