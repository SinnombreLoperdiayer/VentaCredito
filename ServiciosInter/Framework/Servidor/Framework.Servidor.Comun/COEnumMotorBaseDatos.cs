using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Enumeración para el motor de base de datos a usar
  /// </summary>
  [Flags]
  public enum COEnumMotorBaseDatos : byte
  {
    /// <summary>
    /// SqlServer
    /// </summary>
    SQLSERVER = 0
  }
}