using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Comun
{
    public abstract class ApiControllerWithHub<THub> : ApiController
        where THub : IHub
    {
         Lazy<IHubContext> hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        );

        protected static IHubContext Hub
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<THub>(); } // hub.Value; }
        }
    }
}
