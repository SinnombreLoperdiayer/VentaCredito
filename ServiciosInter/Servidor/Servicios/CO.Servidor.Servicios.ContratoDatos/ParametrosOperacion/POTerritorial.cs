using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class POTerritorial
    {
        [DataMember]
        public int IdTerritorial { get; set; }

        [DataMember]
        public string DescripcionTerritorial { get; set; }
    }
}
