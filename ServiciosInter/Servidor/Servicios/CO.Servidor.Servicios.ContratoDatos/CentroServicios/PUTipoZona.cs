using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUTipoZona: DataContractBase
    { 
        [DataMember]
        public int IdTipoZona { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }
}
