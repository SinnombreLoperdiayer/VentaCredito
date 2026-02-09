using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana.Comun
{
  public enum OUEnumTipoDescarga : short
  {
    /// <summary>
    /// Entrega exitosa
    /// </summary>
    ENTREGA_EXITOSA = 1,

    /// <summary>
    /// Entrega mal exitosa
    /// </summary>
    ENTREGA_MAL_DILIGENCIADA = 2,

    /// <summary>
    /// Devolución
    /// </summary>
    DEVOLUCION = 3,
  }
}