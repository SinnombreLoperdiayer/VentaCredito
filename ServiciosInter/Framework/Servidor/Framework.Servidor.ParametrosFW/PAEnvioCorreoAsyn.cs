using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace Framework.Servidor.ParametrosFW
{
  public class PAEnvioCorreoAsyn
  {
    private static readonly PAEnvioCorreoAsyn instancia = new PAEnvioCorreoAsyn();

    public static PAEnvioCorreoAsyn Instancia
    {
      get { return PAEnvioCorreoAsyn.instancia; }
    }

    public void EnviarCorreoAsyn(string destinatario, string asunto, string mensaje)
    {
      Task.Factory.StartNew(() =>
      {
        try
        {
          PAAdministrador.Instancia.EnviarCorreologisticaInversa(destinatario, asunto, mensaje);
        }
        catch (Exception ex)
        {
          AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.TELEMERCADEO);
        }
      }, TaskCreationOptions.PreferFairness);
    }
  }
}