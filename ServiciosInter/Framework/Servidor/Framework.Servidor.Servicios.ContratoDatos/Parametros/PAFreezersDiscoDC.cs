using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PAFreezersDiscoDC : DataContractBase
    {
        [DataMember]
        public string NombrePrograma { get; set; }

        [DataMember]
        public string NombreProceso { get; set; }

        [DataMember]
        public string NombreServicio { get; set; }
    }
}
