using System;
using System.Runtime.Serialization;

namespace Framework.Servidor.Excepciones
{
  /// <summary>
  /// Representa errores presentados en la aplicación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ControllerException
  {
    #region Campo

    private string mensaje;

    private string tipoError;

    private string nombreModulo;

    #endregion Campo

    #region Contructor

    /// <summary>
    /// Crea una nueva instancia de la clase <see cref="ControllerException"/>.
    /// </summary>
    /// <param name="nombreModulo">Nombre del modulo</param>
    /// <param name="codigoError"></param>
    /// <param name="mensaje">Mensaje del error</param>
    public ControllerException(string nombreModulo, string codigoError, string mensaje)
    {
      this.nombreModulo = nombreModulo;
      this.tipoError = codigoError;
      this.mensaje = mensaje;
    }

    #endregion Contructor

    #region Propiedad

    /// <summary>
    /// Retorna el mensaje de error
    /// </summary>
    [DataMember]
    public string Mensaje
    {
      get { return mensaje; }
      set { mensaje = value; }
    }

    /// <summary>
    /// Retorna el tipo de error
    /// </summary>
    [DataMember]
    public string TipoError
    {
      get { return tipoError; }
      set { tipoError = value; }
    }

    /// <summary>
    /// Retorna nombre del modulo que genera el error
    /// </summary>
    public string NombreModulo
    {
      get { return nombreModulo; }
      set { nombreModulo = value; }
    }

    #endregion Propiedad
  }
}