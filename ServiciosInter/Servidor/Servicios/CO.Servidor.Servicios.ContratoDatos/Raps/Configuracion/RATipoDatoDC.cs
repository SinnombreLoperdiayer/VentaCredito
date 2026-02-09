using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace="http://contrologis.com")]
    public class RATipoDatoDC
    {
        [DataMember]
        public int idTipoDato { get; set; }

        [DataMember]
        public string descripcion { get; set; }
    }
}
