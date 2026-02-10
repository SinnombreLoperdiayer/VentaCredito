using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Clientes.Comun
{
  /// <summary>
  /// numeracion de tipos de otro si
  /// </summary>
  public enum CLEnumTipoOtrosi : short
  {
    /// <summary>
    /// Modalidad de tiempo
    /// </summary>
    TIPO_TIEMPO = 1,

    /// <summary>
    /// Modalidad de valor
    /// </summary>
    TIPO_VALOR = 2,

    /// <summary>
    /// Modalidad mixta
    /// </summary>
    TIPO_MIXTO = 3,
  }
}