using System;
using System.Net;
using System.ServiceModel.Channels;
using System.Xml;

namespace Framework.Cliente.Comun
{
  /// <summary>
  /// Clase header que viaja en el message WCF
  /// </summary>
  public class CredentialHeader : MessageHeader
  {
    /// <summary>
    /// Gets or sets the usuario.
    /// </summary>
    /// <value>
    /// The usuario.
    /// </value>
    public string Usuario { get; set; }

    /// <summary>
    /// Gets the name of the message header.
    /// </summary>
    /// <returns>The name of the message header.</returns>
    public override string Name
    {
      get { return (CredencialHeaderNames.CREDENCIAL_HEADERNAME); }
    }

    /// <summary>
    /// Gets the namespace of the message header.
    /// </summary>
    /// <returns>The namespace of the message header.</returns>
    public override string Namespace
    {
      get { return (CredencialHeaderNames.CREADENCIAL_HEADER_NAMESPACE); }
    }

    /// <summary>
    /// Called when the header content is serialized using the specified XML writer.
    /// </summary>
    /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter"/>.</param>
    /// <param name="messageVersion">Contains information related to the version of SOAP associated with a message and its exchange.</param>
    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
      writer.WriteElementString(CredencialHeaderNames.KEY_USUARIO, this.Usuario);
    }
  }

  /// <summary>
  ///Clase de constantes con los nombres del header
  /// </summary>
  internal sealed class CredencialHeaderNames
  {
    /// <summary>
    /// CredencialHeader
    /// </summary>
    public const string CREDENCIAL_HEADERNAME = "CredencialHeader";
    /// <summary>
    /// Usuario
    /// </summary>
    public const string KEY_USUARIO = "Usuario";
    /// <summary>
    /// CredencialHeaderNamespace
    /// </summary>
    public const string CREADENCIAL_HEADER_NAMESPACE = "http://controller.com";
  }

  /// <summary>
  /// Credencial de usuario
  /// </summary>
  public sealed class Credencial
  {
    /// <summary>
    /// Crea una instancia de la clase <see cref="Credencial"/>
    /// </summary>
    /// <param name="nombreUsuario"></param>
    public Credencial(string nombreUsuario)
    {
      this.Usuario = nombreUsuario;
    }

    /// <summary>
    /// Retorna el nombre de usuario
    /// </summary>
    public string Usuario
    {
      get;
      private set;
    }
  }
}