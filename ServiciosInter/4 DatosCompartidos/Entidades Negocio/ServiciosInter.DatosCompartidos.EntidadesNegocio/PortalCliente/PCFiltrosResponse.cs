using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.PortalCliente
{
    public class PCFiltrosResponse
    {
        public List<PCFiltrosBusqueda> FiltrosBusqueda { get; set; }

        public List<PCFiltrosEstados> FiltrosEstados { get; set; }

        public List<PCFiltrosTiposEstado> FiltrosTiposEstados { get; set; }
    }
}
