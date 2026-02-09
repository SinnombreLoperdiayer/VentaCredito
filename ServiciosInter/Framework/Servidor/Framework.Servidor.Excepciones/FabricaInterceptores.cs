using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Excepciones
{
  /// <summary>
  /// Retorna las instancias del interceptor
  /// </summary>
  public class FabricaInterceptores
  {
    /// <summary>
    /// Retorna el proxy de la clase interceptada
    /// </summary>
    /// <param name="obj">Instancia del objeto que se desea interceptar</param>
    /// <param name="Modulo">Identificador del módulo</param>
    /// <returns>Objeto interceptado por el proxy</returns>
    public static object GetProxy(object obj, string Modulo)
    {
      return new Interceptor(obj, Modulo).GetTransparentProxy();
    }
  }
}