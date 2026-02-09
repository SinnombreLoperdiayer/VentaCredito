using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;


namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ControlCuentas")]
    public class ControlCuentasController : ApiController
    {
        /// <summary>
        /// Metodo para obtener la guia de auditoria de liquidacion 
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerGuiaAuditoriaLiquidacion/{NumeroGuia}")]
        public CCRespuestaAuditoriaDC ObtenerGuiaAuditoriaLiquidacion(long NumeroGuia)
        {
            return ApiControlCuentas.Instancia.ObtenerGuiaAuditoriaLiquidacion(NumeroGuia);
        }

        /// <summary>
        /// Metodo para insertar novedad
        /// </summary>
        /// <param name="guia"></param>
        [HttpPost]
        [SeguridadWebApi]
        [Route("InsertarNovedadControlLiquidacion")]
        public CCRespuestaAuditoriaDC InsertarNovedadControlLiquidacion(CCGuiaDC guia)
        {
            return ApiControlCuentas.Instancia.InsertarNovedadControlLiquidacion(guia);
        }
    }
}