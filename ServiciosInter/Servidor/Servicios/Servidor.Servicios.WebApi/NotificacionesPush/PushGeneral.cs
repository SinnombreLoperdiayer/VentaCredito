using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.Implementacion;
using PushSharp;
using PushSharp.Android;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace CO.Servidor.Servicios.WebApi.NotificacionesPush
{
    public class PushGeneral
    {

        private static readonly PAParametrosFrameworkSvc servicioParametros = new PAParametrosFrameworkSvc();

        public static PAParametrosFrameworkSvc ServicioParametros
        {
            get { return PushGeneral.servicioParametros; }
        }


        private string credencialGoogleCloudPushPAM = "AIzaSyBcI4eiU4-D_LREw5PeTSB0M-b7A1GwoUE";

        private string credencialGoogleCloudPushClienteRecogidas = "AIzaSyBGaRF2CGKTdqpKbPEuqKqa7-dISveADhQ";



        private static readonly PushGeneral instancia = new PushGeneral();

        public static PushGeneral Instancia
        {
            get { return PushGeneral.instancia; }
        }

        PushBroker pushPAM = new PushBroker();

        PushBroker pushClienteRecogidas = new PushBroker();

        PushBroker pushClienteRecogidasIOS = new PushBroker();

        private PushGeneral()
        {

            //Wire up the events for all the services that the broker registers
            pushPAM.OnNotificationSent += push_OnNotificationSent;
            pushPAM.OnChannelException += push_OnChannelException;
            pushPAM.OnServiceException += push_OnServiceException;
            pushPAM.OnNotificationFailed += push_OnNotificationFailed;
            pushPAM.OnDeviceSubscriptionExpired += push_OnDeviceSubscriptionExpired;
            pushPAM.OnDeviceSubscriptionChanged += push_OnDeviceSubscriptionChanged;
            pushPAM.OnChannelCreated += push_OnChannelCreated;
            pushPAM.OnChannelDestroyed += push_OnChannelDestroyed;
            pushPAM.OnNotificationRequeue += pushPAM_OnNotificationRequeue;

            pushClienteRecogidas.OnNotificationSent += push_OnNotificationSent;
            pushClienteRecogidas.OnChannelException += push_OnChannelException;
            pushClienteRecogidas.OnServiceException += push_OnServiceException;
            pushClienteRecogidas.OnNotificationFailed += push_OnNotificationFailed;
            pushClienteRecogidas.OnDeviceSubscriptionExpired += push_OnDeviceSubscriptionExpired;
            pushClienteRecogidas.OnDeviceSubscriptionChanged += push_OnDeviceSubscriptionChanged;
            pushClienteRecogidas.OnChannelCreated += push_OnChannelCreated;
            pushClienteRecogidas.OnChannelDestroyed += push_OnChannelDestroyed;

            pushClienteRecogidasIOS.OnNotificationSent += push_OnNotificationSent;
            pushClienteRecogidasIOS.OnChannelException += push_OnChannelException;
            pushClienteRecogidasIOS.OnServiceException += push_OnServiceException;
            pushClienteRecogidasIOS.OnNotificationFailed += push_OnNotificationFailed;
            pushClienteRecogidasIOS.OnDeviceSubscriptionExpired += push_OnDeviceSubscriptionExpired;
            pushClienteRecogidasIOS.OnDeviceSubscriptionChanged += push_OnDeviceSubscriptionChanged;
            pushClienteRecogidasIOS.OnChannelCreated += push_OnChannelCreated;
            pushClienteRecogidasIOS.OnChannelDestroyed += push_OnChannelDestroyed;
            pushClienteRecogidasIOS.OnNotificationRequeue += pushPAM_OnNotificationRequeue;


            //  pushPAM.RegisterService<GcmNotification>(new GcmPushService(new GcmPushChannelSettings(this.credencialGoogleCloudPushPAM)), false);
            // pushClienteRecogidas.RegisterService<GcmNotification>(new GcmPushService(new GcmPushChannelSettings(this.credencialGoogleCloudPushClienteRecogidas)), false);

            //push.RegisterGcmService(new GcmPushChannelSettings(this.credencialGoogleCloudPushClienteRecogidas));
            //  push.RegisterGcmService(new GcmPushChannelSettings(this.credencialGoogleCloudPushPAM));
        }



        public void EnviarNotificacionAndroidPAM(List<string> token, string contenido)
        {
            pushPAM.RegisterGcmService(new GcmPushChannelSettings(this.credencialGoogleCloudPushPAM));
            pushPAM.QueueNotification(new GcmNotification().ForDeviceRegistrationId(token).WithJson(contenido));
            pushPAM.StopAllServices(waitForQueuesToFinish: true);
        }
        /// <summary>
        /// Envia notificaion a un dispositivo android
        /// </summary>
        /// <param name="token"></param>
        /// <param name="contenido"></param>
        public void EnviarNotificacionAndroidPAM(string token, string contenido)
        {
            //   push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyAFvwThD3VZLFWfftzK1rMwFMaW8AKM30w"));
            pushPAM.RegisterGcmService(new GcmPushChannelSettings(this.credencialGoogleCloudPushPAM));
            //Fluent construction of an Android GCM Notification
            //IMPORTANT: For Android you MUST use your own RegistrationId 
            //here that gets generated within your Android app itself!



            pushPAM.QueueNotification(new GcmNotification().ForDeviceRegistrationId(token)
           .WithJson(contenido));


            //  string json = "{\"contentTitle\":\"" + contentTitle + "\", \"message\": \"" + message + "\"" + contenido + "}";

            pushPAM.StopAllServices(waitForQueuesToFinish: true);

        }

        /// <summary>
        /// Envia la notificacion a un cliente externo con OS Android
        /// </summary>
        /// <param name="token"></param>
        /// <param name="contenido"></param>
        public void EnviarNotificacionAndroidClienteRecogidas(string token, string contenido)
        {
            pushClienteRecogidas.RegisterGcmService(new GcmPushChannelSettings(this.credencialGoogleCloudPushClienteRecogidas));
            pushClienteRecogidas.QueueNotification(new GcmNotification().ForDeviceRegistrationId(token)
           .WithJson(contenido));
            pushClienteRecogidas.StopAllServices(waitForQueuesToFinish: true);
        }

        /// <summary>
        /// Envia la notificacion a un cliente externo con OS iOS
        /// </summary>
        /// <param name="token"></param>
        /// <param name="contenido"></param>
        public void EnviarNotificacionIOSClienteRecogidas(string token, string contenido)
        {
            try
            {
                string filename = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("RutaCerApple");
                var appleCert = File.ReadAllBytes(filename);
                pushClienteRecogidasIOS.RegisterAppleService(new PushSharp.Apple.ApplePushChannelSettings(false, appleCert, "C0ntr0ll3r"));
                pushClienteRecogidasIOS.QueueNotification(new PushSharp.Apple.AppleNotification().ForDeviceToken(token)
                    .WithAlert(contenido)
                    .WithBadge(1));

            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Notifica a los mensajero que hay una nueva recogida disponible en una ciudad
        /// </summary>
        /// <param name="idCiudad"></param>
        public void NotificarRecogidaMensajerosMovilPAM(string idCiudad, bool esControllerApp = false,string direccion = "")
        {
            try
            {
                List<PADispositivoMovil> dispMovil = ServicioParametros.ObtenerDispositivosMovilesEmpleadosCiudad(idCiudad, esControllerApp);

                Dictionary<string, string> filtro = new Dictionary<string, string>();
                filtro.Add("LOC_IdLocalidad", idCiudad);
                var loc = ServicioParametros.ObtenerLocalidades(filtro, "", 0, 1, true).Lista.FirstOrDefault();
                string nombreCiudad = "";
                if (loc != null)
                    nombreCiudad = loc.Nombre;


                string contenido = "";
                if (string.IsNullOrEmpty(direccion))
                {
                    contenido = "{\"title\":\"Nueva Recogida\", \"message\": \"Una nueva recogida disponible\",\"idLocalidad\":\"" + idCiudad + "\",\"nombreLocalidad\":\"" + nombreCiudad + "\" }";
                }
                else
                {
                    contenido = "{\"title\":\"Nueva Recogida\", \"message\": \"Una nueva recogida disponible en: "+ direccion + " \",\"idLocalidad\":\"" + idCiudad + "\",\"nombreLocalidad\":\"" + nombreCiudad + "\" }";
                }
                PushGeneral.Instancia.EnviarNotificacionAndroidPAM(dispMovil.Select(d => d.TokenDispositivo).ToList(), contenido);


            }
            catch (Exception ex)
            {

            }

        }




        /// <summary>
        /// Notifica a los mensajero que hay una nueva recogida disponible en una ciudad
        /// </summary>
        /// <param name="idCiudad"></param>
        public void NotificarRecogidaMensajerosMovilPAM(OURecogidasDC recogida, string radioRecogida, bool esControllerApp = false)
        {
            try
            {
                List<PADispositivoMovil> dispMovil = ServicioParametros.ObtenerDispositivosMovilesEmpleadosCiudad(recogida.LocalidadRecogida.IdLocalidad, esControllerApp);

                Dictionary<string, string> filtro = new Dictionary<string, string>();
                filtro.Add("LOC_IdLocalidad", recogida.LocalidadRecogida.IdLocalidad);
                var loc = ServicioParametros.ObtenerLocalidades(filtro, "", 0, 1, true).Lista.FirstOrDefault();
                string nombreCiudad = "";
                if (loc != null)
                    nombreCiudad = loc.Nombre;


                string contenido = "{\"title\":\"Nueva Recogida\", \"message\": \"Una nueva recogida disponible\",\"idLocalidad\":\"" + recogida.LocalidadRecogida.IdLocalidad + "\",\"nombreLocalidad\":\"" + nombreCiudad + "\",\"Longitud\":\"" + recogida.LongitudRecogida + "\",\"Latitud\":\"" + recogida.LatitudRecogida + "\",\"RadioRecogida\":\"" + radioRecogida + "\",\"IdRecogida\":\"" + recogida.IdRecogida + "\",\"TipoNotificacion\":\"RecogidaMensajeroPam\" }";
                List<string> tokens = dispMovil.Select(d => d.TokenDispositivo).ToList();
                if (tokens.Count > 0)
                {
                    PushGeneral.Instancia.EnviarNotificacionAndroidPAM(tokens, contenido);

                }

            }
            catch (Exception ex)
            {

            }

        }


        /// <summary>
        /// Notifica al cliente de recogidas cuando un mensajero aceptò su recogia
        /// </summary>
        /// <param name="idCiudad"></param>
        public void NotificarRecogidaClienteRecogidasMovil(string tokenDispositivo, string mensaje, PAEnumOsDispositivo so)
        {
            try
            {
                switch (so)
                {
                    case PAEnumOsDispositivo.Android:
                        {
                            ///agregar los otros sistemas operativos                                          
                            PushGeneral.Instancia.EnviarNotificacionAndroidClienteRecogidas(tokenDispositivo, mensaje);
                            break;
                        }
                    case PAEnumOsDispositivo.WinMobile:
                        throw new NotImplementedException("falta implementar notificaciones windows");
                        break;

                    case PAEnumOsDispositivo.Ios:
                        PushGeneral.Instancia.EnviarNotificacionIOSClienteRecogidas(tokenDispositivo, mensaje);
                        //throw new NotImplementedException("falta implementar notificaciones Ios");
                        break;

                }



            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Notifica al cliente cuando un mensajero aceptò su recogia
        /// </summary>
        /// <param name="idCiudad"></param>
        public void NotificarVencimientoRecogidaMensajeroMovilPAM(string tokenDispositivo, string mensaje)
        {
            try
            {
                PushGeneral.Instancia.EnviarNotificacionAndroidPAM(tokenDispositivo, mensaje);

            }
            catch (Exception ex)
            {

            }

        }



        void push_OnChannelDestroyed(object sender)
        {

        }

        void push_OnChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {

        }

        void push_OnDeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {

        }

        void push_OnDeviceSubscriptionExpired(object sender, string expiredSubscriptionId, DateTime expirationDateUtc, PushSharp.Core.INotification notification)
        {
            try
            {
                PADispositivoMovil dispositivo = new PADispositivoMovil()
                {
                    TokenDispositivo = expiredSubscriptionId
                };
                FabricaServicios.ServicioParametros.InactivarDispositivoMovil(dispositivo);
            }
            catch
            {
            }

        }

        void push_OnNotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {

        }

        void push_OnServiceException(object sender, Exception error)
        {

        }

        void push_OnChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {

        }

        void push_OnNotificationSent(object sender, PushSharp.Core.INotification notification)
        {
            string jsonData = string.Empty;
            if (notification is PushSharp.Apple.AppleNotification)
            {
                jsonData = ((PushSharp.Apple.AppleNotification)(notification)).Payload.Alert.Body;
            }
            if (notification is PushSharp.Android.GcmNotification)
            {
                jsonData = ((PushSharp.Android.GcmNotification)(notification)).JsonData;
            }

            if (!string.IsNullOrEmpty(jsonData))
            {
                dynamic notificacionData = Json.Decode(jsonData);
                string tipoNotificacion = notificacionData.TipoNotificacion;
                if (tipoNotificacion != null)
                {
                    switch (tipoNotificacion)
                    {
                        case "RecogidaMensajeroPam":
                            string id = notificacionData.IdRecogida;
                            if (id != null)
                            {
                                long idRecogida = Convert.ToInt64(id);
                                ApiOperacionUrbana.Instancia.GuardarNotificacionRecogida(idRecogida);
                            }
                            break;
                    }
                }
            }

            //grabar cantidad de notificaciones enviadas

        }

        void pushPAM_OnNotificationRequeue(object sender, PushSharp.Core.NotificationRequeueEventArgs e)
        {

        }




    }
}
