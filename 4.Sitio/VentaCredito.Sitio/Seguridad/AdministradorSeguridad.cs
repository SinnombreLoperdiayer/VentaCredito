using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VentaCredito.Seguridad;
using VentaCredito.Transversal;

namespace VentaCredito.Sitio.Seguridad
{
    public class AdministradorSeguridad : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            IEnumerable<string> HUsuario;
            IEnumerable<string> HAuthorizacion;
            var token = string.Empty;

            if (!actionContext.Request.Headers.TryGetValues("x-app-signature", out HUsuario)) 
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("No está autorizado. Consulte con el administrador.") });
            }

            if (!actionContext.Request.Headers.TryGetValues("x-app-security_token", out HAuthorizacion))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("No está autorizado. Consulte con el administrador.") });
            }
            else
            {
                token = HAuthorizacion.FirstOrDefault().ToLower();
                if (string.IsNullOrEmpty(token))
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("No está autorizado. Consulte con el administrador.") });
                }
            }

            var seccionUrl = actionContext.Request.RequestUri.LocalPath.Split('/');
            var metodo = seccionUrl.Count() >= 5 ? seccionUrl[4] : string.Empty;


            var contextoSitio = new ContextoSitio()
            {
                Usuario = HUsuario.FirstOrDefault(),    
                Token = token,                            
                Metodo = metodo
            };

            CrearContexto(contextoSitio);
        }

        private void CrearContexto(ContextoSitio contextoSitio)
        {
            IContextChannel mockedChannel = (IContextChannel)ChannelFactory<IMockedService>.CreateChannel(
              new CustomBinding(new BinaryMessageEncodingBindingElement(),
                new HttpTransportBindingElement
                {
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                }),
                new EndpointAddress(new Uri(@"http://localhost/CO.Servidor.Servicios.Web/PAParametrosFW.svc")));


            OperationContext.Current = new OperationContext(mockedChannel);
                        
            OperationContext.Current.Extensions.Add(contextoSitio);

            ValidarAutorizacion();
        }

        private static void ValidarAutorizacion()
        {
            var autenticacion = SeguridadServicio.Instancia.ValidarAutorizacionServicio();

            if (!autenticacion.EstaAutorizado)
            {
                throw new AuthenticationException(autenticacion.ErrorAutenticacion);
            }
        }
    }
}