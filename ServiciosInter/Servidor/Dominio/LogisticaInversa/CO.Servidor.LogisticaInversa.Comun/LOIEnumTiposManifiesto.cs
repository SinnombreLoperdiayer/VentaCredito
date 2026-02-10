using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
  public enum LOIEnumTiposManifiesto : short
  {
    /// <summary>
    /// Manifiesto de pruebas de entrega
    /// </summary>
    TIPO_PRUEBAS_ENTREGA = 1,

    /// <summary>
    /// Modalidad de valor
    /// </summary>
    TIPO_FACTURA_GIROS = 2,

    /// <summary>
    /// Manifiesto de comprobantes de pagos
    /// </summary>
    TIPO_COMPROBANTE_PAGO = 3,

    /// <summary>
    /// Manifiesto de facturas de mensajería
    /// </summary>
    TIPO_FACTURA_MENSAJERIA = 4,
  }
}