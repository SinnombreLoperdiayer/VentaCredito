using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
public class Test : Hub
{


    public void Send(string name, string message)
    {    
        Clients.All.addNewMessageToPage(name, message);
    }


    public static void ReportarCreacionRecogida()
    {
        var context = GlobalHost.ConnectionManager.GetHubContext<Test>();
        context.Clients.All.ReportarCreacionRecogida();          
    }

    public static void ReportarMensajero(string idMensajero, decimal latitud, decimal longitud)
    {
        var context = GlobalHost.ConnectionManager.GetHubContext<Test>();
        context.Clients.All.ReportarMensajero(idMensajero, latitud, longitud);          
    }
}