using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  /// <summary>
  /// Clase de Recaudo Dinero de Al Cobros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADRecaudarDineroAlCobroDC : DataContractBase
  {
    /// <summary>
    /// Informacion de la guia de la que se recauda
    /// el dinero.
    /// </summary>
    /// <value>
    /// The info guia recaudada.
    /// </value>
    [DataMember]
    public ADGuia InfoGuiaRecaudada { get; set; }

    /// <summary>
    /// Clase para registrar el dinero en caja.
    /// </summary>
    /// <value>
    /// The movimiento caja.
    /// </value>
    [DataMember]
    public CARegistroTransacCajaDC MovimientoCaja { get; set; }

    [DataMember]
    public long IdCodigoUsuario { get; set; }
  }
}