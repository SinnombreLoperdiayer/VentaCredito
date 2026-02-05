using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class TAPrecioCargaDC
    {
        public List<TAImpuestosDC> Impuestos { get; set; }
        public decimal ValorServicioRetorno { get; set; }
        public decimal ValorKiloAdicional { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorContraPago { get; set; }
        public decimal ValorPrimaSeguro { get; set; }
    }
}
