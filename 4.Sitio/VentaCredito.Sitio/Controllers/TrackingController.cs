using System;
using System.IO;
using System.Web.Http;
using VentaCredito.Transversal.Entidades;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace VentaCredito.Sitio.Controllers
{
    [RoutePrefix("api/Tracking")]
    public class TrackingController : ApiController
    {
        readonly string rutaLog = "C:/PerfLogs/Traking.txt";
        [HttpPost]        
        [Route("ObtenerToken")]
        public RespuestaToken ObtenerToken(HttpRequestMessage request)
        {
            RespuestaToken respuestaToken = new RespuestaToken();
            try
            {
                respuestaToken.access_token = request.Headers.Authorization.ToString().Split(' ')[1];
                return respuestaToken;
            }
            catch (Exception ex)
            {
                File.AppendAllText(rutaLog, ex.Message + Environment.NewLine);
                throw;
            }
        }

        [HttpPost]        
        [Route("HacerPush")]
        public IHttpActionResult HacerPush([FromBody] NotificacionTracking objTraking)
        {
            try
            {
                var jsonString = new JavaScriptSerializer();
                File.AppendAllText(rutaLog, "Inicio de mensaje" + Environment.NewLine);
                File.AppendAllText(rutaLog, jsonString.Serialize(objTraking) + Environment.NewLine);
                File.AppendAllText(rutaLog, "Fin de mensaje" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(rutaLog, ex.Message + Environment.NewLine);
                throw;
            }
            return Ok();
        }
    }
}
