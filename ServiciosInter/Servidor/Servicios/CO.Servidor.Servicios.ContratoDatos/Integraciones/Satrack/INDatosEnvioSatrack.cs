using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class INDatosEnvioSatrack : DataContractBase
    {
        [DataMember]
        public string Placa { get; set; }
        [DataMember]
        public string Ruta { get; set; }

        [DataMember]
        public DateTime? FechaSalida { get; set; }
    }
}
