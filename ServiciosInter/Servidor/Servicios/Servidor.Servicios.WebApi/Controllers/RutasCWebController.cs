using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [RoutePrefix("api/RutasCWeb")]
    public class RutasCWebController : ApiController
    {
        #region metodos

        /// <summary>
        /// Obtiene información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRuta")]
        [SeguridadWebApiUserDefault]
        public List<RURutaICWeb> ObtenerRuta()
        {
            return ApiRutasCWeb.Instancia.obtenerRuta();
        }
        
        [HttpGet]
        [Route("obtenerRutaDetalleCentroServiciosRuta")]
        [SeguridadWebApiUserDefault]
        public List<RURutaCWebDetalleCentrosServicios> obtenerRutaDetalleCentroServiciosRuta([FromUri]int idruta)
        {
            return ApiRutasCWeb.Instancia.obtenerRutaDetalleCentroServiciosRuta(idruta);
        }

        #endregion
    }
}