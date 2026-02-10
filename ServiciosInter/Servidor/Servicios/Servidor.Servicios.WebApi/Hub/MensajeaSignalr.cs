using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using CO.Servidor.Servicios.WebApiHub;
using Microsoft.AspNet.SignalR;


namespace CO.Servidor.Servicios.WebApi.Hub
{
    public class MensajeaSignalr
    {


        private static MensajeaSignalr instancia;
        private IHubContext context;

        #region Singleton
        public static MensajeaSignalr Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new MensajeaSignalr();
                }

                return instancia;
            }
        }

        #endregion

        #region contructor
        public MensajeaSignalr()
        {
            context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
        }
        #endregion

        #region Recogidas
        public void NotificaAdministradoresRecogidas(ParametrosSignalR mensaje)
        {
            Task taskArray;

            taskArray = new Task(() =>
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
                    var grupo = Convert.ToString((int)COEnumIdentificadorAplicacion.AdministradorRecogidas);
                    mensaje.Mensaje = "Detalle" + mensaje.Mensaje;
                    context.Clients.Group(grupo).enviarNotificacionUsuario(mensaje);
                }
                );

            taskArray.Start();

        }

        public void ActualizaPosicionMensajero(CO.Servidor.Servicios.ContratoDatos.Recogidas.RGDetalleMensajeroBalance.RGDetalleRutaMensajero nuevaPosicion)
        {
            Task taskArray;

            taskArray = new Task(() =>
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
                var grupo = Convert.ToString((int)COEnumIdentificadorAplicacion.AdministradorRecogidas);

                context.Clients.Group(grupo).ActualizaPosicionMensajero(nuevaPosicion);
            });

            taskArray.Start();

        }
        #endregion
    }
}