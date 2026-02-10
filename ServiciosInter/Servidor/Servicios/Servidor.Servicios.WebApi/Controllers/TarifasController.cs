using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [RoutePrefix("api/Tarifas")]
    public class TarifasController : ApiController
    {
        #region metodos

        /// <summary>
        /// Obtiene los tipos de envio
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTipoEnvio")]
        [SeguridadWebApiUserDefault]
        public List<TATipoEnvio> ObtenerTipoEnvio()
        {
            return ApiTarifas.Instancia.ObtenerTipoEnvio();
        }

        [HttpGet]
        [Route("ObtenerTipoServicios")]
        [SeguridadWebApiUserDefault]
        public IList<TAServicioDC> ObtenerTipoServicios()
        {
            return ApiTarifas.Instancia.ObtenerTipoServicios();
        }

        [HttpGet]
        [Route("ObtenerCiudades")]
        [SeguridadWebApiUserDefault]
        public IList<PALocalidadDC> ObtenerCiudades()
        {
            return ApiTarifas.Instancia.ObtenerCiudades();
        }


        #endregion
    }
}