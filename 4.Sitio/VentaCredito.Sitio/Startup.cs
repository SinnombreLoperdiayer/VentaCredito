using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

using Microsoft.Owin.Security.OAuth;
using VentaCredito.Sitio.Seguridad;
using System.Web.Http;
using System.Configuration;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(VentaCredito.Sitio.Startup))]

namespace VentaCredito.Sitio
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureOAuth(app);
            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);

            //HttpConfiguration config = new HttpConfiguration();
            //WebApiConfig.Register(config);
            //app.UseWebApi(config);
            //ConfigureAuth(app);
            //ConfigureOAuth(app);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {

            var vidaSessionToken = Convert.ToInt32(ConfigurationManager.AppSettings["TiempoSessionToken"]);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/Autorizacion/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(vidaSessionToken),
                //AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new ProveedorAutorizacionServidor()
            };

            // Token Generation

            var options = new OAuthBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(options);

        }
    }
}
