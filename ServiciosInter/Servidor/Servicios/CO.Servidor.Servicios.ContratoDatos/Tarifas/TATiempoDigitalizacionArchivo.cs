using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class TATiempoDigitalizacionArchivo
    {
        [DataMember]
        public double numeroDiasDigitalizacion { get; set; }

        [DataMember]
        public double numeroDiasArchivo { get; set; }

        [DataMember]
        public int numeroDiasEntrega { get; set; }
    }
}
