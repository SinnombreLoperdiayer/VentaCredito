using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ProcesosAutomaticos
{
    public class ValidarRecogidasVencidas
    {

        private static readonly ValidarRecogidasVencidas instancia = new ValidarRecogidasVencidas();

        public static ValidarRecogidasVencidas Instancia
        {
            get { return ValidarRecogidasVencidas.instancia; }
        }

        private ValidarRecogidasVencidas()
        {
        }

        private static bool procesoValidacionIniciado = false;

        private static bool procesoVencidasCorriendo = true;

        private static int contError = 0;

        /// <summary>
        /// Proceso que se encarga de validar las recogidas vencidas, y liberarlas para volver a tomarla
        /// </summary>
        public void ValidarRecogidasVencidasDia()
        {
            lock (this)
            {
                if (!procesoValidacionIniciado)
                {
                    Task.Factory.StartNew(() =>
                    {

                        SeguridadWebApiAttribute.CrearUsuarioContextoAutomatico("AUTOMATICO");

                        while (ValidarRecogidasVencidas.procesoVencidasCorriendo)
                        {

                            List<OURecogidasDC> recogidas = FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidasVencidasMensajerosPAMDia();

                        //Parallel.ForEach(recogidas,r =>
                        recogidas.ForEach(r =>
                            {
                                try
                                {
                                    r.MotivoDescargue = new OUMotivoDescargueRecogidasDC()
                                    {
                                        IdMotivo = (int)OUEnumMotivoDescargueRecogida.VENCIDA
                                    };

                                    r.MotivoDescargue = FabricaServicios.ServicioOperacionUrbana.ObtenerMotivosDescargueRecogidasIdMotivo(r.MotivoDescargue.IdMotivo);


                                    OUDescargueRecogidaMensajeroDC descargue = new OUDescargueRecogidaMensajeroDC()
                                    {
                                        IdAsignacion = r.AsignacionMensajero.IdAsignacion,
                                        IdRecogida = r.IdRecogida.Value,
                                        MotivoDescargue = r.MotivoDescargue
                                    };

                                    FabricaServicios.ServicioOperacionUrbana.GuardarDescargueRecogidaPeaton(descargue);


                                    PADispositivoMovil dispositivoEmpleado = FabricaServicios.ServicioParametros.ObtenerDispositivoMovilEmpleado(r.MensajeroPlanilla.NombreApellido);
                                    if (dispositivoEmpleado != null)
                                    {
                                    //   PushGeneral.Instancia.NotificarRecogidaClienteMovil(dispositivo.TokenDispositivo, mensaje);
                                    string mensaje = "{\"title\":\"Recogida Vencida\", \"message\": \"La recogida en la dirección '" + r.RecogidaPeaton.DireccionCliente + "' ha sido desasignada debido a que venció el tiempo de espera. \" }";

                                        PushGeneral.Instancia.NotificarVencimientoRecogidaMensajeroMovilPAM(dispositivoEmpleado.TokenDispositivo, mensaje);
                                    }


                                }
                                catch (Exception ex)
                                {
                                    ValidarRecogidasVencidas.contError++;
                                    if (ValidarRecogidasVencidas.contError >= 5)
                                        ValidarRecogidasVencidas.procesoVencidasCorriendo = false;
                                    try
                                    {

                                        string archivo = @"c:\logServiciosAutomaticos\logVencidas.txt";
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

                            });


                            System.Threading.Thread.Sleep(60000);
                        }
                    });

                    procesoValidacionIniciado = true;
                }
            }
        }



    }

}
