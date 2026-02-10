using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ProcesosAutomaticos
{
    /// <summary>
    /// Controla las notificadiones enviadas a los mensajeros sobre las recogidas
    /// </summary>
    public class NotificadorRecogidas
    {

        private static readonly NotificadorRecogidas instancia = new NotificadorRecogidas();

        public static NotificadorRecogidas Instancia
        {
            get { return NotificadorRecogidas.instancia; }
        }

        private NotificadorRecogidas()
        {
            //IniciarNotificadorAutomaticoRecogidasPeaton();
        }

        private static bool procesoNotificadorPeatonIniciado = false;
        private static bool procesoNotificadorCreditoIniciado = false;
        private static bool procesoNotificadorMicroCreditoIniciado = false;

        private static int contError = 0;

        private static bool procesoNotificacionesCorriendo = true;

        public void IniciarNotificadorAutomaticoRecogidasPeaton()
        {

            lock (this)
            {
                if (!procesoNotificadorPeatonIniciado)
                {
                    Task.Factory.StartNew(() =>
                    {
                    //mucked channel
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
                            Usuario = "AUTOMATICO"
                        });

                        while (procesoNotificacionesCorriendo)
                        {
                            try
                            {

                                if (Convert.ToBoolean(ApiOperacionUrbana.Instancia.ObtenerParametroOperacionUrbana("NotificadorAutoAct")))
                                {
                                //Obtener todas las recogidas peaton  con fecha y hora de recogida, cantidad de paquetes y peso total
                                List<OURecogidasDC> recogidasDia = ApiOperacionUrbana.Instancia.ObtenerRecogidasPeatonPendientesDia();

                                    recogidasDia.ForEach(reco =>
                                    {

                                        int radioCiudad = FabricaServicios.ServicioParametros.ObtenerRadioBusquedaRecogidaLocalidad(reco.LocalidadRecogida.IdLocalidad);

                                        if (reco.VecesNotificadaPush < Convert.ToInt32(ApiOperacionUrbana.Instancia.ObtenerParametroOperacionUrbana("CantidadMaxNotifi")))
                                        {
                                            PushGeneral.Instancia.NotificarRecogidaMensajerosMovilPAM(reco, radioCiudad.ToString());
                                        }
                                        else
                                        {
                                            ApiOperacionUrbana.Instancia.ActualizarEstadoSolicitudRecogida(reco.IdRecogida.Value, OperacionUrbana.Comun.OUEnumEstadoSolicitudRecogidas.IN_PENDIENTE_PROGRAMAR);
                                        }

                                    });
                                }

                                System.Threading.Thread.Sleep(60000);
                            }
                            catch (Exception ex)
                            {
                                NotificadorRecogidas.contError++;
                                if (NotificadorRecogidas.contError >= 5)
                                    NotificadorRecogidas.procesoNotificacionesCorriendo = false;
                                try
                                {

                                    string archivo = @"c:\logServiciosAutomaticos\logNotificaciones.txt";
                                    FileInfo f = new FileInfo(archivo);
                                    StreamWriter writer;
                                    if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                                {
                                        writer = f.CreateText();
                                        writer.Close();
                                    }

                                    writer = f.AppendText();
                                    writer.WriteLine(ex.Message + "-" + ex.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                                    writer.Close();

                                }
                                catch { }
                            }
                        }
                    });

                    procesoNotificadorPeatonIniciado = true;
                }
            }        
        }


        public void IniciarNotificadorAutomaticoRecogidasCredito()
        {
            lock (this)
            {
                if (!procesoNotificadorCreditoIniciado)
                {
                    Task.Factory.StartNew(() =>
                    {

                        while (true)
                        {

                        //Obtener todas las recogidas credito  con fecha y hora de recogida, cantidad de paquetes y peso total



                        System.Threading.Thread.Sleep(60000);
                        }
                    });

                    procesoNotificadorCreditoIniciado = true;
                }
            }
        }


        public void IniciarNotificadorAutomaticoRecogidasMicroCredito()
        {
            lock (this)
            {
                if (!procesoNotificadorMicroCreditoIniciado)
                {

                    Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {

                        //Obtener todas las recogidas Microcredito  con fecha y hora de recogida, cantidad de paquetes y peso total



                        System.Threading.Thread.Sleep(60000);
                        }
                    });

                    procesoNotificadorMicroCreditoIniciado = true;
                }
            }

        }



    }
}
