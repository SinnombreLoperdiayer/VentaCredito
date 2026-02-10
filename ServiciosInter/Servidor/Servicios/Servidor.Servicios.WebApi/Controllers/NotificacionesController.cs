

using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.Implementacion.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using CO.Servidor.Servicios.WebApiHub;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.Implementacion;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{

    [RoutePrefix("api/NotificacionesController")]
    ///Clase que expone los servicios REST para el envio de las notificacios
    public class NotificacionesController : ApiController
    {

        /// <summary>
        /// Registra un dispositivo movil dentro de controller
        /// </summary>
        /// <param name="tipoDispositivo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RegistrarDispositivoMovil")]
        [SeguridadWebApi]
        //public void RegistrarClientePeaton([FromUri]string sistemaOperativo,[FromUri]string tipoDispositivo, [FromUri]string token)
        public long RegistrarDispositivoMovil([FromBody]PADispositivoMovil tipoDispositivo)
        {
            return FabricaServicios.ServicioParametros.RegistrarDispositivoMovil(tipoDispositivo);

        }

        /// <summary>
        /// Envia mensaje a todos los clientes conectados al hub
        /// </summary>
        /// <param name="Usuario"></param>
        /// <param name="Mensaje"></param>
        [HttpPost]
        [Route("NotificarSolicitudUsuario")]
        [SeguridadWebApi]
        public void NotificarSolicitudUsuario([FromBody]ParametrosSignalR parametro)
        {
            ApiConfiguracionRaps.Instancia.NotificarSolicitudUsuario(parametro);                                                                                                                                                                      
        }

        /// <summary>
        /// Envia mensaje a todos los clientes conectados al hub
        /// </summary>
        /// <param name="Usuario"></param>
        /// <param name="Mensaje"></param>
        [HttpPost]
        [Route("NotificarMantenimiento")]
        [SeguridadWebApi]
        public void NotificarMantenimiento([FromBody]ParametrosSignalR parametro)
        {
            ApiConfiguracionRaps.Instancia.NotificarMantenimiento(parametro);
        }

        /// <summary>
        /// Metodo para enviar la notificacion a todo destino
        /// </summary>
        /// <param name="parametro"></param>
        [HttpPost]
        [Route("NotificarSolicitudTodosUsuarios")]
        [SeguridadWebApi]
        public void NotificarSolicitudTodosUsuarios()
        {
            ApiConfiguracionRaps.Instancia.NotificarSolicitudTodosUsuarios();
            //var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
            //context.Clients.All.enviarNotificacionTodoUsuario(parametro);
        }
    }
}
