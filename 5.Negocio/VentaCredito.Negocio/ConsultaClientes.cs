using Servicio.Entidades.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Negocio
{
    public class ConsultaClientes
    {
        private static ConsultaClientes instancia = new ConsultaClientes();
        public static ConsultaClientes Instancia { get { return instancia; } }

        public CLClientesDC ObtenerDatosCliente(int idCliente)
        {
            return Clientes.CLConsultas.Instancia.ObtenerCliente(idCliente);
        }
    }
}
