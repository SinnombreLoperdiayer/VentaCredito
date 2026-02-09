using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que Enumerable que contiene los estados de los Giros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum GIEnumEstadosGirosDC
  {
    /// <summary>
    /// Estado activo del giro
    /// </summary>
    [EnumMember]
    ACTIVO,
    /// <summary>
    ///Estado Pagado del Giro
    /// </summary>
    [EnumMember]
    PAGADO,
    /// <summary>
    /// Estado en custodia del Giro
    /// </summary>
    [EnumMember]
    CUSTODIA,
    /// <summary>
    /// Estado rezago del giro
    /// </summary>
    [EnumMember]
    REZAGO,

    /// <summary>
    /// Estado Anulado del giro
    /// </summary>
    [EnumMember]
    ANULADO,

    /// <summary>
    /// Estado Bloqeado del Giro
    /// </summary>
    [EnumMember]
    BLOQUEADO,

    /// <summary>
    /// Estado Bloqeado Devuelto
    /// </summary>
    [EnumMember]
    DEVOLUCION,

    /// <summary>
    /// Estado rezago del giro
    /// </summary>
    [EnumMember]
    ACT,
    /// <summary>
    ///Estado Pagado del Giro
    /// </summary>
    [EnumMember]
    PAG,
    /// <summary>
    /// Estado en custodia del Giro
    /// </summary>
    [EnumMember]
    CUS,
    /// <summary>
    /// Estado rezago del giro
    /// </summary>
    [EnumMember]
    REZ,

    /// <summary>
    /// Estado Bloqueado del Giro
    /// </summary>
    [EnumMember]
    BLQ,

    /// <summary>
    /// Estado Anulado del Giro
    /// </summary>
    [EnumMember]
    ANU,

    /// <summary>
    /// Estado devuelto del Giro
    /// </summary>
    [EnumMember]
    DEV,
  }
}