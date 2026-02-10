using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CO.Servidor.Servicios.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.EnableCors(new EnableCorsAttribute("*","*","*"));
            config.MapHttpAttributeRoutes();
            config.EnableSystemDiagnosticsTracing();


        }
    }
}
