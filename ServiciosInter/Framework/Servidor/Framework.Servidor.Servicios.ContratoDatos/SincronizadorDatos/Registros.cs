using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class Registros : DataContractBase
    {
        [DataMember]
        public List<Columnas> Columnas { get; set; }
        [DataMember]
        public string NombreTabla { get; set; }
        [DataMember]
        public string ActualAnchor { get; set; }
        [DataMember]
        public int BatchActual { get; set; }
        [DataMember]
        public int TotalBatch { get; set; }

        [DataMember]
        public string Error { get; set; }
    }
}
