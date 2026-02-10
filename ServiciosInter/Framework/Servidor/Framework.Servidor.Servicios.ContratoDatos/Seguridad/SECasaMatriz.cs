using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SECasaMatriz : DataContractBase
    {
        [DataMember]
        public short Id { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }
}