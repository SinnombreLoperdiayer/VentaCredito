using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ProcesosAutomaticos
{
    public class MotorRecogidas
    {

        private static readonly MotorRecogidas instancia = new MotorRecogidas();

        public static MotorRecogidas Instancia
        {
            get { return MotorRecogidas.instancia; }
        }

        private MotorRecogidas()
        { }

        private static bool procesoMotorRecogidasVencidasIniciado = false;
        private static bool procesoMotorRecogidasVencidasCorriendo = true;

        private static bool procesoMotorRecogidasNotificadorIniciado = false;
        private static bool procesoMotorRecogidasNotificadorCorriendo = true;


        #region Notificaciones

        /// <summary>
        /// Notifica una nueva recogida solicitada, la notificacion va dirigida a todos los mensajeros de una ciudad con controller App
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <param name="esControllerApp"></param>
        public void NotificarNuevaRecogida(string idCiudad, bool esControllerApp = false,string direccion = "")
        {
            Task.Factory.StartNew(() =>
            {
                PushGeneral.Instancia.NotificarRecogidaMensajerosMovilPAM(idCiudad, esControllerApp,direccion);
            });
        }


        #endregion


        public void CrearSolicitudesRecogidasFijas()
        {
            //ApiRecogidas.Instancia.InsertarRecogidaEsporadica();
        }

        /// <summary>
        /// Proceso que se encarga escalar
        /// </summary>
        public void IniciarMotorRecogidasVencidas()
        {
            if (!procesoMotorRecogidasVencidasIniciado)
            {
                lock (this)
                {
                    if (!procesoMotorRecogidasVencidasIniciado)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            while (MotorRecogidas.procesoMotorRecogidasVencidasCorriendo)
                            {

                                try
                                {
                                    ApiMotorRecogidas.Instancia.VencerSolicitudesRecogidas();

                                }
                                catch (Exception ex)
                                {
                                    try
                                    {

                                        string archivo = @"c:\logServiciosAutomaticos\logMotorRecogidasVencidas.txt";
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

                                System.Threading.Thread.Sleep(60000);
                            }
                        });

                        procesoMotorRecogidasVencidasIniciado = true;
                    }
                }
            }
        }

        public void IniciarNotificadorRecogidasNuevasParaForzar()
        {

            if (!procesoMotorRecogidasNotificadorIniciado)
            {
                lock (this)
                {
                    if (!procesoMotorRecogidasNotificadorIniciado)
                    {
                        Task.Factory.StartNew(() =>
                        {

                            string archivo1 = @"c:\logServiciosAutomaticos\logMotorRecogidasDeb.txt";
                            FileInfo f1 = new FileInfo(archivo1);
                            StreamWriter writer1;
                            if (!f1.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                            {
                                writer1 = f1.CreateText();
                                writer1.Close();
                            }

                            writer1 = f1.AppendText();
                            writer1.WriteLine("Iniciado " + System.Threading.Thread.GetDomainID().ToString() + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                            writer1.Close();

                            CrearContextoMocked();

                            while (MotorRecogidas.procesoMotorRecogidasNotificadorCorriendo)
                            {

                                try
                                {
                                    ApiMotorRecogidas.Instancia.NotificarRecogidasNuevasCambiarEstadoParaForzar();

                                }
                                catch (Exception ex)
                                {
                                    try
                                    {

                                        string archivo = @"c:\logServiciosAutomaticos\logMotorRecogidasNotificador.txt";
                                        FileInfo f = new FileInfo(archivo);
                                        StreamWriter writer;
                                        if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                                        {
                                            writer = f.CreateText();
                                            writer.Close();
                                        }

                                        writer = f.AppendText();
                                        writer.WriteLine(ex.Message + "-" + ex.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                                        if (ex.InnerException != null)
                                        {
                                            writer.WriteLine(ex.InnerException.Message + "-" + ex.InnerException.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                                        }
                                        if (ex.InnerException != null && ex.InnerException.InnerException != null)
                                        {
                                            writer.WriteLine(ex.InnerException.InnerException.Message + "-" + ex.InnerException.InnerException.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                                        }
                                        writer.Close();

                                    }
                                    catch { }

                                }

                                System.Threading.Thread.Sleep(60000);
                            }
                        });

                        procesoMotorRecogidasNotificadorIniciado = true;
                    }
                }
            }




        }

        private static void CrearContextoMocked()
        {
            //mocked channel
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
                Usuario = "MOTOR_RECO"
            });

        }


    }
}