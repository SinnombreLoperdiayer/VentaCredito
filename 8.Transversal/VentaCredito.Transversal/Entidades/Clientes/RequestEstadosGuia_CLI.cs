using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Clientes
{
    public class RequestEstadosGuia_CLI
    {
        public long IdCliente { get; set; }
        public List<long> NumeroGuias { get; set; }
    }
}
