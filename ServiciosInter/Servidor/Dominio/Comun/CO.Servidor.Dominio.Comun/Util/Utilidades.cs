using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CO.Servidor.Dominio.Comun.Util
{
    public static class Utilidades
    {

        public static string EliminarCaracteresEspeciales(string strIn)
        {
            if (strIn != null)
            {
             string cadena =  Regex.Replace(strIn, @"[^\w]", " ");

             if (cadena.Contains('º'))
             {
                cadena= cadena.Replace('º', ' ');
             }

             return cadena.Trim();

            }
            else
                return strIn;
        }

        public static SqlParameter AddParametro(string parametro, object valor)
        {
            if (valor != null)
            {
                return new SqlParameter(parametro, valor);
            }
            else
                return new SqlParameter(parametro, DBNull.Value);

        }


        public static void AuditarExcepcion(Exception ex,bool esApi=false)
        {

            try
            {
                string archivo = "";

                if(esApi)
                archivo =  @"c:\logExcepciones\logExcepcionesApi.txt";
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

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }


        public static void EjecutarServicioRest(string urlApi,
                            string servicio,
                            Method metodo,
                            dynamic parametros,
                            dynamic informacion,
                            Dictionary<string, Object> header
                            )
        {
            var objJSON = parametros;
            var restClient = new RestClient(urlApi);

            var restRequest = new RestRequest(servicio, metodo);
            restRequest.AddJsonBody(objJSON);
            header.ToList().ForEach(
                h =>
                {
                    restRequest.AddHeader(h.Key, h.Value.ToString());
                }
            );            
            restClient.Execute(restRequest);
        }

    }


    
}


namespace System
{
    public static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        ///<summary>Gets the first week day following a date.</summary>
        ///<param name="date">The date.</param>
        ///<param name="dayOfWeek">The day of week to return.</param>
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }
    }
}
