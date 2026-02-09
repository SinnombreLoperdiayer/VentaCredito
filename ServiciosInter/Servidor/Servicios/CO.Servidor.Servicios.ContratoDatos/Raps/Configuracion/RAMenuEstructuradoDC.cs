using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace="http://contrologis.com")]
    public class RAMenuEstructuradoDC
    {
        [DataMember]
        public RAMenusPermitidosDC MenuPadre { get; set; }

        [DataMember]
        public List<RAMenusPermitidosDC> MenuHijo { get; set; }
    }
}
