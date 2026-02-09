using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    [DataContract]
    public class PAValorDolar
    {
        [DataMember]
        public DateTime FechaVigencia { get; set; }

        [DataMember]
        public double Valor { get; set; }
    }
}
