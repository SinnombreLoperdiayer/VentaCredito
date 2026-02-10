using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace="")]
    public class puntoDeAtencion
    {
        [DataMember]
        public string codigoPuntoAtencion { get; set; }

        [DataMember]
        public double montoIngreso { get; set; }

        [DataMember]
        public double montoEgreso { get; set; }

        [DataMember]
        public double ingresoNeto { get; set; }
    }
}
