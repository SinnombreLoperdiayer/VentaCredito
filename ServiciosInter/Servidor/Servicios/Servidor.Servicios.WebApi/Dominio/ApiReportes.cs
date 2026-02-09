using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiReportes : ApiDominioBase
    {
        private static readonly ApiReportes instancia = (ApiReportes)FabricaInterceptorApi.GetProxy(new ApiReportes(), COConstantesModulos.REPORTES);

        public static ApiReportes Instancia
        {
            get { return ApiReportes.instancia; }
        }


        /// <summary>
        /// retorna la url de reportes
        /// </summary>
        /// <returns></returns>
        public string ConsultarUrlReportes()
        {
            return FabricaServicios.ServicioReportes.ConsultarUrlReportes();
        }
    }
}