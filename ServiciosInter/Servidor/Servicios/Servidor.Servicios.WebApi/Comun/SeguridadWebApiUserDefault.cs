using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CO.Servidor.Servicios.WebApi.Comun
{
    public class SeguridadWebApiUserDefault : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            IEnumerable<string> HUsuario;
            if (!actionContext.Request.Headers.TryGetValues("usuario", out HUsuario))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("No está autorizado. Consulte con el administrador.") });
            }

            CrearUsuarioContexto(HUsuario.ToArray()[0]);
        }

        /// <summary>
        /// Sube a memoria del hilo el usuairo 
        /// </summary>
        /// <param name="usuario"></param>
        public static void CrearUsuarioContexto(string usuario)
        {
            //if(usuario == "WEB" || usuario == "IVR" || usuario == "APP")
            // No validar la seguridad


            IContextChannel mockedChannel = (IContextChannel)ChannelFactory<IMockedService>.CreateChannel(
               new CustomBinding(new BinaryMessageEncodingBindingElement(),
                 new HttpTransportBindingElement
                 {
                     MaxBufferSize = 2147483647,
                     MaxReceivedMessageSize = 2147483647,
                 }),
                 new EndpointAddress(new Uri(@"http://localhost/CO.Servidor.Servicios.Web/PAParametrosFW.svc")));


            OperationContext.Current = new OperationContext(mockedChannel);

            OperationContext.Current.Extensions.Add(new ControllerContext()
            {
                Usuario = usuario,
                CodigoUsuario = 123,
                NombreCentroServicio = "CentroServ1",
                IdCentroServicio = 456
            });


        }

    }
}
