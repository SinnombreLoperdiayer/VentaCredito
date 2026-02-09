using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Contiene los estados de
  /// la Factura
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEnumEstadoFacturacion
  {
    /// <summary>
    /// Pendiente por facturar
    /// </summary>
    [EnumMember]
    PED,
    /// <summary>
    /// Facturado,
    /// </summary>
    [EnumMember]
    FAC,
    /// <summary>
    /// Factura anulada
    /// </summary>
    [EnumMember]
    ANU
  }
}