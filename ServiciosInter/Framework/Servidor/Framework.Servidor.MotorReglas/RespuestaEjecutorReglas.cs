using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.MotorReglas
{
  /// <summary>
  ///  Repuesta del ejecutor de reglas tras ejecutar una regla
  /// </summary>
  public class RespuestaEjecutorReglas
  {
    private Exception excepcion;

    /// <summary>
    /// Retorna valor indicando si hubo error en la ejecución de la regla.
    /// </summary>
    /// <value>
    ///   <c>true</c> si hubo error; de otra forma, <c>false</c>.
    /// </value>
    public bool HuboError { get;  set; }

    /// <summary>
    /// Retorna la excepción si hubo error en la ejecución de la regla.
    /// </summary>
    public Exception Excepcion
    {
      get { return excepcion; }
       set
      {
        excepcion = value;
        HuboError = true;
      }
    }

    /// <summary>
    /// Retorna los parámetros de entrada y salida que fueron pasados y devueltos por la regla.
    /// </summary>
    public IDictionary<string, object> ParametrosRegla { get; internal set; }

    /// <summary>
    /// Crea una nueva instancia de la clase <see cref="RespuestaEjecutorReglas"/>.
    /// </summary>
    public RespuestaEjecutorReglas()
    {
      HuboError = false;
    }

    /// <summary>
    /// Crea una nueva instancia de la clase <see cref="RespuestaEjecutorReglas"/>.
    /// </summary>
    /// <param name="ParametrosRegla">parámetros que fueron pasados y devueltos por la regla.</param>
    public RespuestaEjecutorReglas(IDictionary<string, object> ParametrosRegla)
      : this()
    {
      this.ParametrosRegla = ParametrosRegla;
    }
  }
}