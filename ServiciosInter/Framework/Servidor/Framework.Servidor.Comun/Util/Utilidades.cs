using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.Util
{
    public class UtilidadesFW
    {

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static void AuditarExcepcion(Exception ex, bool esApi = false)
        {

            try
            {
                string archivo = "";

                if (!esApi)
                    archivo = @"c:\logExcepciones\logExcepcionesApi.txt";
                else
                    archivo = @"c:\logExcepciones\logExcepciones.txt";

                FileInfo f = new FileInfo(archivo);
                StreamWriter writer;
                if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                {
                    writer = f.CreateText();
                    writer.Close();
                }

                writer = f.AppendText();
                writer.WriteLine(ExtraerInformacionExcepcion(ex) + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                writer.WriteLine("************************************************************************************************************");
                writer.WriteLine("****************************************CONTROLLER F*************************************************************");
                writer.WriteLine("************************************************************************************************************");
                writer.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Extrae la traza completa del error
        /// </summary>
        /// <param name="excepcion">Excepción</param>
        /// <returns>Traza del error</returns>
        public static string ExtraerInformacionExcepcion(Exception excepcion)
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


    }


}




