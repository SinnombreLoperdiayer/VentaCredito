using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using CO.Servidor.Servicios.WebApi.ProcesosAutomaticos;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(CO.Servidor.Servicios.WebApi.Startup))]

namespace CO.Servidor.Servicios.WebApi
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);


            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            HubConfiguration hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableJSONP = true;
            hubConfiguration.EnableJavaScriptProxies = true;
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);

            app.Map("/signalRWebApi", map =>
            {
                map.RunSignalR(hubConfiguration);
            });

            

        }

        //

    }
}
