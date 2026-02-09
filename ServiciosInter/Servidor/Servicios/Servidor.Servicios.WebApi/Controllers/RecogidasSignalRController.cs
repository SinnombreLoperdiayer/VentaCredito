using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Comun.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
     //[RoutePrefix("api/RecogidasWebSocket")]
    // [InvalidModelStateFilter]
    public class RecogidasSignalRController : ApiControllerWithHub<RecogidasHub>
    {

        public RecogidasSignalRController()
        {

        }

      /*  public void Subscribe(string customerId)
        {
            Groups.Add(Context.ConnectionId, customerId);
        }

        public void Unsubscribe(string customerId)
        {
            Groups.Remove(Context.ConnectionId, customerId);
        }*/


        public static void ReportarCreacionRecogida()
        {
           // var context = GlobalHost.ConnectionManager.GetHubContext<RecogidasHub>();
          // Clients.All.ReportarCreacionRecogida();
           Hub.Clients.All.ReportarCreacionRecogida();
        }

        public static  void ReportarMensajero(string idMensajero, decimal latitud, decimal longitud)
        {
           // var context = GlobalHost.ConnectionManager.GetHubContext<RecogidasHub>();
            Hub.Clients.All.ReportarMensajero(idMensajero, latitud, longitud);
        }
    }
}