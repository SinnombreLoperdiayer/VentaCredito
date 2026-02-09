using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAModulosDC
    {
        [DataMember]
        public int IdModulo { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public List<RAMenuDC> Menu { get; set; }
    }
}
