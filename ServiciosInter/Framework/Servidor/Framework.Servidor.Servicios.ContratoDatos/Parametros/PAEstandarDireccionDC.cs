using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PAEstandarDireccionDC : DataContractBase
    {
        [DataMember]
        public int? IdAbreviaturaDireccion { get; set; }

        [DataMember]
        public string VariacionDireccion { get; set; }

        [DataMember]
        public string AbreviacionDireccion { get; set; }
    }
}
