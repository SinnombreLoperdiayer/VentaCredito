using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "")]
   public  class INTrackingGuiaDC
    {
        [DataMember]
        public string respuestaAcceso { get; set; }

        [DataMember]
        public string descripcionRespuestaAcceso { get; set; }

        [DataMember]
        public ADGuia guiaConsultada { get; set; }
        [DataMember]
        public string observacionConsultaGuia { get; set; }
    }
}
