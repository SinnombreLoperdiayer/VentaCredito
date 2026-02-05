using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Clientes;
using VentaCredito.Clientes.Datos.Repositorio;

namespace VentaCredito.Clientes
{
    public class CLConsultas
    {
        private static CLConsultas instancia = new CLConsultas();

        public static CLConsultas Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Retorna la información básica de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalDC ObtenerSucursalCliente(int idSucursal, CLClientesDC cliente)
        {
            return CLSucursalRepositorio.Instancia.ObtenerSucursalCliente(idSucursal, cliente);
        }

        /// <summary>
        /// Consulta información de un cliente por su id
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerCliente(int idCliente)
        {
            return CLClienteCreditoRepositorio.Instancia.ObtenerCliente(idCliente);
        }

        /// <summary>
        /// Retorna información del cliente crédito,
        /// retornar las  sucursal y
        /// los contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public List<CLContratosDC> ObtenerContratosSucursal(int idSucursal)
        {
            return CLClienteCreditoRepositorio.Instancia.ObtenerContratosSucursal(idSucursal);
        }

    }
}
