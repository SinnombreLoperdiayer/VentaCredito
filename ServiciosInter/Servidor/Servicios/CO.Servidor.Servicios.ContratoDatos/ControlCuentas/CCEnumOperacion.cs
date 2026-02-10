using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CCEnumOperacion : short
  {
    [EnumMember]
    Mensajeria = 1,
    [EnumMember]
    Giros = 2,
    [EnumMember]
    OperacionCaja = 3,
    [EnumMember]
    PagoGiro = 4
  }
}