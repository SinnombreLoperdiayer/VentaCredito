using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using CustomException;
using VentaCredito.Sitio.Seguridad;

namespace VentaCredito.Sitio.Controllers
{
    [RoutePrefix("api/Recogida")]
    public class RecogidasVentaCreditoController : ApiController
    {
        private readonly Recogidas.RecogidasNegocio instanciaRecogidasNegocio = Recogidas.RecogidasNegocio.Instancia;
        IEnumerable<string> HUsuario;
        IEnumerable<string> HAuthorizacion;

        /// <summary>
        /// Inserta recogidas preenvios asociados a un cliente.
        /// Hevelin Dayana Diaz - 17/06/2021
        /// </summary>
        /// <param name="recogidas">Objeto recogidas solicitado al cliente con numeros de preenvios</param>
        /// <returns>Objeto que contiene numero de recogida, numeros de preenvios a recoger y la fecha de la solicitud de recogid</returns>
        [HttpPost]
        [AdministradorSeguridad]
        [Route("InsertarRecogidaCliente")]
        [LogExceptionFilter]
        public ResponseRecogidas InsertarRecogidaCliente([FromBody] RequestRecogidas recogidas)
        {
            Request.Headers.TryGetValues("x-app-signature", out HUsuario);
            Request.Headers.TryGetValues("x-app-security_token", out HAuthorizacion);
            return instanciaRecogidasNegocio.InsertarRecogidaCliente(recogidas, HUsuario.FirstOrDefault(), HAuthorizacion.FirstOrDefault());
        }

    }
}
