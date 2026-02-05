using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Clientes
{
    public class ResponseEstadosGuia_CLI
    {
        public List<GuiasEstados> ListadoGuias { get; set; }
        public string MensajeGuiasNoCliente { get; set; }
        public List<long> ListadoGuiasNoCliente { get; set; }

    }

    public class GuiasEstados
    {
        public long NumeroGuia { get; set; }
        public string DetalleMotivoDevolucion { get; set; }

        public List<EstadoGuiaCLI_MEN> EstadosGuia { get; set; }
        public List<EstadoGuiaCLI_MEN> EstadosPreenvio { get; set; }
        public List<EstadoGuiaCLI_MEN> EstadosRecogida { get; set; }
    }
}
