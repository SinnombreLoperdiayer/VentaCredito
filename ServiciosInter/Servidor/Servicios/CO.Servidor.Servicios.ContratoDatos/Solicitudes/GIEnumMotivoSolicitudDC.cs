using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que Enumerable que contiene los motivos para crear una solicitud de modificacion
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum GIEnumMotivoSolicitudDC
  {
    /// <summary>
    /// SOLICITUD CAMBIO AGENCIA CREADA
    /// POR EL MÓDULO DE PAGOS
    /// </summary>
    [EnumMember]
    MOTIVO_SOLICITUD_CAMBIO_AGENCIA = 1,

    /// <summary>
    /// SOLICITUD CAMBIO AGENCIA CREADA
    /// POR EL MÓDULO DE PAGOS
    /// </summary>
    [EnumMember]
    MOTIVO_SOLICITUD_CAMBIO_ESTADO = 2,

    /// <summary>
    /// SOLICITUD PARA LA ANULACION
    /// DEL COMPROBANTE DE PAGO
    /// </summary>
    [EnumMember]
    ANULACION_COMPROBANTE_PAGO = 31,

    /// <summary>
    /// Tipo Motivo Devolucion solicitada
    /// por el remitente
    /// </summary>
    [EnumMember]
    SOLICITADA_REMITENTE = 3,

    /// <summary>
    /// Tipo Motivo Devolucion
    /// por Error Operativo
    /// </summary>
    [EnumMember]
    ERROR_OPERATIVO = 6,

    /// <summary>
    /// Tipo Motivo Devolucion por la
    /// agencia sin servicio
    /// </summary>
    [EnumMember]
    AGENCIA_SIN_SERVICIO = 5,

    /// <summary>
    /// Tipo Motivo Devolucion por la
    /// agencia sin dinero
    /// </summary>
    [EnumMember]
    AGENCIA_SIN_DINERO = 4,
  }
}