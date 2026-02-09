using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAPantillaAccionCorreoDC
    {
        [DataMember]
        public long IdPlantilla { get; set; }

        [DataMember]
        public Byte IdAccion { get; set; }

        [DataMember]
        public string Asunto { get; set; }

        [DataMember]
        public string Cuerpo { get; set; }

        [DataMember]
        public bool EsPredeterminada { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
