using System.Collections.Generic;
using System.Web.Http;
using VentaCredito.Sitio.Dominio;
using VentaCredito.Sitio.Seguridad;
using VentaCredito.Transversal.Entidades;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.Clientes.Comun;
using VentaCredito.Negocio.VentaCredito;
using VentaCredito.Negocio.Interface;
using CustomException;

namespace VentaCredito.Sitio.Controllers
{
    [RoutePrefix("api/ClienteCorporativo")]
    public class ClienteCorporativoController : ApiController
    {

        private readonly IClienteCorporativoNegocio NegocioCliente = ClienteCorporativoNegocio.Instancia;

        [HttpPost]
        //[Authorize]
        [AdministradorSeguridad]
        [Route("RegistrarEnvio")]
        [LogExceptionFilter]
        public AdmisionEnvioResponse RegistrarEnvio([FromBody]AdmisionEnvioRequest admisionEnvio)
        {
            return ApiNegocio.Instancia.RegistrarEnvioAutomatico(admisionEnvio);
        }

        /// <summary>
        /// Controlador del metodo que crea una solicitud de cancelacion de guia.
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <param name="Observaciones"></param>
        /// <returns></returns>
        [HttpPost]
        [AdministradorSeguridad]
        [Route("CrearSolicitudCancelacion")]
        [LogExceptionFilter]
        public IHttpActionResult CrearSolicitudCancelacion(CancelacionGuia_Wrapper Cancelacion)
        {
            NegocioCliente.EnviarCorreoCancelacionGuia(Cancelacion);
            return Ok();
        }
        //Metodo que crea una gía en admisionmensajeria y retorna guía en base 64
        [HttpPost]
        //[Authorize]
        [AdministradorSeguridad]
        [Route("RegistrarEnvioGuia")]
        [LogExceptionFilter]
        public AdmisionEnvioResponse RegistrarVentaConGuiaRetorno([FromBody]AdmisionEnvioRequest admisionEnvio)
        {
            return ApiNegocio.Instancia.RegistrarEnvioAutomatico(admisionEnvio, true);
        }


        [HttpGet]
        [AdministradorSeguridad]
        [Route("ObtenerHorariosServiciosAgiles")]
        [LogExceptionFilter]
        public IEnumerable<ServicioAgilFranjaDC> ObtenerHorariosServiciosAgiles([FromUri]int? idServicio = null)
        {
            return Negocio.Parametros.Instancia.ObtenerHorariosServiciosAgiles(idServicio);
        }

        [HttpGet]
        //[Authorize]
        [AdministradorSeguridad]
        [Route("ObtenerBase64PdfGuia/{numeroGuia}")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerBase64PdfGuia(long numeroGuia)
        {
            return Ok(ApiNegocio.Instancia.ObtenerPdfGuia(numeroGuia)); 
        }

    }
}