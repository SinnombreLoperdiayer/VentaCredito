using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class ResponsePlanilla
    {
        public long NumeroPlanilla { get; set; }
        public DateTime FechaHoraCreacion { get; set; }
        public long PuntoDeVenta { get; set; }
        public string NombrePuntoVenta { get; set; }
        public int CantidadEnviosSueltos { get; set; }
        public int CantidadEnviosConsolidados { get; set; }
        public int CantidadEnviosSinPlanillar { get; set; }
        public string IdentificacionMenajero { get; set; }
        public string UsuarioQueGenera { get; set; }

    }
}
