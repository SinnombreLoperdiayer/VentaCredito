using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "")]
    public class INItinerarioDC
    {
        [DataMember]
        public string Placa { get; set; }
        [DataMember]
        public string Ruta { get; set; }
        [DataMember]
        public INParametrosDC Parametros { get; set; }

    }
}