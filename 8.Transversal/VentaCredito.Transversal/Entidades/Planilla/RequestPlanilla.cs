using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class RequestPlanilla
    {
        public long IdCentroServicio { get; set; }
        public string DocumentoMensajero { get; set; }
        public long IdSolicitudRecogida { get; set; }
        public List<GuiasConsolidadas> ListaGuiasConsolidadas { get; set; }
        public List<long> ListaGuiasSueltas { get; set; }
        public List<string> ListaConsolidados { get; set; }
        public string Usuario { get; set; }
        public bool NoTieneTula { get; set; }
        public bool NoTienePrecinto { get; set; }
    }
}
