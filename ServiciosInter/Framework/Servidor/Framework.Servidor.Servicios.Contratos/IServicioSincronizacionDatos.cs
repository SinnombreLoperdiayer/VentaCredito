using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Framework.Servidor.Servicios.ContratoDatos.SincronizadorDatos;

namespace Framework.Servidor.Servicios.Contratos
{
    //[ServiceContract(CallbackContract = typeof(ISincronizacionDatosServiceCallback))]
    [ServiceContract()]
    public interface IServicioSincronizacionDatos
    {
        [OperationContract]
        List<EsquemaDB> ObtenerEsquema(bool armarQueryCreacion);

        [OperationContract]
        EsquemaDB ObtenerEsquemaTabla(string nombreTabla);
        /// <summary>
        /// Obtiene los datos de una tabla
        /// </summary>
        /// <param name="nombreTabla"></param>
        /// <param name="BatchSize"></param>
        /// <param name="filtro"></param>
        [OperationContract]
        List<Registros> ObtenerDatosTabla(string nombreTabla, int BatchSize, string filtro, string actualAnchor, int batchActual, int totalbatch);
        
        [OperationContract]
        string ObtenerArchivoBaseDatosSinEsquema();

        [OperationContract]
        void IniciarServidorNotificaciones();

        [OperationContract]
        List<string> ObtenerPuntosSuscritos();

         /// <summary>
        /// Realiza la sincronizacion de los puntos offline
        /// </summary>
        /// <param name="contenido">el contenido del archivo encriptado</param>
        /// <returns></returns>
        [OperationContract]
        string SincronizacionCentrosServicioOffLine(string contenido);


       /* #region Notificaciones

       
        

        /// <summary>
        /// Subcribes a client for any message broadcast.
        /// </summary>
        /// <returns>An id that will identify a client.</returns>
        [OperationContract]
        void Subscribe(string idCentroServiciosCaja);

        /// <summary>
        /// Unsubscribes a client from any message broadcast.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        [OperationContract(IsOneWay = true)]
        void Unsubscribe(string clientId);

        /// <summary>
        /// Keeps the connection between the client and server.
        /// Connection between a client and server has a time-out,
        /// so the client needs to call this before that happens
        /// to remain connected to the server.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        /// <summary>
        /// Broadcasts a message to other connected clients.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="message">The message to be broadcasted.</param>
        [OperationContract]
        void SendMessage(string clientId, string message);

        #endregion*/

    }

   /* /// <summary>
    /// The callback contract to be implemented by the client
    /// application.
    /// </summary>
    public interface ISincronizacionDatosServiceCallback
    {
        /// <summary>
        /// Implemented by the client so that the server may call
        /// this when it receives a message to be broadcasted.
        /// </summary>
        /// <param name="message">
        /// The message to broadcast.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void HandleMessage(string message);
    }*/
}
