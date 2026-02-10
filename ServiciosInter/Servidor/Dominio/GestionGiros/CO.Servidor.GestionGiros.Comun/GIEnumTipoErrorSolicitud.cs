using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.GestionGiros.Comun
{
  /// <summary>
  /// Enumeriacion con los codigos de mensajes de Solicitudes
  /// </summary>
  public enum GIEnumTipoErrorSolicitud : int
  {
    /// <summary>
    /// Error no se encuentra Configurado.
    /// </summary>
    EL_ERROR_NO_SE_ENCUENTRA_CONFIGURADO = 0,

    /// <summary>
    /// Mensaje de Error referente al giro
    /// con Solicitudes con estado Activo
    /// </summary>
    GIRO_CON_SOLICITUDES_EN_ESTADO_ACTIVO = 1,

    /// <summary>
    /// Error del giro con estado Pago
    /// </summary>
    GIRO_CON_ESTADO_PAGO = 2,

    /// <summary>
    /// Error al Adicionar la Infrormacion del Giro
    /// </summary>
    ERROR_ADICIONAR_NVA_SOLICITUD = 3,

    /// <summary>
    /// Error al Cargar los Archivos
    /// </summary>
    ERROR_AL_CARGAR_LOS_ARCHIVOS = 4,

    /// <summary>
    /// Error al no encontrar el giro
    /// </summary>
    ERROR_AL_NO_ENCONTRAR_GIRO = 5,

    /// <summary>
    /// Error al no encontrar el detalle
    /// de la solicitud
    /// </summary>
    ERROR_AL_TRAER_DETALLE_SOLICITUD = 6,

    /// <summary>
    /// Error agencia no acta
    /// para realizar pago
    /// </summary>
    ERROR_AGENCIA_NO_ACTA_PARA_PAGO = 7,

    /// <summary>
    /// Error Giro ya pago
    /// </summary>
    ERROR_GIRO_YA_PAGO = 8,

    /// <summary>
    /// Error No existe un destinatario Inicial
    /// </summary>
    ERROR_NO_TIENE_UN_DESTINATARIO_DEFINIDO = 9,

    /// <summary>
    /// Error No tiene datos en la tabla admision Giro
    /// </summary>
    ERROR_DATOS_EN_LA_INFORMACION_DEL_GIRO = 10,

    /// <summary>
    /// Error No tiene datos en la tabla admision Giro
    /// </summary>
    ERROR_DIGITO_CHEQUEO = 11,

    /// <summary>
    /// Error No tiene datos en la tabla admision Giro
    /// </summary>
    ERROR_SOLICITUD_SIN_DATOS = 12,

    /// <summary>
    /// El número de comprobante de pago no pertenece a la agencia destino.
    /// </summary>
    ERROR_COMPROBANTE_PAGO_AGENCIA = 13,

    /// <summary>
    /// sOLICITUD NO EXISTE
    /// </summary>
    ERROR_SOLICITUD_NO_EXISTE = 14,

    /// <summary>
    /// El parametro no esta configurado en la base de datos
    /// </summary>
    ERROR_PARAMETRO_NO_CONFIGURADO = 15,

    /// <summary>
    /// El parametro no esta configurado en la base de datos
    /// </summary>
    ERROR_TIPO_GIRO_NO_CONFIGURADO = 16,

    /// <summary>
    /// Mensaje de Error en la consulta del numero de la solicitud manual.
    /// </summary>
    ERROR_NUMERO_SOLICITUD_MANUAL_NO_EXISTE = 17,

    /// <summary>
    /// Mensaje de Error por la consulta del numero de compobante de Pago
    /// </summary>
    ERROR_NUMERO_COMPROBANTE_PAGO_NO_CREADO = 18,

    /// <summary>
    /// Mensaje de Error por consultar una solicitud ya Activa.
    /// </summary>
    ERROR_SOLICITUD_NUMERO_COMPROBANTE_YA_CREADA = 19,

    /// <summary>
    /// Mensaje de Error por fecha de anulación fuera de la definida.
    /// </summary>
    ERROR_SOLICITUD_FECHA_ANULACION_GIRO = 20,

    /// <summary>
    /// Mensaje de Error por fecha de ajuste fuera de la definida.
    /// </summary>
    ERROR_SOLICITUD_FECHA_AJUSTE_GIRO = 21,

    /// <summary>
    /// Error del giro con estado Anulado
    /// </summary>
    GIRO_CON_ESTADO_ANULADO = 22,

    /// <summary>
    /// Error del giro con estado Bloqueado
    /// </summary>
    GIRO_CON_ESTADO_BLOQUEADO = 23,

    /// <summary>
    //Mensaje de Error por no estar configurado el motivo de devolucion de la solicitud
    /// </summary>
    ERROR_MOTIVO_DEVOLUCION_NO_CONFIGURADO = 24,

    /// <summary>
    /// Error cuando la solcitud esta rechazada
    /// </summary>
    ERROR_SOLICITUD_ACTIVA_RECHAZADA = 25,

    /// <summary>
    /// Error cuando el giro no existe o el origen no pertenece al racol
    /// </summary>
    ERROR_GIRO_NO_EXISTE_O_ORIGEN_NO_PERTENECE_RACOL = 26,

    /// <summary>
    /// Error cuando la solicitud no pertenece al racol del usuario
    /// </summary>
    ERROR_SOLICITUD_NO_PERTENECE_A_RACOL = 27,

    /// <summary>
    /// Mensaje de error cuando la tarifa consultada
    /// esta fuera de los rangos consultados
    /// </summary>
    ERROR_TARIFA_FUERA_DE_LOS_RANGOS = 28,

    /// <summary>
    /// Mensaje de error cuando no hay rango
    /// de precios para ese servicio
    /// </summary>
    ERROR_NO_HAY_RANGOS_PARA_EL_SERVICIO = 29
  }
}