using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase con las Propiedades de Registro en Tbl Caja
  /// y Registro en tbl Pin Prepago
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAVenderPinPrepagoDC : DataContractBase
  {
    /// <summary>
    /// Clase de Registo en Pin Prepago
    /// </summary>
    /// <value>
    /// The add pin prepago.
    /// </value>
    [DataMember]
    public CAPinPrepagoDC AddPinPrepago { get; set; }

    /// <summary>
    /// Clase de Ristro en Caja la Venta de in Pun Prepago.
    /// </summary>
    /// <value>
    /// The registro en caja pin prepago.
    /// </value>
    [DataMember]
    public CARegistroTransacCajaDC RegistroEnCajaPinPrepago { get; set; }
  }
}