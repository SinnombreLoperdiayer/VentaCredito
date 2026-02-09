using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Contiene el tipo de datos adicionales
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEnumTipoDatosAdicionales
  {
    /// <summary>
    /// Cliente Crédito
    /// </summary>
    [EnumMember]
    CRE,

    /// <summary>
    /// Peatón
    /// </summary>
    [EnumMember]
    PEA,

    /// <summary>
    /// Prepago
    /// </summary>
    [EnumMember]
    PRE
  }
}