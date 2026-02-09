using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiControlCuentas : ApiDominioBase
    {
        #region Singleton
        private static readonly ApiControlCuentas instancia = (ApiControlCuentas)FabricaInterceptorApi.GetProxy(new ApiControlCuentas(), COConstantesModulos.MODULO_RECOGIDAS);
        public static ApiControlCuentas Instancia
        {
            get { return ApiControlCuentas.instancia; }
        }

        #endregion

        #region Constructor
        private ApiControlCuentas()
        {
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Metodo para obtener guia para auditoria de liquidacion 
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        internal CCRespuestaAuditoriaDC ObtenerGuiaAuditoriaLiquidacion(long NumeroGuia)
        {
            return FabricaServicios.ServicioControlCuentas.ObtenerGuiaAuditoriaLiquidacion(NumeroGuia);
        }

        /// <summary>
        /// Metodo para insertar novedades control de liquidación 
        /// </summary>
        /// <param name="guia"></param>
        internal CCRespuestaAuditoriaDC InsertarNovedadControlLiquidacion(CCGuiaDC guia)
        {
            return FabricaServicios.ServicioControlCuentas.InsertarNovedadControlLiquidacion(guia);
        }

        #endregion
    }
}