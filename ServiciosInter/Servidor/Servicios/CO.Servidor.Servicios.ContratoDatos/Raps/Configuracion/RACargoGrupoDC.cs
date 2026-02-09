using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RACargoGrupoDC
    {
        [DataMember]
        public List<RACargoDC> lstCargo { get; set; }

        [DataMember]
        public string DescripcionGrupo { get; set; }

        [DataMember]
        public int IdGrupo { get; set; }

        [DataMember]
        public int IdSucursal { get; set; }
    }
}
