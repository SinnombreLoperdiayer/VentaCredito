using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Areas.Comun
{
  /// <summary>
  /// Enumeración de tipos de áreas
  /// </summary>
  public enum AREnumTipoErrorAreas : int
  {
    /// <summary>
    /// Casa Matriz con identificador {0} no está configurada.
    /// </summary>
    ERROR_CASA_MATRIZ_NO_CONFIGURADA = 0,

    /// <summary>
    /// No se puede guardar un Macroproceso sin su código
    /// </summary>
    ERROR_MACROPROCESO_SIN_CODIGO = 1
  }
}