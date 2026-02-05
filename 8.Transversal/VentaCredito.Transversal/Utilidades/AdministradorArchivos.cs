using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;

namespace VentaCredito.Transversal.Utilidades
{
    public class AdministradorArchivos
    {
        private string nombreArchivo;

        private static AdministradorArchivos instancia = new AdministradorArchivos();

        public static AdministradorArchivos Instancia
        {
            get
            {
                return instancia;
            }
        }

        public AdministradorArchivos()
        {
            nombreArchivo = ConfigurationManager.AppSettings["ArchivoLogServicio"];            
        }

        public void CrearArchivoLogEstandar(NullReferenceException ex, string metodoExcepcion, string nombreClase, string parametros)
        {            
            CreacionCarpetas(ex, metodoExcepcion, nombreClase, parametros);                      
        }

        public void CrearArchivoLogBD(SqlException ex, string metodoExcepcion, string nombreClase, string parametros)
        {            
            CreacionCarpetas(ex, metodoExcepcion, nombreClase, parametros);
        }

        internal void CrearArchivoLogGeneral(Exception ex, string metodoExcepcion, string nombreClase, string parametros)
        {            
            CreacionCarpetas(ex, metodoExcepcion, nombreClase, parametros);
        }

        private void CreacionCarpetas(Exception ex, string metodoExcepcion, string nombreClase, string parametros)
        {
            SqlException excepcionSql = null;
            NullReferenceException excepcionNullReference = null;
            Exception excepcionGeneral = null;
            var innerExcepcion = string.Empty;
            var falla = string.Empty;

            if (ex is SqlException)
            {
                excepcionSql = (SqlException)ex;
                innerExcepcion = excepcionSql.InnerException != null ? excepcionSql.InnerException.Message : "No contiene InnerException";
                falla = excepcionSql.Message;
            }
            else if (ex is NullReferenceException)
            {
                excepcionNullReference = (NullReferenceException)ex;
                innerExcepcion = excepcionNullReference.InnerException != null ? excepcionNullReference.InnerException.Message : "No contiene InnerException";
                falla = excepcionNullReference.Message;
            }
            else
            {
                excepcionGeneral = (Exception)ex;
                innerExcepcion = excepcionGeneral.InnerException != null ? excepcionGeneral.InnerException.Message : "No contiene InnerException";
                falla = excepcionGeneral.Message;
            }
            
            
            var directory = string.Empty;
            directory = System.AppDomain.CurrentDomain.BaseDirectory;

            var carpetaDestino = Path.Combine("C:\\ExcepcionesServicio" + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(carpetaDestino + "\\" + nombreArchivo + ".txt", true);
            var sb = new StringBuilder();
            sb.Append("------------------------ INICIO EXCEPCION --------------------------- ");
            sb.AppendLine();
            sb.Append(DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " Hora: " + DateTime.Now.ToShortTimeString());
            sb.AppendLine();
            sb.Append(" Falla en: " + falla);
            sb.AppendLine();
            sb.Append(" Método: " + metodoExcepcion);
            sb.AppendLine();
            sb.Append(" Valor Parametros: " + parametros);
            sb.AppendLine();
            sb.Append(" Clase: " + nombreClase);
            sb.AppendLine();
            sb.Append(" Pila de error: " + ex.StackTrace);
            sb.AppendLine();
            sb.Append("InnerException: " + innerExcepcion);            
            sb.AppendLine();
            sb.Append("------------------------ FIN EXCEPCION --------------------------- ");

            file.WriteLine(sb.ToString());
            file.Close();
        }

        private static string PrintXML(string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException)
            {
                // Handle the exception
            }

            mStream.Close();
            writer.Close();

            return result;
        }
    }
}
