using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Excepciones.Modelo;
using System.IO;

namespace Framework.Servidor.Excepciones
{
  /// <summary>
  /// Descripcion: Atributo para la intercepcion de las excepciones
  /// Autor: Christian Eduardo Velandia Suarez
  /// Fecha: 19/09/2011
  /// Version: 1.0
  /// Modificado por:
  /// Fecha Modificación:
  /// </summary>
  public class Interceptor : RealProxy, IDisposable
  {
    private object _subject;
    private string _Modulo;

    public Interceptor(object subject, string Modulo)
      : base(subject.GetType())
    {
      //We have to attach our object to the proxy.
      //This isn't an automagic thing, probably because it isn't always relevant.
      AttachServer((MarshalByRefObject)subject);
      _subject = subject;
      _Modulo = Modulo;
    }

    public void Dispose()
    {
      //For proper cleanup, let's detach our server.
      //I am sure this is done in GC, but why risk it?
      DetachServer();
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


    /// Here is the magic of our proxy.
    /// We create a new instance of the class, then get a transparent proxy of our server.
    /// Note that we are returning object, but this is actually of whatever type our obj
    /// parameter is.
    /*public static object Instancia(Object obj)
    {
        return new Interceptor(obj).GetTransparentProxy();
    }*/

    /// In the Invoke Method we do our interception work.
    public override IMessage Invoke(IMessage msg)
    {
      IMessage ReturnMsg = null;
      MethodReturnMessageWrapper Retual = null;
      MarshalByRefObject owner = GetUnwrappedServer();

      string usuario = "";
      if (ControllerContext.Current != null)
          usuario = ControllerContext.Current.Usuario;
      else
          usuario = "NoUsuario";

      if (owner != null)
      {
        DateTime fechaInicio = DateTime.Now;



        ControlExcepcionesHandler.EjecutarAccion(() =>
        {
            
                       
          ReturnMsg = RemotingServices.ExecuteMessage((MarshalByRefObject)_subject, (IMethodCallMessage)msg);
          Retual = new MethodReturnMessageWrapper((IMethodReturnMessage)ReturnMsg);
          if (Retual.Exception != null)
          {
              try
              {
                  string archivo = @"c:\logExcepciones\logExcepciones.txt";
                  FileInfo f = new FileInfo(archivo);
                  StreamWriter writer;
                  if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                  {
                      writer = f.CreateText();
                      writer.Close();
                  }

                  writer = f.AppendText();
                  writer.WriteLine(ExtraerInformacionExcepcion(Retual.Exception)+ "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                  writer.WriteLine("************************************************************************************************************");
                  writer.WriteLine("************************************************************************************************************");
                  writer.WriteLine("************************************************************************************************************");
                  writer.Close();
              }
              catch
              {
              }


            throw Retual.Exception;
          }
        },        
        _Modulo, msg,usuario);

        DateTime fechaFin = DateTime.Now;
        string strMetodo = (msg.Properties.Values as System.Collections.ArrayList)[1].ToString();
       
        string strStackTrace = Environment.StackTrace;
        string strModulo = _Modulo;
      

        if (RepositorioInstrumentacion.Instancia.ModulosInstrumentados != null)
        {
          if (RepositorioInstrumentacion.Instancia.ModulosInstrumentados.Contains(_Modulo))
          {
            Task t = Task.Factory.StartNew(() =>
            {
              Instrumentacion_AUD instrum = new Instrumentacion_AUD()
              {
                INS_FechaInicio = fechaInicio,
                INS_FechaFin = fechaFin,
                INS_TiempoTotalMs = (decimal)(fechaFin - fechaInicio).TotalMilliseconds,
                INS_CallStack = strStackTrace,
                INS_Metodo = strMetodo,
                INS_Modulo = strModulo,
                INS_Usuario = usuario,
                INS_FechaGrabacion = DateTime.Now
              };

              RepositorioInstrumentacion.Instancia.InstrumentarMetodo(instrum);
            });
          }
        }

        return msg = Retual;
      }
      else
      {
        AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_EN_INTERCEPTOR), _Modulo,null,usuario);
        throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_EN_INTERCEPTOR.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_EN_INTERCEPTOR)));
      }
    }
  }
}