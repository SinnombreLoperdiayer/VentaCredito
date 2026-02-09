using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEnumMovTransaccionCaja
  {
    /// <summary>
    /// Movimiento Hecho por un centro de Servicio
    /// </summary>
    [EnumMember]
    CES,

    /// <summary>
    /// Movimiento Hecho por Casa Matriz
    /// </summary>
    [EnumMember]
    EMP,

    /// <summary>
    /// Movimiento Hecho por el Banco
    /// </summary>
    [EnumMember]
    BAN,

    /// <summary>
    /// Movimiento Hecho por Operación Nacional
    /// </summary>
    [EnumMember]
    OPN
  }
}