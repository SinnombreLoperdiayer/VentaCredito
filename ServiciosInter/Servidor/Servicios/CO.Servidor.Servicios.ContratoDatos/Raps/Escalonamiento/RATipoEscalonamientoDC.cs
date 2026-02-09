using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RATipoEscalonamientoDC
    {
        [DataMember]
        public int IdTipoEscalonamiento { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }
}