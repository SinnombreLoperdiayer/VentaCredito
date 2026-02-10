using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.Reglas
{
  /// <summary>
  /// Almacena las claves de los parámetros de la regla genericos.
  /// </summary>
  public class ClavesReglasFramework
  {
    /// <summary>
    /// Indicador (bool) para indicar que la regla tuvo un error.
    /// </summary>
    public const string HUBO_ERROR = "HUBO_ERROR";

    /// <summary>
    /// Resultado de la regla.
    /// </summary>
    public const string RESULTADO = "RESULTADO";

    /// <summary>
    /// Mensaje de error de la regla.
    /// </summary>
    public const string ERROR = "ERROR";

    /// <summary>
    /// Excepción controlada por la regla.
    /// </summary>
    public const string EXCEPCION = "EXCEPCION";
  }
}