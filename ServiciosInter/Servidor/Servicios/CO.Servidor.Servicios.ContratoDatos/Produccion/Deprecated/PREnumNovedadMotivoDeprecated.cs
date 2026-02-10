using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  /// <summary>
  /// tipos de motivos de Novedades
  /// </summary>
  public enum PREnumNovedadMotivoDeprecated : int
  {
    [EnumMember]
    NOVEDAD_CAMBIO_DE_DESTINO = 1,

    [EnumMember]
    NOVEDAD_CAMBIO_FORMA_DE_PAGO = 2,

    [EnumMember]
    NOVEDAD_POR_ANULACION_DEL_GIRO = 3,

    [EnumMember]
    NOVEDAD_POR_DEVOLUCION_DEL_GIRO = 4
  }
}