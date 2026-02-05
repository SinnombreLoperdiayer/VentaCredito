using Framework.Servidor.Excepciones;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using VentaCredito.Transversal;

namespace VentaCredito.Sitio.Comun
{
    public class Util
    {
        public const double EarthRadius = 6371;
        /// <summary>
        /// Calcula la distancia en metros de dos puntos georeferenciados
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double CalcularDistanciaMetros(double latitud1, double longitud1, double latitud2, double longitud2)
        {
            double distance = 0;
            double Lat = (latitud2 - latitud1) * (Math.PI / 180);
            double Lon = (longitud2 - longitud1) * (Math.PI / 180);
            double a = Math.Sin(Lat / 2) * Math.Sin(Lat / 2) + Math.Cos(latitud1 * (Math.PI / 180)) * Math.Cos(latitud2 * (Math.PI / 180)) * Math.Sin(Lon / 2) * Math.Sin(Lon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            distance = EarthRadius * c;
            return distance * 1000;
        }

        /// <summary>
        /// Convierte un stream a un array de bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }


        /// <summary>
        /// Convierte un string a un double
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        public static double ConvertirStringToDouble(string numero)
        {
            if (CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator == ".")
            {
                return Convert.ToDouble(numero.Replace(',', '.'));
            }
            else
            {
                return Convert.ToDouble(numero.Replace('.', ','));
            }
        }
        /// <summary>
        /// Audita la excepcion a un log en disco
        /// </summary>
        /// <param name="ex"></param>
        public static void AuditarExcepcion(Exception ex)
        {

            try
            {
                var dia = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
                var mes = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                var anio = DateTime.Now.Year.ToString();
                var aplicacion = "Servicio Externo" != null ? "Servicio Externo" : string.Empty;
                var fechaArchivo = string.Format("{0}-{1}-{2}", dia, mes, anio);
                string archivo = @"c:\logExcepciones\logExcepcionesApi_" + fechaArchivo + "_IDAPL_" + aplicacion + ".txt";

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
                writer.WriteLine("****************************************WEB API*************************************************************");
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
