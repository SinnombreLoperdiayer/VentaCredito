using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract]
  public class SEFaultDetail : DataContractBase
  {
    //- @ErrorCode -//
    /// <summary>
    /// Custom business-specific error code.
    /// </summary>
    [DataMember]
    public Int32 ErrorCode { get; set; }

    //- @Type -//
    /// <summary>
    /// Specifies the type of error.
    /// </summary>
    [DataMember]
    public String Type { get; set; }
  }
}