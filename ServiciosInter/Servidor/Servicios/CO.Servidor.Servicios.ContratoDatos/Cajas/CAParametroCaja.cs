using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAParametroCaja
    {
        [DataMember]
        public string IdParametro { get; set; }

        [DataMember]
        public string ValorParametro { get; set; }

        [DataMember]
        public string DescripcionParametro { get; set; }
    }
}
