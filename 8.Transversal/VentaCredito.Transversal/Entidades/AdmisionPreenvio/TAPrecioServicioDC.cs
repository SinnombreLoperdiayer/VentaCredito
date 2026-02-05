using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class TAPrecioServicioDC
    {
        public IEnumerable<TAImpuestosDC> Impuestos { get; set; }

        public decimal Valor { get; set; }

        public decimal TRM { get; set; }

        public decimal ValorDolares
        {
            get
            {
                return Valor / TRM;
            }
        }

        public decimal PrimaSeguro { get; set; }
    }
}
