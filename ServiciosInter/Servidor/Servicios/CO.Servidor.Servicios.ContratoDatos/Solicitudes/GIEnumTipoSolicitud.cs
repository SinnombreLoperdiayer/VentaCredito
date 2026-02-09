using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase con los Enumeradores de los Tipos de Solicitudes Fijos
  /// </summary>
  public enum GIEnumTipoSolicitud : int
  {
    /// <summary>
    /// Tipo de Solicitud Autorización de Devolución
    /// </summary>
    [EnumMember]
    AUTORIZACION_DEVOLUCION = 1,

    /// <summary>
    /// Tipo de Solicitud Modificación Datos del Destinatario
    /// </summary>
    [EnumMember]
    MODIFICACION_DATOS_DESTINATARIO = 2,

    /// <summary>
    /// Tipo de Solicitud Cambio de Agencia
    /// </summary>
    [EnumMember]
    CAMBIO_AGENCIA = 3,

    /// <summary>
    /// Tipo de Solicitud Cambio Estado Custodia a Sin Cancelar
    /// </summary>
    [EnumMember]
    CAMBIO_ESTADO_CUSTODIA_SIN_CANCELAR = 4,

    /// <summary>
    /// Tipo de Solicitud Cambio Estado Rezago a Sin Cancelar
    /// </summary>
    [EnumMember]
    CAMBIO_ESTADO_REZAGO_SIN_CANCELAR = 5,

    /// <summary>
    /// Tipo de Solicitud Anulación de Giro
    /// </summary>
    [EnumMember]
    ANULACION_GIRO = 6,

    /// <summary>
    /// Tipo de Solicitud Anulación de Comprobante de Pago
    /// </summary>
    [EnumMember]
    ANULACION_COMPROBANTE_PAGO = 7,

    /// <summary>
    /// Tipo de Solicitud Ajuste a Valor de Giro
    /// </summary>
    [EnumMember]
    AJUSTE_VALOR_GIRO = 8
  }
}