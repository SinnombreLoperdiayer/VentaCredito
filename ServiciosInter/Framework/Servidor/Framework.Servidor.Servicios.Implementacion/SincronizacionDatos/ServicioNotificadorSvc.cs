using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Servicios.Contratos;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Framework.Servidor.ParametrosFW;
using System.Threading.Tasks;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;
using Framework.Servidor.ServicioNotificador;
using System.IO;

namespace Framework.Servidor.Servicios.Implementacion.SincronizacionDatos
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ServicioNotificadorSvc : IServicioNotificadorSvc
    {
        private static readonly Dictionary<string, IServicioNotificadorCallBack> clientesNotificador = new Dictionary<string, IServicioNotificadorCallBack>();

        public static Dictionary<string, IServicioNotificadorCallBack> ClientesNotificador
        {
            get { return ServicioNotificadorSvc.clientesNotificador; }
           
        }





        /// <summary>
        /// Agrega la conexion del cliente al diccionario 
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool ConectarNotificador(string idCliente)
        {
            try
            {

                lock (this)
                {
                    if (ClientesNotificador.ContainsKey(idCliente))
                    {
                        ClientesNotificador.Remove(idCliente);

                    }

                    IServicioNotificadorCallBack callback = OperationContext.Current.GetCallbackChannel<IServicioNotificadorCallBack>();
                    ClientesNotificador.Add(idCliente, callback);

                    LogNotificador("ConectarNotificador OK idCliente:" + idCliente);
                    return true;
                }
            }
            catch(Exception ex)
            {

                LogNotificador("ConectarNotificador Excepcion idCliente:" + idCliente, ex);

                return false;
            }
        }


        private void LogNotificador(string origen,Exception ex=null)
        {

            System.Threading.Thread h = new System.Threading.Thread(() =>
                {

                    lock (this)
                    {
                        try
                        {

                            string archivo = @"c:\logNotificador\logConexionNotificador.txt";
                            FileInfo f = new FileInfo(archivo);
                            StreamWriter writer;
                            if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                            {
                                writer = f.CreateText();
                                writer.Close();
                            }                            

                            StringBuilder sb = new StringBuilder();

                            sb.AppendLine("************************************************************************");
                            sb.AppendLine(origen);
                            if (ex != null)
                            {
                                string errorCompleto = ExtraerInformacionExcepcion(ex);
                                sb.AppendLine(errorCompleto);
                            }
                            sb.AppendLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));

                            sb.AppendLine("************************************************************************");

                            writer = f.AppendText();
                            writer.WriteLine(sb.ToString());
                            writer.Close();

                            //File.AppendAllText(archivo, sb.ToString());                            

                        }
                        catch { }
                    }
                });
            h.Start();
        }

        /// <summary>
        /// Extrae la traza completa del error
        /// </summary>
        /// <param name="excepcion">Excepción</param>
        /// <returns>Traza del error</returns>
        private string ExtraerInformacionExcepcion(Exception excepcion)
        {
            //traza completa del error
            StringBuilder detalleError = new StringBuilder();
            Exception excep = excepcion;
            detalleError.AppendLine(excep.Message);
            detalleError.AppendLine("----------------------------------");
            detalleError.AppendLine("Trace Exception :" + excep.StackTrace);
            detalleError.AppendLine("----------------------------------");
            int i = 0;
            while (excep.InnerException != null)
            {
                i += 1;
                excep = excep.InnerException;
                detalleError.AppendLine("----------------------------------");
                detalleError.AppendLine("Mensaje InnerException " + i + ":");
                detalleError.AppendLine(excep.Message);
                detalleError.AppendLine("----------------------------------");
                detalleError.AppendLine("Trace InnerException " + i + " :");
                detalleError.AppendLine(excep.StackTrace);
            }
            return detalleError.ToString();
        }


        /// <summary>
        /// Verifica que un cliente esté conectado, si no esta conectado agrega la conexion al diccionario
        /// </summary>
        /// <param name="idCliente"></param>
        public bool VerificarConexionNotificador(string idCliente)
        {
            try
            {

                lock (this)
                {
                    if (!ClientesNotificador.ContainsKey(idCliente))
                    {
                        IServicioNotificadorCallBack callback = OperationContext.Current.GetCallbackChannel<IServicioNotificadorCallBack>();
                        ClientesNotificador.Add(idCliente, callback);

                        LogNotificador("VerificarConexionNotificador No estaba Conectado idCliente:" + idCliente);
                    }
                    else
                    {
                        LogNotificador("VerificarConexionNotificador Si estaba Conectado idCliente:" + idCliente);
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                LogNotificador("VerificarConexionNotificador Excepcion idCliente:" + idCliente, ex);

                return false;
            }

        }

        public List<string> ObtenerClientesConectados()
        {
            List<Task> tasks = new List<Task>();
            ClientesNotificador.ToList().ForEach(c =>
            //Parallel.ForEach(clientesNotificador,c=>            
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                if (!c.Value.testConexion())
                                {
                                    ClientesNotificador.Remove(c.Key);
                                    LogNotificador("ObtenerClientesConectados fallo test idCliente-c.Key:" + c.Key);
                                }
                                else
                                {
                                    LogNotificador("ObtenerClientesConectados ok test idCliente-c.Key:" + c.Key);
                                }
                            }
                            catch(Exception ex)
                            {
                                LogNotificador("ObtenerClientesConectados " , ex);

                                ClientesNotificador.Remove(c.Key);
                            }
                        }));
                });

           Task.WaitAll(tasks.ToArray());
           return ClientesNotificador.Select(c => c.Key).ToList();
        }

        public bool EnviarNotificacionCliente(string idCliente)
        {
            try
            {
                if (ClientesNotificador.ContainsKey(idCliente))
                {
                    IServicioNotificadorCallBack callback = ClientesNotificador[idCliente];
                    callback.NotificarClienteController("Mensaje enviado a " + idCliente.ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool BloquearClienteOnLine(string idCliente)
        {
            try
            {
                if (ClientesNotificador.ContainsKey(idCliente))
                {
                    IServicioNotificadorCallBack callback = ClientesNotificador[idCliente];
                    callback.BloquearCliente();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void EnviarNoticiasClientes(MEMensajeEnviado mensaje, List<string> clientes)
        {
            clientes.ForEach(c =>
                {
                    try
                    {
                        var clientesEnviarMensaje = ClientesNotificador.Keys.Where(ke => ke.StartsWith(c));

                      foreach (var clienteKey in clientesEnviarMensaje)
                        {
                            if (ClientesNotificador.ContainsKey(clienteKey))
                            {
                                IServicioNotificadorCallBack callback = ClientesNotificador[clienteKey];
                                callback.ServicioNoticias(mensaje);
                            }
                        }
                       
                    }
                    catch
                    {
                       
                    }
                });
        }


        public bool DesBloquearClienteOnLine(string idCliente)
        {
            try
            {
                if (ClientesNotificador.ContainsKey(idCliente))
                {
                    IServicioNotificadorCallBack callback = ClientesNotificador[idCliente];
                    callback.DesBloquearCliente();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Obtiene los centros de servicios que deberian estar en linea
        /// </summary>        
        /// <returns></returns>
        public List<Framework.Servidor.Servicios.ContratoDatos.Notificador.CentrosServiciosLinea> ConsultarCentrosServiciosDeberianEstarLinea()
        {
            return PAParametros.Instancia.ConsultarCentrosServiciosDeberianEstarLinea();
        }


        /// <summary>
        /// Notifica a todos los clientes de una ciudad, que se creó una nueva recogida
        /// </summary>
        /// <param name="idLocalidadNotificacion"></param>
        public void NotificarRecogidaNodeJS(string idRecogida)
        {
            try
            {                
                var recogida = COServicioNotificador.Instancia.ObtenerRecogida(idRecogida);
                if (recogida != null)
                {
             
                    string idLocalidadRecogida = recogida.RecogidaPeaton.IdMunicipio;

                    var clientes = ClientesNotificador.Where(c => c.Key.Split('-')[3] == idLocalidadRecogida);

                    clientes.ToList().ForEach(c =>
                        {
                            IServicioNotificadorCallBack callback = c.Value;
                            callback.NotificarRecogidaClientes(idLocalidadRecogida, recogida.RecogidaPeaton.DireccionCliente, recogida.RecogidaPeaton.NombreCliente);
                        });
                }

            }
            catch(Exception ex)
            {

            }

        }
     
    }
}
