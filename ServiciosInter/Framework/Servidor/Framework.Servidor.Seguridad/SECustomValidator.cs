using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Seguridad
{
  public class SECustomValidator : UserNamePasswordValidator
  {
    // This method validates users. It allows in two users,
    // test1 and test2 with passwords 1tset and 2tset respectively.
    // This code is for illustration purposes only and
    // MUST NOT be used in a production environment because it
    // is NOT secure.
    public override void Validate(string userName, string password)
    {
      if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
      {
        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_USER_PASS_WCF_INCORRECTOS), COConstantesModulos.SEGURIDAD);
        throw new ArgumentNullException("Not. Found.");
      }

      bool validado = false;
      try
      {
          validado = true;//SEProveedor.Instancia.ValidarCredencialesWCF(userName, password);
      }
      catch (Exception exc)
      {
        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.Message, COConstantesModulos.SEGURIDAD);
      }
      if (!validado)
      {
        AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_USER_PASS_WCF_INCORRECTOS), COConstantesModulos.SEGURIDAD);
        throw new ArgumentException("Not. Found.");
      }
    }
  }
}