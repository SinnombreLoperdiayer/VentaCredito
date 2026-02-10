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
    public class SeguridadWebApiAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            IEnumerable<string> HUsuario;
            if (!actionContext.Request.Headers.TryGetValues("usuario", out HUsuario))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("No está autorizado. Consulte con el administrador.") });
            }

            IEnumerable<string> HIdusuario;
            actionContext.Request.Headers.TryGetValues("IdUsuario", out HIdusuario);
            IEnumerable<string> HIdCentroServicio;
            actionContext.Request.Headers.TryGetValues("IdCentroServicio", out HIdCentroServicio);
            IEnumerable<string> HNombreCentroServicio;
            actionContext.Request.Headers.TryGetValues("NombreCentroServicio", out HNombreCentroServicio);
            IEnumerable<string> HIdAplicativoOrigen;
            actionContext.Request.Headers.TryGetValues("IdAplicativoOrigen", out HIdAplicativoOrigen);
            IEnumerable<string> HIdentificacion;
            actionContext.Request.Headers.TryGetValues("Identificacion", out HIdentificacion);


            CrearContexto(HUsuario.ToArray()[0],
                HIdusuario != null ? HIdusuario.ToArray()[0] : "",
                HIdCentroServicio != null ? HIdCentroServicio.ToArray()[0] : "",
                HNombreCentroServicio != null ? HNombreCentroServicio.ToArray()[0] : "",
                HIdAplicativoOrigen != null ? HIdAplicativoOrigen.ToArray()[0] : "",
                HIdentificacion != null ? HIdentificacion.ToArray()[0] : "");



        }

        /// <summary>
        /// Sube a memoria del hilo los datos de la sesion del usuario
        /// </summary>
        /// <param name="usuario"></param>
        public static void CrearContexto(string usuario, string idUsuario, string idCentroServicio, string nombreCentroServicio, string idAplicativoOrigen, string identificacionH)
        {
            int idUsuarioValidado, idCentroServicioValidado, idAplicativoOrigenValidado;
            long identificacion = 0;

            idUsuarioValidado = (string.IsNullOrEmpty(idUsuario)) ? 0 : int.Parse(idUsuario);
            idCentroServicioValidado = (string.IsNullOrEmpty(idCentroServicio)) ? 0 : int.Parse(idCentroServicio);
            idAplicativoOrigenValidado = (string.IsNullOrEmpty(idAplicativoOrigen)) ? 0 : int.Parse(idAplicativoOrigen);
            identificacion = (string.IsNullOrEmpty(identificacionH)) ? 0 : long.Parse(identificacionH);

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
                CodigoUsuario = idUsuarioValidado,
                NombreCentroServicio = nombreCentroServicio,
                IdCentroServicio = idCentroServicioValidado,
                IdAplicativoOrigen = idAplicativoOrigenValidado,
                Identificacion = identificacion
            });


        }

        /// <summary>
        /// sube a la memoria del hilo los datos del usuario "AUTOMATICO"
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="nombreCentroServicio"></param>
        /// <param name="idAplicativoOrigen"></param>
        public static void CrearUsuarioContextoAutomatico(string usuario)
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

            OperationContext.Current.Extensions.Add(new ControllerContext()
            {
                Usuario = usuario,
                CodigoUsuario = 0,
                NombreCentroServicio = "",
                IdCentroServicio = 0,
                IdAplicativoOrigen = 0
            });
        }



    }
}
