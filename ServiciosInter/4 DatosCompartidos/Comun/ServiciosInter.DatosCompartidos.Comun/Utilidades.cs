using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ServiciosInter.DatosCompartidos.Comun
{
    public class Utilidades
    {
        private Assembly m_assembly = Assembly.Load("ServiciosInter.DatosCompartidos.Comun"); //Assembly.LoadFrom("ContratosCanalesVenta.dll");

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


        public static void AuditarExcepcion(Exception ex,string adicional = "")
        {

            try
            {
                string archivo = @"C:\LogExcepciones\LogExcepcionesServInter.txt";                

                FileInfo f = new FileInfo(archivo);
                StreamWriter writer;
                if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                {
                    writer = f.CreateText();
                    writer.Close();
                }

                writer = f.AppendText();
                writer.WriteLine(ExtraerInformacionExcepcion(ex) + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                if (!string.IsNullOrWhiteSpace(adicional))
                {
                    writer.WriteLine("Data Adicional: "+ adicional);
                }
                writer.WriteLine("************************************************************************************************************");
                writer.WriteLine("****************************************SERVICIOS INTER*************************************************************");
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

        public Dictionary<string, object> MapearCamposImpresion<T>(T objeto) where T : new()
        {

            Dictionary<string, object> mapeo = new Dictionary<string, object>();
            Type tipo = m_assembly.GetType("ServiciosInter.DatosCompartidos.Comun." + objeto.ToString());
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) || property.PropertyType.BaseType != typeof(object))
                {
                    if (!mapeo.Keys.Contains("{" + property.Name + "}"))
                    {
                        if (property.PropertyType == typeof(decimal))
                        {
                            var valor = String.Format("{0:n0}", (long)(Math.Round((decimal)property.GetValue(objeto), 0)));
                            mapeo.Add("{" + property.Name + "}", valor);
                        }
                        else
                        {
                            mapeo.Add("{" + property.Name + "}", property.GetValue(objeto));
                        }
                    }
                }
                else if (property.PropertyType.Namespace == "System.Collections.Generic")
                {
                    var values = property.GetValue(objeto) as IEnumerable;
                    var subObjeto = property.GetValue(objeto);
                    if (subObjeto != null)
                    {
                        foreach (var item in values)
                        {
                            IList<PropertyInfo> subProperties = item.GetType().GetProperties().ToList();
                            foreach (var subProperty in subProperties)
                            {
                                mapeo.Add("{" + property.Name + subProperty.Name + "}", subProperty.GetValue(item));
                            }
                        }
                    }
                }
                else
                {
                    object subObjeto = property.GetValue(objeto);
                    if (subObjeto != null)
                    {
                        IList<PropertyInfo> subProperties = subObjeto.GetType().GetProperties().ToList();
                        foreach (var subProperty in subProperties)
                        {
                            object valor = subProperty.GetValue(subObjeto);
                            Type t = null;
                            Type tipoValor = null;
                            if (valor != null)
                            {
                                t = valor.GetType();
                                tipoValor = valor.GetType().BaseType;
                            }
                            if ((t != null && t == typeof(string)) || (tipoValor != null && tipoValor != typeof(object)))
                            {
                                mapeo.Add("{" + property.Name + subProperty.Name + "}", subProperty.GetValue(subObjeto));
                            }                            
                        }
                    }
                }
            }
            return mapeo;
        }

    }
}