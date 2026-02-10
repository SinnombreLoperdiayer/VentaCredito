using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RUMedioTransporte : DataContractBase
    {
        [DataMember]
        public short IdMedioTransporte { get; set; }
        [DataMember]
        public string DescMedioTransporte { get; set; }
    }
}
