using ServiciosInter.DatosCompartidos.EntidadesNegocio.PortalCliente;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.PortalCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.Negocio.PortalCliente
{
    public class PortalClienteNegocio
    {
        private static readonly PortalClienteNegocio instancia = new PortalClienteNegocio();

        public static PortalClienteNegocio Instancia
        {
            get
            {
                return instancia;
            }
        }

        private PortalClienteNegocio()
        { 
        }

        /// <summary>
        /// Retornar los filtros de busqueda para Pago en Casa
        /// </summary>
        /// <returns></returns>
        public PCFiltrosResponse ObtenerFiltrosPortalCliente()
        {
            return PortalClienteRepository.Instancia.ObtenerFiltrosPortalCliente();
        }
    }
}
