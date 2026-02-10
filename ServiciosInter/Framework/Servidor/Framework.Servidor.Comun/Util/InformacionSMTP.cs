using System;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace Framework.Servidor.Comun.Util
{
  public class InformacionSMTP
  {
    /// <summary>
    /// Direccion de correo envia mail
    /// </summary>
    public string Remitente { get; set; }

    /// <summary>
    /// Nombre de la persona que envia el mail
    /// </summary>
    public string DisplayRemitente { get; set; }

    /// <summary>
    /// Password de la cuenta que envia el correo
    /// </summary>
    public string PasswordRemitente { get; set; }

    /// <summary>
    /// Host smpt utilizado
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Puerto para envio de correos
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Habilita SSL
    /// </summary>
    public bool EnableSsl { get; set; }

    //
    // Summary:
    //     Specifies how outgoing email messages will be handled.
    //
    // Returns:
    //     An System.Net.Mail.SmtpDeliveryMethod that indicates how email messages are
    //     delivered.
    public SmtpDeliveryMethod DeliveryMethod { get; set; }

    //
    // Summary:
    //     Gets or sets a System.Boolean value that controls whether the System.Net.CredentialCache.DefaultCredentials
    //     are sent with requests.
    //
    // Returns:
    //     true if the default credentials are used; otherwise false. The default value
    //     is false.
    //
    // Exceptions:
    //   System.InvalidOperationException:
    //     You cannot change the value of this property when an e-mail is being sent.
    public bool UseDefaultCredentials { get; set; }

    /// <summary>
    /// Fecha en la cual se ha insertado por ultima vez en este objeto
    /// </summary>
    public DateTime FechaUltimaGrabacion { get; set; }
  }
}