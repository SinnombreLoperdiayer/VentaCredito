using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiMotorRecogidas : ApiDominioBase
    {
        private static readonly ApiMotorRecogidas instancia = (ApiMotorRecogidas)FabricaInterceptorApi.GetProxy(new ApiMotorRecogidas(), COConstantesModulos.MODULO_RECOGIDAS);

        public static ApiMotorRecogidas Instancia
        {
            get { return ApiMotorRecogidas.instancia; }
        }

        private ApiMotorRecogidas()
        {
        }

        /// <summary>
        /// Vence las solicitudes asignadas y retorna la lista de dispositivos para notificar el vencimiento
        /// </summary>
        /// <returns></returns>
        public void VencerSolicitudesRecogidas()
        {
            try
            {

                List<PADispositivoMovil> dispositivos = FabricaServicios.ServicioRecogidasMotor.VencerSolicitudesRecogidas();
                dispositivos.ForEach(d =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        string mensaje = "{\"title\":\"Recogida Vencida\", \"message\": \"Recogida desasignada debido a que venció el tiempo de espera. \" }";
                        PushGeneral.Instancia.EnviarNotificacionAndroidPAM(d.TokenDispositivo.Trim(), mensaje);
                    });

                });
            }
            catch (Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }
        }

        /// <summary>
        /// Obtiene las recogidas nuevas y canceladas para notificar a los mensajeros y obtiene las recogidas para cambiar de estado a ParaForzar
        /// </summary>
        /// <returns></returns>
        public void NotificarRecogidasNuevasCambiarEstadoParaForzar()
        {
            try
            {
                List<RGRecogidaMotorDC> lstRecogidas = FabricaServicios.ServicioRecogidasMotor.ObtenerRecogidasNuevasNotificarForzar();
                //Notifica las recogidas disponibles
                lstRecogidas.Where(r => r.Accion == EnumAccionMotor.Notificar).ToList().ForEach(r =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        PushGeneral.Instancia.NotificarRecogidaMensajerosMovilPAM(r.IdLocalidadRecogida, true);
                        FabricaServicios.ServicioRecogidasMotor.AumentarContadorNotificacionRecogidas(r.IdSolicitudRecogida, r.EsNueva);
                    });

                });
                //Cambia de estado las recogidas paraForzar
                lstRecogidas.Where(r => r.Accion == EnumAccionMotor.Forzar).ToList().ForEach(r =>
                {
                    RGAsignarRecogidaDC recogida = new RGAsignarRecogidaDC()
                    {
                        IdSolicitudRecogida = r.IdSolicitudRecogida,
                        LocalidadCambio = "11001000",
                        Longitud = "0",
                        Latitud = "0",
                    };

                    ApiRecogidas.Instancia.InsertarEstadoSolRecogidaTraza(recogida, EnumEstadoSolicitudRecogida.ParaForzar);

                });
            }
            catch (Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }

        }

        /// <summary>
        /// Audita errores
        /// </summary>
        /// <param name="ex"></param>
        private void AuditarError(Exception ex)
        {
            try
            {
                string archivo = @"c:\logServiciosAutomaticos\logMotorRecogidas.txt";
                FileInfo f = new FileInfo(archivo);
                StreamWriter writer;
                if (!f.Exists)
                {
                    writer = f.CreateText();
                    writer.Close();
                }
                writer = f.AppendText();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message + "-" + ex.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                if (ex.InnerException != null)
                {
                    sb.AppendLine("    " + ex.InnerException.Message + "-" + ex.InnerException.StackTrace);
                    if (ex.InnerException.InnerException != null)
                    {
                        sb.AppendLine("          " + ex.InnerException.InnerException.Message + "-" + ex.InnerException.InnerException.StackTrace);
                    }
                }
                writer.WriteLine(ex.Message + "-" + ex.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                writer.Close();
            }
            catch
            { }
        }
    }
}