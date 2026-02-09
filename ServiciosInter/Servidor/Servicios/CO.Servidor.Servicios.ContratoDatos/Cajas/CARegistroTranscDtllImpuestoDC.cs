using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la Info de la
  /// tbl RegistroTransDtllImpuesto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroTranscDtllImpuestoDC : DataContractBase
  {
    /// <summary>
    /// Esla info del impuesto
    /// </summary>
    /// <value>
    /// The info impuesto.
    /// </value>
    [DataMember]
    public TAImpuestoDelServicio InfoImpuesto { get; set; }

    /// <summary>
    /// Es el valor del impuesto liquidado
    /// </summary>
    /// <value>
    /// The valor impuesto liquidado.
    /// </value>
    [DataMember]
    public decimal ValorImpuestoLiquidado { get; set; }
  }
}