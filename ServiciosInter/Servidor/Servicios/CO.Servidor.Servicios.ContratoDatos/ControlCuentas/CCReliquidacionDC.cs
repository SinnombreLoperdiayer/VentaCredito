using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCReliquidacionDC
    {
        [DataMember]
        public decimal ValorTransporte { get; set; }

        [DataMember]
        public decimal ValorPrimaSeguro { get; set; }

        [DataMember]
        public decimal ValorImpuestos { get; set; }

        [DataMember]
        public decimal ValorTotal { get { return ValorTransporte + ValorPrimaSeguro; } }
    }
}