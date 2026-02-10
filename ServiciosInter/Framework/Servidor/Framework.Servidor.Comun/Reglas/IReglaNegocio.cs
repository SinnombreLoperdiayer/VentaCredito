using System.Collections.Generic;

namespace Framework.Servidor.Comun.Reglas
{
  /// <summary>
  /// Interfaz que deben implementar todas las reglas de negocio que se pueden ejecutar a través del motor de reglas
  /// </summary>
  public interface IReglaNegocio
  {
    /// <summary>
    /// Ejecutar regla especifica.
    /// </summary>
    /// <param name="parametrosRegla">Parámetros de entreda y salida de la regla.</param>
    void Ejecutar(IDictionary<string, object> parametrosRegla);
  }
}