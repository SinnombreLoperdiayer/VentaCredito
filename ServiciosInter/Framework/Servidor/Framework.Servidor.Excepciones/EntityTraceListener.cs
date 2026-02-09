using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Configuration;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones.Modelo;

namespace Framework.Servidor.Excepciones
{
  public class EntityTraceListener : TraceListener
  {
    private const string COLUMN_SEPARATOR = "|";
    private int m_iMaximumRequests;

    //private StringCollection m_objCollection;
    private List<SEAuditoriaErrores> LstErrores;
    StringBuilder msg = new StringBuilder();

    public EntityTraceListener()
    {
      InitializeListener();
    }

    public EntityTraceListener(string r_strListenerName)
      : base(r_strListenerName)
    {
      InitializeListener();
    }

    private void InitializeListener()
    {
      m_iMaximumRequests = int.Parse(WebConfigurationManager.AppSettings["controller.MaximoRequestAuditoria"]);
      //m_objCollection = new StringCollection();
      LstErrores = new List<SEAuditoriaErrores>();
    }

    private void SaveErrors()
    {
      try
      {
        using (ModeloExcepciones contexto = new ModeloExcepciones(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString("ModeloExcepciones")))
        {
          DateTime fecha = DateTime.Now;
          bool grabarCambios = false;

          foreach (SEAuditoriaErrores obj in LstErrores)
          {
              AuditoriaExcepciones_AUD aud = new AuditoriaExcepciones_AUD()
               {
                   AUE_FechaError = obj.FechaError,
                   AUE_Mensaje = obj.Mensaje,
                   AUE_Modulo = obj.Modulo,
                   AUE_StackTrace = obj.StackTrace,
                   AUE_Tipo = obj.Tipo,
                   AUE_Usuario = obj.Usuario,
                   AUE_InnerException = obj.InnerException,
                   AUE_Assembly = obj.NombreAssembly,
                   AUE_Metodo = obj.NombreMetodo,
                   AUE_Parametros = obj.Parametros
               };
            contexto.AuditoriaExcepciones_AUD.Add(aud);
            grabarCambios = true;
          }
          if (grabarCambios)
            contexto.SaveChanges();
        }
      }
      catch (Exception ex)
      {
        GuardarLogEventos(ex);
      }
      finally
      {
        LstErrores.Clear();
      }
    }

    private void SaveErrors(List<SEAuditoriaErrores> errores)
    {
      try
      {
        using (ModeloExcepciones contexto = new ModeloExcepciones(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString("ModeloExcepciones")))
        {
          DateTime fecha = DateTime.Now;
          bool grabarCambios = false;

          foreach (SEAuditoriaErrores obj in errores)
          {
              AuditoriaExcepciones_AUD aud = new AuditoriaExcepciones_AUD()
              {
                  AUE_FechaError = obj.FechaError,
                  AUE_Mensaje = obj.Mensaje,
                  AUE_Modulo = obj.Modulo,
                  AUE_StackTrace = obj.StackTrace,
                  AUE_Tipo = obj.Tipo,
                  AUE_Usuario = obj.Usuario,
                  AUE_InnerException = obj.InnerException,
                  AUE_Assembly = obj.NombreAssembly,
                  AUE_Metodo = obj.NombreMetodo,
                  AUE_Parametros = obj.Parametros
              };
            contexto.AuditoriaExcepciones_AUD.Add(aud);
            grabarCambios = true;
          }
          if (grabarCambios)
            contexto.SaveChanges();
        }
      }
      catch (Exception ex)
      {
        LstErrores = errores;
        GuardarLogEventos(ex);
      }
      finally
      {
        LstErrores.Clear();
      }
    }

    private void GuardarLogEventos(Exception excepcion)
    {
      try
      {
        string str = "LogController";

        if (!EventLog.SourceExists(str))
          EventLog.CreateEventSource(str, str);

        msg.Clear();
        msg.AppendLine("<== Error por el que no se audito: " + AuditoriaTrace.ExtraerInformacionExcepcion(excepcion) + " ==>");
          foreach (SEAuditoriaErrores obj in LstErrores)
          {
              msg.AppendLine(obj.FechaError.ToString() + "  " + obj.Mensaje + "  " + obj.Modulo + "  " + obj.StackTrace + "  " + obj.Tipo + obj.Usuario + "  " + obj.InnerException);
          }

        EventLog evento = new EventLog(str);
        evento.Source = str;
        evento.WriteEntry(msg.ToString(), EventLogEntryType.Error);
      }
      catch
      {
      }
      finally
      {
        LstErrores.Clear();
      }
    }

    private void AddToCollection(string r_strTraceDateTime,
        string r_strTraceCategory,
        string r_strTraceDescription,
        string r_strStackTrace,
        string r_strDetailedErrorDescription)
    {
      string usuarioSinContexto;

      if (OperationContext.Current != null && ControllerContext.Current != null && ControllerContext.Current.Usuario != null)
      {
        usuarioSinContexto = ControllerContext.Current.Usuario;
      }
      else
      {
        usuarioSinContexto = "No_Usuario";
      }

      LstErrores.Add(new SEAuditoriaErrores()
      {
        FechaError = DateTime.Parse(r_strTraceDateTime),
        Mensaje = r_strTraceDescription,
        StackTrace = r_strStackTrace,
        Usuario = usuarioSinContexto,
        Modulo = r_strTraceCategory,
        Tipo = "Info"
      });

      if (LstErrores.Count >= m_iMaximumRequests)
      {
        System.Threading.Tasks.Task.Factory.StartNew(() =>
        {
          SaveErrors(this.LstErrores);
        }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
      }
    }

    private void AddToCollection(SEAuditoriaErrores auditoria)
    {
        LstErrores.Add(new SEAuditoriaErrores()
        {
            FechaError = DateTime.Now,
            Mensaje = auditoria.Mensaje,
            StackTrace = auditoria.StackTrace,
            Usuario = auditoria.Usuario,
            Modulo = auditoria.Modulo,
            Tipo = auditoria.Tipo,
            InnerException = auditoria.InnerException,
            NombreAssembly = auditoria.NombreAssembly,
            Parametros = auditoria.Parametros,
            NombreMetodo = auditoria.NombreMetodo
        });

      if (LstErrores.Count >= m_iMaximumRequests)
      {
        System.Threading.Tasks.Task.Factory.StartNew(() =>
        {
          SaveErrors(this.LstErrores);
        }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
      }
    }

    public override void Write(string message)
    {
      StackTrace objTrace = new StackTrace(true);
      AddToCollection(DateTime.Now.ToString(), "", message, objTrace.ToString(), "");
    }

    public override void Write(object o)
    {
      if (o is SEAuditoriaErrores)
      {
        SEAuditoriaErrores Aut = o as SEAuditoriaErrores;
        StackTrace objTrace = new StackTrace(true);
        Aut.StackTrace = objTrace.ToString();
        AddToCollection(Aut);
      }
    }

    public override void Write(string message, string category)
    {
      StackTrace objTrace = new StackTrace(true);
      AddToCollection(DateTime.Now.ToString(), category, message, objTrace.ToString(), "");
    }

    public override void Write(object o, string category)
    {
      StackTrace objTrace = new StackTrace(true);
      AddToCollection(DateTime.Now.ToString(), category, o.ToString(), objTrace.ToString(), "");
    }

    public override void WriteLine(string message)
    {
      Write(message + "\n");
    }

    public override void WriteLine(object o)
    {
      Write(o.ToString() + "\n");
    }

    public override void WriteLine(string message, string category)
    {
      Write((message + "\n"), category);
    }

    public override void WriteLine(object o, string category)
    {
      Write((o.ToString() + "\n"), category);
    }

    public override void Fail(string message)
    {
      StackTrace objTrace = new StackTrace(true);
      AddToCollection(DateTime.Now.ToString(), "Fail", message, objTrace.ToString(), "");
    }

    public override void Fail(string message, string detailMessage)
    {
      StackTrace objTrace = new StackTrace(true);
      AddToCollection(DateTime.Now.ToString(), "Fail", message, objTrace.ToString(), detailMessage);
    }

    public override void Close()
    {
      System.Threading.Tasks.Task.Factory.StartNew(() =>
      {
        SaveErrors(this.LstErrores);
      }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
    }

    public override void Flush()
    {
      System.Threading.Tasks.Task.Factory.StartNew(() =>
      {
        SaveErrors(this.LstErrores);
      }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
    }



  }
}