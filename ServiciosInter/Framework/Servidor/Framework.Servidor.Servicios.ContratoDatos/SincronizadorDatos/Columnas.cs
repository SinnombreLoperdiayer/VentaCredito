using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class Columnas : DataContractBase
    {
        [DataMember]
        public string NombreColumna { get; set; }
        [DataMember]
        public string TipoDato { get; set; }
        [DataMember]
        public object Valor { get; set; }

    }
}
