using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "http://contrologis.com")]
   public class MensajeTextoDC : DataContractBase
    {
        [DataMember]
        public int IdMensajeNoEnviado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string NumeroCelular { get; set; }
    }
}
