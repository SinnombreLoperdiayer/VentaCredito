using Servicio.Entidades.Admisiones.Mensajeria;
using System.Web.Http;
using VentaCredito.Sitio.Dominio;
using CustomException;

namespace VentaCredito.Sitio.Controllers
{

    [RoutePrefix("api/VentaCredito")]    
    public class VentaCreditoController : ApiController
    {
        [HttpPost]
        [Authorize]
        //[AdministradorSeguridad]        
        [Route("RegistrarVenta")] 
        [LogExceptionFilter]
        public ADResultadoAdmision RegistrarVenta(VentaCreditoRequest admisionRequest)
        {
            return ApiNegocio.Instancia.RegistrarGuiaAutomatica(admisionRequest.Guia, admisionRequest.IdCaja, admisionRequest.RemitenteDestinatario);            
        }

    }
}