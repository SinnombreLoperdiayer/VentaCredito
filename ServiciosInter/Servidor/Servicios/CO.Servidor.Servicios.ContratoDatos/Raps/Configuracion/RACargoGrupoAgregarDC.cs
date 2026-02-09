using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RACargoGrupoAgregarDC
    {
        [DataMember]
        public int IdCargo { get; set; }

        [DataMember]
        public int IdGrupo { get; set; }
    }
}
