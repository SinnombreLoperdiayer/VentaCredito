using System;
using System.IO;
using System.Text;

namespace VentaCredito.Transversal.Utilidades
{
    public class LogTraza
    {
        /// <summary>
        /// Meotod para escribir en el log
        /// </summary>
        /// <param name="Mensaje"></param>
        /// <param name="excepcion"></param>
        public static void EscribirLog(string Mensaje, Exception excepcion = null)
        {
            var dia = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            var mes = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var anio = DateTime.Now.Year.ToString();
            var aplicacion = ContextoSitio.Current != null ? ContextoSitio.Current.IdCliente.ToString() : string.Empty;
            var fechaArchivo = string.Format("{0}-{1}-{2}", dia, mes, anio);
            string archivo = @"c:\logExcepciones\logExcepciones_" + fechaArchivo + "_IDAPL_" + aplicacion.ToString() + ".txt";
            FileInfo f = new FileInfo(archivo);
            StreamWriter writer;
            if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
            {
                writer = f.CreateText();
                writer.Close();
            }

            writer = f.AppendText();

            writer.WriteLine(Mensaje);
            if (excepcion != null)
            {
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


                writer.WriteLine(detalleError.ToString() + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            }

            writer.WriteLine("************************************************************************************************************");
            writer.WriteLine("************************************************************************************************************");
            writer.WriteLine("************************************************************************************************************");
            writer.Close();
        }
    }
}
