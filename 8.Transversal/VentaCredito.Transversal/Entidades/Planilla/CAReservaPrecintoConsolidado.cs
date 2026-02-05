using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class CAReservaPrecintoConsolidado
    {
        public int IdTipo { get; set; }
        public string NumeroConsolidado { get; set; }
        public string NumeroPrecinto { get; set; }
        public bool EsReservada { get; set; }
        public long IdCentroServicio { get; set; }
    }
}
