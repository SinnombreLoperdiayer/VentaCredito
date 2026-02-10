using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Framework.Servidor.Excepciones
{
  public class AuditoriaTrace
  {
    private static TraceSwitch traceSwitch = new TraceSwitch("Auditoria", "Auditorias de Controller");

         
       /// <summary>
    /// Escribe la auditoria a base de datos dependiendo del tipo de auditoria
    /// </summary>
    /// <param name="tipoAuditoria">Indica el tipo de auditoria</param>
    /// <param name="Mensaje">Mensaje a guardar</param>
    /// <param name="excepcion">Excepcion, solo se envia cuando es un error</param>
    public static void EscribirAuditoria(ETipoAuditoria tipoAuditoria, string Mensaje, string Modulo, Exception excepcion = null)
    {
        EscribirAuditoriaParametros(tipoAuditoria, Mensaje, Modulo, excepcion, null, null);
    }


    /// <summary>
    /// Escribe la auditoria a base de datos dependiendo del tipo de auditoria
    /// </summary>
    /// <param name="tipoAuditoria">Indica el tipo de auditoria</param>
    /// <param name="Mensaje">Mensaje a guardar</param>
    /// <param name="excepcion">Excepcion, solo se envia cuando es un error</param>
    public static void EscribirAuditoriaParametros(ETipoAuditoria tipoAuditoria, string Mensaje, string Modulo, Exception excepcion = null, string usuario=null, IMessage msg = null)
    {

        if (string.IsNullOrWhiteSpace(usuario))
        {
            if (ControllerContext.Current != null)
                usuario = ControllerContext.Current.Usuario;
            else
                usuario = "NoUsuario";
        }
      SEAuditoriaErrores objAud = new SEAuditoriaErrores();
      if (excepcion != null)
        objAud.InnerException = ExtraerInformacionExcepcion(excepcion);




      objAud.Mensaje = Mensaje;
      objAud.Modulo = Modulo;

      if (msg != null)
      {
          System.Collections.ArrayList mensajeMetodo = (msg.Properties.Values as System.Collections.ArrayList);
          objAud.NombreMetodo = string.Format("{0}.{1}({2})", ((IMethodCallMessage)msg).MethodBase.ReflectedType.FullName, ((IMethodCallMessage)msg).MethodBase.Name, string.Join(",", ((IMethodCallMessage)msg).MethodBase.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name))));
          objAud.NombreAssembly = mensajeMetodo[3].ToString();
          StringBuilder parametros = new StringBuilder();
          if (mensajeMetodo[2] != null)
          {
              Type[] tipoDatosParametros = (Type[])mensajeMetodo[2];
              object[] valorParametros = (object[])mensajeMetodo[4];

              for (int i = 0; i <= tipoDatosParametros.Count() - 1; i++)
              {
                  parametros.AppendLine(AuditoriaTrace.serializarObjeto(valorParametros[i], tipoDatosParametros[i] as Type));
                  parametros.AppendLine();
                  parametros.AppendLine("/****/");
                  parametros.AppendLine();
              }
          }

          objAud.Parametros = parametros.ToString();
      }
      else
      {
          objAud.NombreMetodo = "";
          objAud.NombreAssembly = "";
          objAud.Parametros = "";
      }
     

      objAud.Usuario = usuario;
      objAud.Tipo = tipoAuditoria.ToString();

      if (tipoAuditoria == ETipoAuditoria.Error)
        Trace.WriteIf(traceSwitch.TraceError, objAud);
      else
        Trace.WriteIf(traceSwitch.TraceInfo, objAud);
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

    public static string serializarObjeto(object objeto, Type tipoObjeto)
    {
        try
        {
            if (tipoObjeto.FullName != "System.Data.SqlClient.SqlConnection" && tipoObjeto.FullName != "System.Data.SqlClient.SqlTransaction")
            {
                var serializer = new DataContractSerializer(tipoObjeto);
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb))
                {
                    serializer.WriteObject(writer, objeto);
                    writer.Flush();
                    return sb.ToString();
                }
            }
            else
            {
                return tipoObjeto.FullName;
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }


  }
}