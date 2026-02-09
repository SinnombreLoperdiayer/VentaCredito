using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Tipos de Apertura
  /// </summary>
  public enum CAEnumTipoApertura : short
  {
    /// <summary>
    /// Tipo Centro Servicio
    /// </summary>
    [EnumMember]
    CES = 0,

    /// <summary>
    /// Tipo Casa Matriz
    /// </summary>
    [EnumMember]
    CAM,

    /// <summary>
    /// Tipo Banco
    /// </summary>
    [EnumMember]
    BAN,

    /// <summary>
    /// Tipo Operacion Nacional
    /// </summary>
    [EnumMember]
    OPN
  }
}