using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class LogMovimientoPrecintoPUAIntegra
    {

        public int IdLogMovPrecinto { get; set; }

        public int IdTipoMovimientoPrecinto { get; set; }

        public string NumeroPrecinto { get; set; }

        public string Fecha { get; set; }

        public string Usuario { get; set; }
    }
}
