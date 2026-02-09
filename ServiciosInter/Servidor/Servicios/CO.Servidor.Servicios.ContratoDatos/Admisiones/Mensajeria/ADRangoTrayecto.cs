using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADRangoTrayecto
    {
        [DataMember]
        public string IdLocalidadOrigen { get; set; }

        [DataMember]
        public string IdLocalidadDestino { get; set; }

        [DataMember]
        public List<ADRangoCasillero> Rangos { get; set; }
    }
}
