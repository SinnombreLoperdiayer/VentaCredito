using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "")]
    public class INRespuestaDC
    {
        [DataMember]
        public string Placa { get; set; }
        [DataMember]
        public string Ruta { get; set; }
        [DataMember]
        public int Codigo { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public string Campo1 { get; set; }

    }
}
