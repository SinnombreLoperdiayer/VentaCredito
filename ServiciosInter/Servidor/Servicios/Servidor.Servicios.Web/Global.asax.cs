using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Framework.Servidor.Comun;
using Framework.Servidor.ParametrosFW;

namespace CO.Servidor.Servicios.Web
{
  public class Global : System.Web.HttpApplication
  {
    protected void Application_Start(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(AppServidor.NOMBRE_CULTURA))
      {
        AppServidor.NOMBRE_CULTURA = PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_CULTURA_SERVER);
        AppServidor.CultureInfoServer = new CultureInfo(AppServidor.NOMBRE_CULTURA);
      }
    }

    protected void Session_Start(object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
      Thread.CurrentThread.CurrentCulture = AppServidor.CultureInfoServer;
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
    }

    protected void Application_Error(object sender, EventArgs e)
    {
    }

    protected void Session_End(object sender, EventArgs e)
    {
    }

    protected void Application_End(object sender, EventArgs e)
    {
    }
  }
}