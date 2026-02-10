using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Framework.Servidor.Comun.Util
{
    /// <summary>
    /// Clase para el envío de correo electrónico
    /// </summary>
    public class CorreoElectronico
    {
        #region Singleton

        private static readonly CorreoElectronico instancia = new CorreoElectronico();

        /// <summary>
        /// Retorna una instancia de la clase
        /// </summary>
        public static CorreoElectronico Instancia
        {
            get { return CorreoElectronico.instancia; }
        }

        #endregion Singleton

        /// <summary>
        /// Informacion necesaria para enviar el correo electronico
        /// </summary>
        private InformacionSMTP informacionSMTP;

        public InformacionSMTP InformacionSMTP
        {
            get { return informacionSMTP; }
            set { informacionSMTP = value; }
        }



        /// <summary>
        /// Enviar correo electrónico
        /// </summary>
        /// <param name="remitente">Correo del remitente</param>
        /// <param name="destinatario">Correos del destinatario ej: a@empresa, b@empresa2</param>
        /// <param name="asunto">Asunto del correo electrónico</param>
        /// <param name="mensajes">Cuerpo del mensaje del correo electrónico</param>
        /// <param name="displayRemitente">Display del remitentte a mostrar si no se envia se utilizara el configurado en el archivo xml</param>
        /// <param name="Nombreadjuntos">Nombre del archivo a adjuntar si no se envia se utilizara el configurado en el archivo xml</param>
        /// <param name="passwordRemitente">Password del remitente si no se envia se utilizara el configurado en el archivo xml </param>
        public void Enviar(string destinatario, string asunto, string mensajes, string remitente = null, string displayRemitente = null, string passwordRemitente = null)
        {
            ValidarDestinatarioMensaje(destinatario, mensajes);

            SmtpClient smtp = CrearSMTP(remitente, displayRemitente, passwordRemitente);
            using (var message = new MailMessage(informacionSMTP.Remitente, destinatario)
            {
                Subject = asunto == null ? string.Empty : asunto,
                Body = mensajes,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Enviar correo electrónico con adjuntos
        /// </summary>
        /// <param name="remitente">Correo del remitente</param>
        /// <param name="destinatario">Correo del destinatario</param>
        /// <param name="asunto">Asunto del correo electrónico</param>
        /// <param name="mensajes">Cuerpo del mensaje del correo electrónico</param>
        /// <param name="adjuntos">Ruta del archivo Adjuntos al correo</param>
        /// <param name="displayRemitente">Display del remitentte a mostrar si no se envia se utilizara el configurado en el archivo xml</param>
        /// <param name="Nombreadjuntos">Nombre del archivo a adjuntar si no se envia se utilizara el configurado en el archivo xml</param>
        /// <param name="passwordRemitente">Password del remitente si no se envia se utilizara el configurado en el archivo xml </param>
        public void Enviar(string destinatario, string asunto, string mensajes, string nombreAdjuntos, string remitente = null, string displayRemitente = null, string passwordRemitente = null)
        {
            ValidarDestinatarioMensaje(destinatario, mensajes);

            SmtpClient smtp = CrearSMTP(remitente, displayRemitente, passwordRemitente);

            // Crea el archivo adjunto para ser enviado en el correo
            Attachment data = new Attachment(nombreAdjuntos, MediaTypeNames.Application.Octet);
            // Adiciona la informacion de tiempos al archivo
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(nombreAdjuntos);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(nombreAdjuntos);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(nombreAdjuntos);

            using (var message = new MailMessage(informacionSMTP.Remitente, destinatario)
            {
                Subject = asunto,
                Body = mensajes
            })
            {
                message.Attachments.Add(data);
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Enviar correo electrónico con adjuntos
        /// </summary>
        /// <param name="remitente">Correo del remitente</param>
        /// <param name="destinatario">Correo del destinatario</param>
        /// <param name="asunto">Asunto del correo electrónico</param>
        /// <param name="mensajes">Cuerpo del mensaje del correo electrónico</param>
        /// <param name="adjuntos">Ruta del archivo Adjuntos al correo</param>
        /// <param name="displayRemitente">Display del remitentte a mostrar si no se envia se utilizara el configurado en el archivo xml</param>
        /// <param name="Nombreadjuntos">Nombre del archivo a adjuntar si no se envia se utilizara el configurado en el archivo xml</param>
        /// <param name="passwordRemitente">Password del remitente si no se envia se utilizara el configurado en el archivo xml </param>
        public void Enviar(string destinatario, string asunto, string mensajes, Stream adjuntos, string remitente = null, string displayRemitente = null, string passwordRemitente = null)
        {
            ValidarDestinatarioMensaje(destinatario, mensajes);

            SmtpClient smtp = CrearSMTP(remitente, displayRemitente, passwordRemitente);

            // Crea el archivo adjunto para ser enviado en el correo
            Attachment data = new Attachment(adjuntos, MediaTypeNames.Application.Octet);

            using (var message = new MailMessage(informacionSMTP.Remitente, destinatario)
            {
                Subject = asunto,
                Body = mensajes
            })
            {
                message.Attachments.Add(data);
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Envio de adjuntos por mail
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="asunto"></param>
        /// <param name="mensajes"></param>
        /// <param name="NombreAdjunto"></param>
        /// <param name="remitente"></param>
        /// <param name="displayRemitente"></param>
        /// <param name="passwordRemitente"></param>
        public void EnviarAdjunto(string destinatario, string asunto, string mensajes, string NombreAdjunto, string remitente = null, string displayRemitente = null, string passwordRemitente = null)
        {
            ValidarDestinatarioMensaje(destinatario, mensajes);

            using (SmtpClient smtp = CrearSMTP(remitente, displayRemitente, passwordRemitente))
            {
                using (FileStream archivo = new FileStream(NombreAdjunto, FileMode.Open, FileAccess.Read))
                {
                    // Crea el archivo adjunto para ser enviado en el correo
                    Attachment data = new Attachment(archivo, Path.GetFileName(NombreAdjunto));

                    using (MailMessage message = new MailMessage(informacionSMTP.Remitente, destinatario) { Subject = asunto, Body = mensajes })
                    {
                        message.Attachments.Add(data);

                        smtp.Send(message);
                    }
                }
            }
        }

        /// <summary>
        /// Creacion del SMTP para envio de correos
        /// </summary>
        /// <param name="remitente"></param>
        /// <param name="displayRemitente"></param>
        /// <param name="passwordRemitente"></param>
        /// <returns></returns>
        public SmtpClient CrearSMTP(string remitente, string displayRemitente, string passwordRemitente)
        {
            if (informacionSMTP == null || informacionSMTP.FechaUltimaGrabacion.AddDays(1) > DateTime.Now)
            {
                ConsultarParametrosSMTP();
            }

            informacionSMTP.Remitente = remitente != null ? remitente : informacionSMTP.Remitente;
            informacionSMTP.DisplayRemitente = displayRemitente != null ? displayRemitente : informacionSMTP.DisplayRemitente;
            informacionSMTP.PasswordRemitente = passwordRemitente != null ? passwordRemitente : informacionSMTP.PasswordRemitente;

            return new SmtpClient
            {
                Host = informacionSMTP.Host,
                Port = informacionSMTP.Port,
                EnableSsl = informacionSMTP.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = informacionSMTP.UseDefaultCredentials,
                Credentials = new NetworkCredential((new MailAddress(informacionSMTP.Remitente, informacionSMTP.DisplayRemitente)).Address, informacionSMTP.PasswordRemitente)
            };
        }


        public SmtpClient CrearSMTP()
        {
            if (informacionSMTP == null || informacionSMTP.FechaUltimaGrabacion.AddDays(1) > DateTime.Now)
            {
                ConsultarParametrosSMTP();
            }

            return new SmtpClient
            {
                Host = informacionSMTP.Host,
                Port = informacionSMTP.Port,
                EnableSsl = informacionSMTP.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = informacionSMTP.UseDefaultCredentials,
                Credentials = new NetworkCredential(informacionSMTP.Remitente, informacionSMTP.PasswordRemitente)
            };
        }



        /// <summary>
        /// consulta los parametros del smtp de un archivo
        /// </summary>
        /// <param name="pXmlizedString"></param>
        /// <returns></returns>
        private void ConsultarParametrosSMTP()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/ConfiguracionCorreoElec.xml"))
            {
                XDocument documento = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/ConfiguracionCorreoElec.xml");
                XmlSerializer xs = new XmlSerializer(typeof(InformacionSMTP));
                MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(documento.ToString()));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                informacionSMTP = (InformacionSMTP)xs.Deserialize(memoryStream);
                informacionSMTP.FechaUltimaGrabacion = DateTime.Now;
            }
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Validar los correos del destinatario
        /// valida que existea un mensaje
        /// </summary>
        /// <param name="destinatario">correos del destinatario</param>
        /// <param name="mensajes">mensaje del email</param>
        private void ValidarDestinatarioMensaje(string destinatario, string mensajes)
        {
            if (String.IsNullOrWhiteSpace(mensajes))
            {
                throw new Exception("Error en Enviar Correo electrónico, No se puede enviar el correo electrónico debido a que no hay mensaje");
            }

            if (String.IsNullOrWhiteSpace(destinatario))
            {
                throw new Exception("Error en Enviar Correo electrónico, No se puede enviar el correo electrónico debido a que no hay correo de los destinatarios");
            }
            else
            {
                string[] vecMail = destinatario.Split(',');
                foreach (string mail in vecMail)
                {
                    if (EsValidoEmail(mail.Trim()) == false)
                    {
                        throw new Exception("Error en Enviar Correo electrónico, El correo electrónico del destinatario es inválido: " + mail);
                    }
                }
            }
        }

        /// <summary>
        /// Definir si la cadena es una dirección de mail valida
        /// </summary>
        /// <param name="strEmail">mail a validar</param>
        /// <returns>true: es valido el correo</returns>
        public bool EsValidoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}