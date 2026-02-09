using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que Enumerable que contiene los estados de los Solicitudes de un Giro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum GIEnumEstadosSolGirosDC
  {
    /// <summary>
    /// Estado activo de la Solicitud del giro
    /// </summary>
    [EnumMember]
    ACTIVA,
    /// <summary>
    ///Estado Aprobado de la Solicitud del Giro
    /// </summary>
    [EnumMember]
    APROBADA,
    /// <summary>
    /// Estado Rechazado de la Solicitud del Giro
    /// </summary>
    [EnumMember]
    RECHAZADA
  }
}