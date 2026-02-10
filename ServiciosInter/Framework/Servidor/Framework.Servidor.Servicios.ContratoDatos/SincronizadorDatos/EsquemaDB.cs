using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class EsquemaDB : DataContractBase
    {
        [DataMember]
        public string NombreTabla { get; set; }

        [DataMember]
        public string Pk { get; set; }

        [DataMember]
        public string QueryCreacion { get; set; }

        [DataMember]
        public int BatchSize { get; set; }

        [DataMember]
        public string Filtro { get; set; }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public int NumeroCampos { get; set; }
    }
}
