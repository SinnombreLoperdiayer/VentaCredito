using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Contiene el tipo de comision
  /// </summary>
  [DataContract]
  public enum CMEnumTipoComision : short
  {
    [EnumMember]
    Entregar = 1,
    [EnumMember]
    Vender = 2,
    [EnumMember]
    Pagar = 3
  }
}