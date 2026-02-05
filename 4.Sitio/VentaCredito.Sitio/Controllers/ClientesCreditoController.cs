using System.Collections.Generic;
using System.Web.Http;
using VentaCredito.Transversal.Entidades.Clientes;
using CustomException;
using VentaCredito.Sitio.Seguridad;
using System.Web.Http.Cors;
using VentaCredito.Clientes.Datos.Repositorio;
using VentaCredito.Clientes;
using Servicio.Entidades.Clientes;
using System;

namespace VentaCredito.Sitio.Controllers
{
    
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ClientesCredito")]
    public class ClientesCreditoController : ApiController
    {
        private readonly Clientes.CLClienteCredito instanciaClientes = Clientes.CLClienteCredito.Instancia;

        [HttpPost]
        [Route("InsertarClienteCredito")]
        [LogExceptionFilter]
        public UsuarioIntegracion InsertarClienteCredito(RequestCLCredito cliente)
        {
            return instanciaClientes.InsertarClienteCredito(cliente);
        }

        [HttpGet]
        [Route("ObtenerServiciosSeguridad")]
        [LogExceptionFilter]
        public List<Servicios_SEG> ObtenerServiciosSeguridad()
        {
            return instanciaClientes.ObtenerServiciosSeguridad();
        }

        [HttpGet]
        //[AdministradorSeguridad]
        [Route("ObtenerClienteCreditoActivo/{idCliente}/{idSucursal}")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerClienteCreditoActivo(long idCliente, long idSucursal)
        {
            return Ok(CLClienteCreditoRepositorio.Instancia.ObtenerClienteCreditoActivo(idCliente, idSucursal));
        }

        [HttpGet]
        //[AdministradorSeguridad]
        [Route("ObtenerSucursalClienteCredito/{idCliente}/{idSucursal}")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerSucursalClienteCredito(int idCliente, int idSucursal)
        {
            return Ok(CLConsultas.Instancia.ObtenerSucursalCliente(idSucursal, new CLClientesDC() { IdCliente = idCliente }));
        }

        /// <summary>
        /// Servicio que consulta los estados de una cantidad parametrizada de guias asociadas a un cliente
        /// Hevelin Dayana Diaz - 11/10/2021
        /// </summary>
        /// <param name="request">Objeto que contiene id de cliente y número de guias a consultar</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ConsultarEstadosGuiasCliente")]
        [AdministradorSeguridad]
        [LogExceptionFilter]
        public ResponseEstadosGuia_CLI ConsultarEstadosGuiaPorCliente([FromBody] RequestEstadosGuia_CLI request)
        {
            return instanciaClientes.ConsultarEstadosGuiaPorCliente(request);
        }

        /// <summary>
        /// Servicio que consulta las sucursales activas asociadas  a un cliente, teniendo en cuenta el contrato actual.
        /// Hevelin Dayana Diaz - 28/04/2023
        /// </summary>
        /// <param name="idCliente">Id cliente credito</param>
        /// <returns>Lista de sucursales asociadas a un cliente credito</returns>
        [HttpGet]
        [AdministradorSeguridad]
        [Route("ObtenerSucursalesActivasPorCliente")]
        [LogExceptionFilter]
        public List<SucursalCliente_CLI> ObtenerSucursalesActivasPorCliente([FromUri] int idCliente)
        {
            return instanciaClientes.ObtenerSucursalesActivasPorCliente(idCliente);
        }

        /// <summary>
        /// Actualiza la información de recogida de un cliente crédito
        /// Mauricio Hernandez Cabrera - 18/08/2023 - HU 51407
        /// </summary>
        /// <returns>Mensaje Satisfactorio o Fallido del proceso de actualización de la información de recogida del cliente crédito</returns>
        [HttpPost]
        [AdministradorSeguridad]
        [Route("ActualizarInfoRecogidaClienteCredito")]
        [LogExceptionFilter]
        public string ActualizarInfoRecogidaClienteCredito([FromBody] RequestCLCredito reqCliente)
        {
            return instanciaClientes.ActualizarInfoRecogidaClienteCredito(reqCliente);
        }

    }
}
