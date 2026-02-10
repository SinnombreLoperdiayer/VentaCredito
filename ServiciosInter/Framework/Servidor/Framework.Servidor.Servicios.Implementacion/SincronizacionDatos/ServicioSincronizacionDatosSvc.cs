using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Framework.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos;
using Framework.Servidor.SincronizacionDatos;
using System.Threading;
using Hik.Communication.ScsServices.Service;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Samples.Scs.IrcChat.Contracts;

namespace Framework.Servidor.Servicios.Implementacion.SincronizacionDatos
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, MaxItemsInObjectGraph = Int32.MaxValue)]
    public class ServicioSincronizacionDatosSvc : IServicioSincronizacionDatos
    {
        public List<EsquemaDB> ObtenerEsquema(bool armarQueryCreacion)
        {
            return COSincronizacionDatos.Instancia.ObtenerEsquema(armarQueryCreacion);
        }

        public EsquemaDB ObtenerEsquemaTabla(string nombreTabla)
        {
            return COSincronizacionDatos.Instancia.ObtenerEsquemaTabla(nombreTabla);
        }

        /// <summary>
        /// Obtiene los datos de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <param name="BatchSize"></param>
        /// <param name="filtro"></param>
        public List<Registros> ObtenerDatosTabla(string nombreTabla, int BatchSize, string filtro, string actualAnchor, int batchActual, int totalbatch)
        {
            List<Registros> lst = COSincronizacionDatos.Instancia.ObtenerDatosTabla(nombreTabla, BatchSize, filtro, actualAnchor, batchActual, totalbatch);
            return lst;
        }

        public string ObtenerArchivoBaseDatosSinEsquema()
        {
            return COSincronizacionDatos.Instancia.ObtenerArchivoBaseDatosSinEsquema();
        }

        /// <summary>
        /// This object is used to host Chat Service on a SCS server.
        /// </summary>
        private static   IScsServiceApplication _serviceApplication;

        /// <summary>
        /// Chat Service object that serves clients.
        /// </summary>
        private static ChatService _chatService;

        public void IniciarServidorNotificaciones()
        {
            if (_serviceApplication == null || _chatService == null)
            {
                lock (this)
                {
                    if (_serviceApplication == null || _chatService == null)
                    {
                        try
                        {
                            int port = 2543;
                            _serviceApplication = ScsServiceBuilder.CreateService(new ScsTcpEndPoint("192.168.2.218",port));
                            _chatService = new ChatService();
                            _serviceApplication.AddService<IChatService, ChatService>(_chatService);
                            //_chatService.UserListChanged += chatService_UserListChanged;
                            _serviceApplication.Start();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        private void chatService_UserListChanged(object sender, EventArgs e)
        {


           // Dispatcher.Invoke(new Action(UpdateUserList));
        }

        /// <summary>
        /// Updates user list on GUI.
        /// </summary>
        private void UpdateUserList()
        {
            var users = new StringBuilder();
            foreach (var user in ChatService.UserList)
            {
                if (users.Length > 0)
                {
                    users.Append(", ");
                }

                users.Append(user.Nick);
            }

            //lblUsers.Text = users.ToString();
        }


       public List<string> ObtenerPuntosSuscritos()
        {
            return ChatService.UserList.Select(c=>c.Nick).OrderBy(s=>s).ToList();
        }


        /// <summary>
        /// Realiza la sincronizacion de los puntos offline
        /// </summary>
        /// <param name="contenido">el contenido del archivo encriptado</param>
        /// <returns></returns>
       public string SincronizacionCentrosServicioOffLine(string contenido)
       {
           return COSincronizacionDatos.Instancia.SincronizacionCentrosServicioOffLine(contenido);
       }

       /*
        #region Notificaciones

        private static readonly Dictionary<string, ISincronizacionDatosServiceCallback> clients =
            new Dictionary<string, ISincronizacionDatosServiceCallback>();
       

        public void Subscribe(string idCentroServiciosCaja)
        {
            ISincronizacionDatosServiceCallback callback =
                OperationContext.Current.GetCallbackChannel<ISincronizacionDatosServiceCallback>();

           // Guid clientId = Guid.NewGuid();

            if (callback != null)
            {
                lock (clients)
                {
                    if (clients.ContainsKey(idCentroServiciosCaja))
                    {
                        clients.Remove(idCentroServiciosCaja);
                    }
                    clients.Add(idCentroServiciosCaja, callback);
                }
            }

           // return clientId;
        }

        public void Unsubscribe(string clientId)
        {
            lock (clients)
            {
                if (clients.ContainsKey(clientId))
                {
                    clients.Remove(clientId);
                }
            }
        }

        public void KeepConnection()
        {
            // Do nothing.
        }

       public void SendMessage(string clientId, string message)
        {
            BroadcastMessage(clientId, message);
        }


        /// <summary>
        /// Notifies the clients of messages.
        /// </summary>
        /// <param name="clientId">Identifies the client that sent the message.</param>
        /// <param name="message">The message to be sent to all connected clients.</param>
        private void BroadcastMessage(string clientId, string message)
        {
            // Call each client's callback method
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (clients)
                    {
                        List<string> disconnectedClientGuids = new List<string>();

                        foreach (KeyValuePair<string, ISincronizacionDatosServiceCallback> client in clients)
                        {
                            try
                            {
                                client.Value.HandleMessage(message);
                            }
                            catch (Exception)
                            {
                                // TODO: Better to catch specific exception types.                     

                                // If a timeout exception occurred, it means that the server
                                // can't connect to the client. It might be because of a network
                                // error, or the client was closed  prematurely due to an exception or
                                // and was unable to unregister from the server. In any case, we 
                                // must remove the client from the list of clients.

                                // Another type of exception that might occur is that the communication
                                // object is aborted, or is closed.

                                // Mark the key for deletion. We will delete the client after the 
                                // for-loop because using foreach construct makes the clients collection
                                // non-modifiable while in the loop.
                                disconnectedClientGuids.Add(client.Key);
                            }
                        }

                        foreach (string clientGuid in disconnectedClientGuids)
                        {
                            clients.Remove(clientGuid);
                        }
                    }
                }
            );
        }

        #endregion
        */
    }
}
