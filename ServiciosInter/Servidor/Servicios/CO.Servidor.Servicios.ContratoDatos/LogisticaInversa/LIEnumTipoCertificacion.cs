using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum LIEnumTipoCertificacion
  {
    /// <summary>
    /// Tipo de Certificacion entrega
    /// </summary>
    [EnumMember]
    ENT,

    /// <summary>
    /// Tipo de Certificacion devolucion
    /// </summary>
    [EnumMember]
    DEV
  }
}