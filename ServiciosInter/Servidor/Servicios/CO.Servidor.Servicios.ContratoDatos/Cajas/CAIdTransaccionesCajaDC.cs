using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que devuelve las transacciones y transacciones
  /// detalle de los movimientos en caja
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAIdTransaccionesCajaDC
  {
    /// <summary>
    /// Es el Id de la tbl RegistroTransaccionesCaja_CAJ
    /// </summary>
    /// <value>
    /// The id transaccion caja.
    /// </value>
    [DataMember]
    public long IdTransaccionCaja { get; set; }

    /// <summary>
    /// Es el Id de la tbl RegistroTransacDetalleCaja_CAJ
    /// </summary>
    /// <value>
    /// The id transaccion caja DTLL.
    /// </value>
    [DataMember]
    public List<long> IdTransaccionCajaDtll { get; set; }

    /// <summary>
    /// Número del consecutivo asociado a la transacción
    /// </summary>
    [DataMember]
    public string NumeroConsecutivo { get; set; }
  }
}