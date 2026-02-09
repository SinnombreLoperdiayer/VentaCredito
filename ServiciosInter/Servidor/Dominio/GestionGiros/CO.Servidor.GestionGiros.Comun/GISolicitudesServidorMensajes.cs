using System;

namespace CO.Servidor.GestionGiros.Comun
{
  /// <summary>
  /// Clase para Manejar los mensajes de Solicitudes
  /// </summary>
  public class GISolicitudesServidorMensajes
  {
    /// <summary>
    /// Carga un mensaje de error de Solicitudes desde el recurso del lenguaje
    /// </summary>
    /// <param name="tipoErrorSolicitudes"></param>
    /// <returns></returns>
    public static string CargarMensaje(GIEnumTipoErrorSolicitud tipoErrorSolicitudes)
    {
      string mensajeError;

      switch (tipoErrorSolicitudes)
      {
        case GIEnumTipoErrorSolicitud.GIRO_CON_SOLICITUDES_EN_ESTADO_ACTIVO:
          mensajeError = GISolServidorMensajes.EX_001;
          break;

        case GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_PAGO:
          mensajeError = GISolServidorMensajes.EX_002;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_ADICIONAR_NVA_SOLICITUD:
          mensajeError = GISolServidorMensajes.EX_003;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_AL_CARGAR_LOS_ARCHIVOS:
          mensajeError = GISolServidorMensajes.EX_004;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_AL_NO_ENCONTRAR_GIRO:
          mensajeError = GISolServidorMensajes.EX_005;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_AL_TRAER_DETALLE_SOLICITUD:
          mensajeError = GISolServidorMensajes.EX_006;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_AGENCIA_NO_ACTA_PARA_PAGO:
          mensajeError = GISolServidorMensajes.EX_007;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_GIRO_YA_PAGO:
          mensajeError = GISolServidorMensajes.EX_008;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_NO_TIENE_UN_DESTINATARIO_DEFINIDO:
          mensajeError = GISolServidorMensajes.EX_009;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_DATOS_EN_LA_INFORMACION_DEL_GIRO:
          mensajeError = GISolServidorMensajes.EX_010;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_DIGITO_CHEQUEO:
          mensajeError = GISolServidorMensajes.EX_011;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_SIN_DATOS:
          mensajeError = GISolServidorMensajes.EX_012;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_COMPROBANTE_PAGO_AGENCIA:
          mensajeError = GISolServidorMensajes.EX_013;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NO_EXISTE:
          mensajeError = GISolServidorMensajes.EX_014;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO:
          mensajeError = GISolServidorMensajes.EX_015;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_TIPO_GIRO_NO_CONFIGURADO:
          mensajeError = GISolServidorMensajes.EX_016;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_NUMERO_SOLICITUD_MANUAL_NO_EXISTE:
          mensajeError = GISolServidorMensajes.EX_017;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_NUMERO_COMPROBANTE_PAGO_NO_CREADO:
          mensajeError = GISolServidorMensajes.EX_018;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NUMERO_COMPROBANTE_YA_CREADA:
          mensajeError = GISolServidorMensajes.EX_019;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_FECHA_ANULACION_GIRO:
          mensajeError = GISolServidorMensajes.EX_020;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_FECHA_AJUSTE_GIRO:
          mensajeError = GISolServidorMensajes.EX_021;
          break;

        case GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_ANULADO:
          mensajeError = GISolServidorMensajes.EX_022;
          break;

        case GIEnumTipoErrorSolicitud.GIRO_CON_ESTADO_BLOQUEADO:
          mensajeError = GISolServidorMensajes.EX_023;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_MOTIVO_DEVOLUCION_NO_CONFIGURADO:
          mensajeError = GISolServidorMensajes.EX_024;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_ACTIVA_RECHAZADA:
          mensajeError = GISolServidorMensajes.EX_025;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_GIRO_NO_EXISTE_O_ORIGEN_NO_PERTENECE_RACOL:
          mensajeError = GISolServidorMensajes.EX_026;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NO_PERTENECE_A_RACOL:
          mensajeError = GISolServidorMensajes.EX_027;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_TARIFA_FUERA_DE_LOS_RANGOS:
          mensajeError = GISolServidorMensajes.EX_028;
          break;

        case GIEnumTipoErrorSolicitud.ERROR_NO_HAY_RANGOS_PARA_EL_SERVICIO:
          mensajeError = GISolServidorMensajes.EX_029;
          break;

        default:
          mensajeError = String.Format(GISolServidorMensajes.EX_000, tipoErrorSolicitudes.ToString());
          break;
      }

      return mensajeError;
    }
  }
}