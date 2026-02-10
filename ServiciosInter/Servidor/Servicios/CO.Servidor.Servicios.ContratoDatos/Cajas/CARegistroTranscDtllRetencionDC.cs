using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase qe contiene la info
  /// de la tbla registro TransaDtllRetencion
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroTranscDtllRetencionDC : DataContractBase
  {
    /// <summary>
    /// Es el Id del registro de transaccion
    /// del detalle de la Caja
    /// </summary>
    /// <value>
    /// The id registro transc DTLL caja.
    /// </value>
    [DataMember]
    public long IdRegistroTranscDtllCaja { get; set; }

    /// <summary>
    /// Es la Info de la Retencion Base-ValorFijo-Valor%.
    /// </summary>
    /// <value>
    /// The info de retencion.
    /// </value>
    [DataMember]
    public CAConceptoCajaRetencionDC InfoDeRetencion { get; set; }
  }
}