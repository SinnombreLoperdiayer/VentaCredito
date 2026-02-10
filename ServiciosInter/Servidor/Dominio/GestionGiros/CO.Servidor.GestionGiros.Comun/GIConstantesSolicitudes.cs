using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.GestionGiros.Comun
{
  /// <summary>
  /// Clase que contiene las constantes de Solcitudes y Giro.
  /// </summary>
  public class GIConstantesSolicitudes
  {
    /// <summary>
    /// Identificador del Estado del Giro Activo
    /// </summary>
    public const string ESTADO_GIRO_ACT = "ACT";

    /// <summary>
    /// Identificador del Estado del Giro Activo
    /// </summary>
    public const string ESTADO_GIRO_ACTIVO = "Activo";

    /// <summary>
    /// Identificador del Estado del Giro Pagado
    /// </summary>
    public const string ESTADO_GIRO_PAG = "PAG";

    /// <summary>
    /// Identificador del Estado del Giro Pagado
    /// </summary>
    public const string ESTADO_GIRO_PAGADO = "Pagado";

    /// <summary>
    /// Identificador del Estado del Giro Custodia
    /// </summary>
    public const string ESTADO_GIRO_CUS = "CUS";

    /// <summary>
    /// Identificador del Estado del Giro Custodia
    /// </summary>
    public const string ESTADO_GIRO_CUSTODIA = "Custodia";

    /// <summary>
    /// Identificador del Estado del Giro Anulado
    /// </summary>
    public const string ESTADO_GIRO_ANU = "ANU";

    /// <summary>
    /// Identificador del Estado del Giro Bloqueado
    /// </summary>
    public const string ESTADO_GIRO_ANULADO = "Anulado";

    /// <summary>
    /// Identificador del Estado del Giro Bloqueado
    /// </summary>
    public const string ESTADO_GIRO_BLQ = "BLQ";

    /// <summary>
    /// Identificador del Estado del Giro Bloqueado
    /// </summary>
    public const string ESTADO_GIRO_BLOQUEADO = "Bloqueado";

    /// <summary>
    /// Identificador del Estado del Giro Devolución
    /// </summary>
    public const string ESTADO_GIRO_DEV = "DEV";

    /// <summary>
    /// Identificador del Estado del Giro Bloqueado
    /// </summary>
    public const string ESTADO_GIRO_DEVOLUCION = "Devolucion";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_ACT = "ACT";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_INA = "INA";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_ACTIVA = "Activo";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_APR = "APR";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_APROBADA = "Aprobada";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_REC = "REC";

    /// <summary>
    /// Identificador del Estado de la Solicitud
    /// </summary>
    public const string ESTADO_SOL_RECHAZADA = "Rechazada";

    /// <summary>
    /// Id del tipo de Solicitud por
    /// Devolucion
    /// </summary>
    public const int SOL_POR_DEVOLUCION = 1;

    /// <summary>
    /// Id del tipo de Solicitud cambio de
    /// destinatario
    /// </summary>
    public const int SOL_POR_CAMBIO_DEST = 2;

    /// <summary>
    /// Id del tipo de Solicitud por
    /// Cambio Agencia
    /// </summary>
    public const int SOL_POR_CAMBIO_AGENCIA = 3;

    /// <summary>
    /// Id del tipo de Solicitud por
    /// Cambio Estado Custodia a Sin Cancelar
    /// </summary>
    public const int SOL_POR_CAMBIO_ESTADO = 4;

    /// <summary>
    /// Id Servicio de Giros
    /// </summary>
    public static int SERVICIO_GIRO = 8;

    /// <summary>
    /// Valor defecto Ocupacion
    /// </summary>
    public const string VALOR_POR_DEFECTO_OCUPACION = "OCUPACION NO DEFINIDA";

    /// <summary>
    /// Valor Generico RACOL
    /// </summary>
    public const long VALOR_GRAL_RACOL = 9;

    /// <summary>
    /// Asunto del mail de Respuesta de una solicitud
    /// </summary>
    public const int ASUNTO_TIPO_DE_CONTENIDO_MAIL = 2;

    /// <summary>
    /// Contenido del Mail de Rta de una solicitud
    /// </summary>
    public const string ASUNTO_MAIL_RTA_SOLICITUD = "Respuesta Solicitud N° ";

    /// <summary>
    /// Contenido del Mail de Rta de una solicitud
    /// </summary>
    public const string CONTENIDO_MAIL_RTA_SOLICITUD = "Buen dia, la solicitud colocada por ud. ya a sido gestionada y fue: ";

    /// <summary>
    /// Contenido final del Mail de Rta de una solicitud
    /// </summary>
    public const string CONTENIDO_FINAL_MAIL_RTA_SOLICITUD = " ,Cordialmente, Inter Rapidisimo.";

    /// <summary>
    /// Valor Tipo de Consecutivo
    /// </summary>
    public const int TIPO_CONSECUTIVO_SOLICITUD = 1;

    /// <summary>
    /// Contenido final del Mail de Rta de una solicitud
    /// </summary>
    public const string GIRO_TRANSMITIDO = "Transmitido Agencia Manual";

    /// <summary>
    /// Giro peaton peaton
    /// </summary>
    public const string GIROPEATONAPEATON = "PP";

    /// <summary>
    /// id concepto de pago (cajas)
    /// </summary>
    public const string IDCONCEPTOPAGO = "IdConceptoPago";

    /// <summary>
    /// Es la descripcion en la observación por Devolucion
    /// </summary>
    public const string OBSERVACION_DEVOLUCION = "-DEVOLUCION- ";

    /// <summary>
    /// Es la descripcion en la observación por Devolucion
    /// </summary>
    public const string MOTIVO_SOLICITUD_ANULACION_COMPROBANTE = "Anulación de Comprobante de Pago";
  }
}