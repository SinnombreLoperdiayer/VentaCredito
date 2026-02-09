using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.PortalCliente
{
    public class PCFiltrosEstados
    {
        public int IdEstadoFiltro { get; set; }
        public string DescripcionEstadoFiltro { get; set; }
        public int TipoEstadoFiltro { get; set; }
        public int OrdenTipoEstadoFiltro { get; set; }
        public int IdEstadoPortalCliente { get; set; }
    }
}
