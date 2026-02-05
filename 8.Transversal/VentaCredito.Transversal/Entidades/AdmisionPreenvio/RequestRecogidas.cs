using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class RequestRecogidas
    {
        public int IdClienteCredito {get; set; }
        public int IdSucursalCliente { get; set; }
        public List<long> ListaNumPreenvios { get; set; }
        public DateTime FechaRecogida { get; set; }
    }
}
