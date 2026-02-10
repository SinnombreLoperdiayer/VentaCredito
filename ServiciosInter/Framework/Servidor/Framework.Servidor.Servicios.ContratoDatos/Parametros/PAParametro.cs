using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PAParametro : DataContractBase
    {
        [DataMember]
        public string ValorParametro { get; set; }
    }
}