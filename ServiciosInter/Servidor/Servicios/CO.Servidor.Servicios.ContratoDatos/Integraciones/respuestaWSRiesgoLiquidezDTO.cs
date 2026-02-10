using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "")]
    public class respuestaWSRiesgoLiquidezDTO
    {
        [DataMember]
        public string codigoRed { get; set; }

        [DataMember]
        public DateTime fecha { get; set; }

        [DataMember]
        public string estado { get; set; }

        [DataMember]
        public string descripcionEstado { get; set; }

        [DataMember]
        [XmlArrayItem()]
        public List<puntoDeAtencion> listaPuntosDeAtencion { get; set; }
    }
}
