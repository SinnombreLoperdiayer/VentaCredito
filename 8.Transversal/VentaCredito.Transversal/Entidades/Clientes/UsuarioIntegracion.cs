using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Clientes
{
    public class UsuarioIntegracion
    {
        public string UsuarioCliente { get; set; }
        public string PasswordCliente { get; set; }
        public string Token { get; set; }
        public int IdClienteCredito { get; set; }
    }
}
