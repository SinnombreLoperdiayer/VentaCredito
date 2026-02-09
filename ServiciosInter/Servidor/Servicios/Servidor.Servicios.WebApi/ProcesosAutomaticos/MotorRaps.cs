using CO.Servidor.Servicios.Implementacion.Raps;
using CO.Servidor.Servicios.WebApi.Dominio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ProcesosAutomaticos
{
    public class MotorRaps
    {
        private static readonly MotorRaps instancia = new MotorRaps();

        public static MotorRaps Instancia
        {
            get { return MotorRaps.instancia; }
        }

        private static bool procesoMotorRapsVencidasIniciado = false;

        private static bool procesoMotorRapsAutomaticasSinSistemaFuenteIniciado = false;

        private static bool procesoMotorRapsAutomaticasConSistemaFuenteIniciado = false;



        private static bool procesoMotorRapsCorriendo = true;

        private static int contError = 0;

        private MotorRaps()
        {

        }

        /// <summary>
        /// Proceso que se encarga escalar
        /// </summary>
        public void IniciarMotorEscalamientoVencidas()
        {
            if (!procesoMotorRapsVencidasIniciado)
            {
                lock (this)
                {
                    if (!procesoMotorRapsVencidasIniciado)
                    {
                        Task.Factory.StartNew(() =>
                        {


                            while (MotorRaps.procesoMotorRapsCorriendo)
                            {

                                try
                                {
                                    ApiMotorRaps.Instancia.EscalarSolicitudesVencidas();

                                }
                                catch (Exception ex)
                                {
                                    MotorRaps.contError++;
                                    if (MotorRaps.contError >= 5)
                                        MotorRaps.procesoMotorRapsCorriendo = false;
                                    try
                                    {

                                        string archivo = @"c:\logServiciosAutomaticos\logEscalamientoRapsVencidos.txt";
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

                        procesoMotorRapsVencidasIniciado = true;
                    }
                }
            }
        }



        /// <summary>
        /// Proceso que se encarga de crear las solicitudes de los raps automaticos
        /// </summary>
        public void IniciarMotorCreacionRapsAutomaticosSinSistemaFuente()
        {
            if (!procesoMotorRapsAutomaticasSinSistemaFuenteIniciado)
            {
                lock (this)
                {
                    if (!procesoMotorRapsAutomaticasSinSistemaFuenteIniciado)
                    {
                        Task.Factory.StartNew(() =>
                        {


                            while (MotorRaps.procesoMotorRapsCorriendo)
                            {

                                try
                                {
                                    ApiMotorRaps.Instancia.CrearSolicitudesAutomaticasSinSistemaFuente();

                                }
                                catch (Exception ex)
                                {
                                    MotorRaps.contError++;
                                    if (MotorRaps.contError >= 5)
                                        MotorRaps.procesoMotorRapsCorriendo = false;
                                    try
                                    {

                                        string archivo = @"c:\logServiciosAutomaticos\logSolicitudesAutomaticasSinSisFuente.txt";
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

                        procesoMotorRapsAutomaticasSinSistemaFuenteIniciado = true;
                    }
                }
            }
        }


        /// <summary>
        /// Proceso que se encarga de crear las solicitudes de los raps automaticos con sistema fuente
        /// </summary>
        public void IniciarMotorCreacionRapsAutomaticosConSistemaFuente()
        {
            if (!procesoMotorRapsAutomaticasConSistemaFuenteIniciado)
            {
                lock (this)
                {
                    if (!procesoMotorRapsAutomaticasConSistemaFuenteIniciado)
                    {
                        Task.Factory.StartNew(() =>
                        {


                            while (MotorRaps.procesoMotorRapsCorriendo)
                            {

                                try
                                {
                                    ApiMotorRaps.Instancia.CrearSolicitudesAutomaticasConSistemaFuente();

                                }
                                catch (Exception ex)
                                {
                                    /* MotorRaps.contError++;
                                     if (MotorRaps.contError >= 5)
                                         MotorRaps.procesoMotorRapsCorriendo = false;*/
                                    try
                                    {

                                        string archivo = @"c:\logServiciosAutomaticos\logSolicitudesAutomaticasConSisFuente.txt";
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

                        procesoMotorRapsAutomaticasConSistemaFuenteIniciado = true;
                    }
                }
            }
        }


    }
}