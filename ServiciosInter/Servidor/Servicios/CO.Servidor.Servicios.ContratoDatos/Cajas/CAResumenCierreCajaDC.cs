using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la lista de
  /// los conceptas de caja Asociados al cierre
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAResumenCierreCajaDC : DataContractBase
  {
    /// <summary>
    /// Gets or sets the id cierre caja.
    /// </summary>
    /// <value>
    /// Identificador del cierre de caja de cada caja del punto.
    /// </value>
    [DataMember]
    public long IdCierreCaja { get; set; }

    /// <summary>
    /// Gets or sets the id caja.
    /// </summary>
    /// <value>
    /// The id caja.
    /// </value>
    [DataMember]
    public int IdCaja { get; set; }
  }
}