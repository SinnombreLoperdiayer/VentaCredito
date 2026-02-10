using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Dominio
{

    public class ApiAdministradorClientes : ApiDominioBase
    {

        private static readonly ApiAdministradorClientes instancia = (ApiAdministradorClientes)FabricaInterceptorApi.GetProxy(new ApiAdministradorClientes(), COConstantesModulos.CLIENTES);

        public static ApiAdministradorClientes Instancia
        {
            get { return ApiAdministradorClientes.instancia; }
        }

        private ApiAdministradorClientes()
        {
        }

        /// <summary>
        /// Obtiene los centros de servicio
        /// </summary>
        /// <returns></returns>
        public string ObtenerNombreyDireccionCliente(string telefono)
        {
            string retorno = FabricaServicios.ServicioAdministracionClientes.ObtenerNombreyDireccionCliente(telefono);
            return retorno;

        }

        /// <summary>
        /// Obtiene la lista de todos los clientes credito
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<CLClienteCreditoSucursalContrato> ObtenerClientesCredito()
        {
            return FabricaServicios.ServicioAdministracionClientes.ObtenerClientesCredito();
        }

        /// <summary>
        /// Obtiene la lista de las sucursales de clientes credito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesClientesCredito(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, int idEstado)
        {
            return FabricaServicios.ServicioAdministracionClientes.ObtenerSucursalesClientesCredito(fechaInicial, fechaFinal, idMensajero, idEstado);
        }
    }
}