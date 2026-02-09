using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
      [DataContract(Namespace = "http://contrologis.com")]
   public enum LIEnumTipoPlanillaNotificacion
    {
           /// <summary>
    /// Tipo de planilla credito
    /// </summary>
    [EnumMember]
    CRE,

    /// <summary>
    /// Tipo de planilla centro servicio
    /// </summary>
    [EnumMember]
    CES
    }
}
