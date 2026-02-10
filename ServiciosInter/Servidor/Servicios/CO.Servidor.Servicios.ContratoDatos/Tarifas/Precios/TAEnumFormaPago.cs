using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{
  /// <summary>
  /// Identificadores de las formas de pago
  /// </summary>
  public enum TAEnumFormaPago : short
  {
    /// <summary>
    /// Efectivo
    /// </summary>
    EFECTIVO = 1,

    /// <summary>
    /// Crédito
    /// </summary>
    CREDITO = 2,

    /// <summary>
    /// Al Cobro
    /// </summary>
    AL_COBRO = 3,

    /// <summary>
    /// Prepago
    /// </summary>
    PREPAGO = 4,

    /// <summary>
    /// Prepago
    /// </summary>
    MIXTA = -1,
  }
}