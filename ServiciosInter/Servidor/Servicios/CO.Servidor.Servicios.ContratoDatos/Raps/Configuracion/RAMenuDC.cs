using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAMenuDC
    {
        [DataMember]
        public int IdMenu { get; set; }

        [DataMember]
        public int AccionesMenu { get; set; }

    }
}
