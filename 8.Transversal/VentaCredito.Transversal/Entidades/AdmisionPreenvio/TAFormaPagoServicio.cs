using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class TAFormaPagoServicio
    {
        public int IdServicio { get; set; }
        public List<TAFormaPago> FormaPago { get; set; }
    }
}
