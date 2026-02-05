using System.Collections.Generic;
using System.Web.Http;
using VentaCredito.Transversal.Entidades;
using CustomException;
using VentaCredito.Sitio.Seguridad;

namespace VentaCredito.Sitio.Controllers
{
    [RoutePrefix("api/Localidades")]
    public class LocalidadesVentaCreditoController : ApiController
    {
        private readonly LocalidadesNegocio.LocalidadesNegocio instanciaLocalidadesNegocio = LocalidadesNegocio.LocalidadesNegocio.Instancia;


        [HttpGet]
        [AdministradorSeguridad]
        [Route("ObtenerlocalidadesVentaCredito/{idSucursal}")]
        [LogExceptionFilter]
        public List<LocalidadesCLI> ObtenerlocalidadesVentaCredito(int idSucursal)
        {
            return instanciaLocalidadesNegocio.ObtenerlocalidadesVentaCredito(idSucursal);
        }
    }
}
