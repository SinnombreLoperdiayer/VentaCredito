using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using CO.Servidor.ExploradorGiros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros;
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/giros")]
    ///Clase que expone los servicios REST para el modulo de Giros GIR
    public class GirosController : ApiController
    {
        private readonly IGIAdministradorExploradorGiros InstanciaNegocio = GIAdministradorExploradorGiros.Instancia;

        [Route("")]
        [SeguridadWebApi]
        [ResponseType(typeof(GIExploradorGirosWebDC))]
        public IHttpActionResult post(GIExploradorGirosWebDC informacionGiro)
        {
            GIExploradorGirosWebDC giro = null;
            try
            {
                giro = InstanciaNegocio.ObtenerDatosGiros(informacionGiro);
                if (giro == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(giro);
        }
    }
}