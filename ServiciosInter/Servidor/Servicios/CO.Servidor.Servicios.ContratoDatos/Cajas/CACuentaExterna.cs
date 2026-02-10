using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CACuentaExterna
    {
        [DataMember]
        public short Id { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string CodCtaExterna { get; set; }
    }
}