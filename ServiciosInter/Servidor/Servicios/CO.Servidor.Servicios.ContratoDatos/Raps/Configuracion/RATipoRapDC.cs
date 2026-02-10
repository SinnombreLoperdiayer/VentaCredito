using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RATipoRapDC
    {
        [DataMember]
        public int IdTipoRap { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
