using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class InformacionECAPTURE : DataContractBase
    {
        [DataMember]
        public bool RegistroExitoso { get; set; }

        [DataMember]
        public string MensajeServicio { get; set; }

        [DataMember]
        public DateTime FechaEnvioEcapture { get; set; }

        [DataMember]
        public DateTime FechaLecturaMarca { get; set; }

    }
}
