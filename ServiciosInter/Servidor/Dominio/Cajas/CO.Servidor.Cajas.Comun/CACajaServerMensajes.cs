using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Cajas.Comun
{
  /// <summary>
  /// Mensajes de modulo cajas lado cliente
  /// </summary>
  public class CACajaServerMensajes
  {
    public static string CargarMensaje(CAEnumTipoErrorCaja tipoErrorCaja)
    {
      string mensajeError = string.Empty;

      switch (tipoErrorCaja)
      {
        case CAEnumTipoErrorCaja.ERROR_DE_APERTURA_CAJA:
          mensajeError = CACajaServidorMensajes.EX_001;
          break;

        case CAEnumTipoErrorCaja.ERROR_DE_MOVIMIENTO_CAJA_CERRADA:
          mensajeError = CACajaServidorMensajes.EX_002;
          break;

        case CAEnumTipoErrorCaja.ERROR_DE_APERTURA_CAJA_EQUIPO:
          mensajeError = CACajaServidorMensajes.EX_003;
          break;

        case CAEnumTipoErrorCaja.ERROR_CAJA_CERRADA_NO_EFECTUAR_TRANSACCION:
          mensajeError = CACajaServidorMensajes.EX_004;
          break;

        case CAEnumTipoErrorCaja.ERROR_CLIENTE_CREDITO_SIN_DATOS:
          mensajeError = CACajaServidorMensajes.EX_005;
          break;

        case CAEnumTipoErrorCaja.ERROR_NO_EXISTE_CIERRE:
          mensajeError = CACajaServidorMensajes.EX_006;
          break;

        case CAEnumTipoErrorCaja.ERROR_VALOR_PINPREPAGO_SUPERA_SALDO:
          mensajeError = CACajaServidorMensajes.EX_007;
          break;

        case CAEnumTipoErrorCaja.ERROR_PINPREPAGO_NO_ENCONTRADO:
          mensajeError = CACajaServidorMensajes.EX_008;
          break;

        case CAEnumTipoErrorCaja.ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO:
          mensajeError = CACajaServidorMensajes.EX_009;
          break;

        case CAEnumTipoErrorCaja.ERROR_VALOR_CIERRE_PUNTO_CERO:
          mensajeError = CACajaServidorMensajes.EX_010;
          break;

        case CAEnumTipoErrorCaja.ERROR_NO_EXISTE_CIERRE_DEL_PUNTO:
          mensajeError = CACajaServidorMensajes.EX_011;
          break;

        case CAEnumTipoErrorCaja.ERROR_BOLSA_SEGURIDAD_YA_REGISTRADA:
          mensajeError = CACajaServidorMensajes.EX_012;
          break;

        case CAEnumTipoErrorCaja.ERROR_DINERO_NO_REPORTADO_AGENCIA:
          mensajeError = CACajaServidorMensajes.EX_013;
          break;

        case CAEnumTipoErrorCaja.ERROR_TRANSACCION_CAJA:
          mensajeError = CACajaServidorMensajes.EX_014;
          break;

        case CAEnumTipoErrorCaja.ERROR_TRANSACCION_DETALLE_CAJA:
          mensajeError = CACajaServidorMensajes.EX_015;
          break;

        case CAEnumTipoErrorCaja.ERROR_INSERCION_REGISTRO_DETALLE_CAJA:
          mensajeError = CACajaServidorMensajes.EX_016;
          break;

        case CAEnumTipoErrorCaja.ERROR_TRANSACCION_CLIENTE_CREDITO:
          mensajeError = CACajaServidorMensajes.EX_017;
          break;

        case CAEnumTipoErrorCaja.ERROR_CONCEPTO_ANULACIONGUIA_NOEXISTE:
          mensajeError = CACajaServidorMensajes.EX_018;
          break;

        case CAEnumTipoErrorCaja.ERROR_DINERO_YA_REPORTADO_AGENCIA:
          mensajeError = CACajaServidorMensajes.EX_019;
          break;

        case CAEnumTipoErrorCaja.ERROR_OPERACION_SOLO_DESDE_RACOL:
          mensajeError = CACajaServidorMensajes.EX_020;
          break;

        case CAEnumTipoErrorCaja.ERROR_NO_CUENTA_BANCO:
          mensajeError = CACajaServidorMensajes.EX_021;
          break;

        case CAEnumTipoErrorCaja.ERROR_PINPREPAGO_ERRADO:
          mensajeError = CACajaServidorMensajes.EX_022;
          break;

        case CAEnumTipoErrorCaja.ERROR_NO_AUTORIZADO:
          mensajeError = CACajaServidorMensajes.EX_023;
          break;

        case CAEnumTipoErrorCaja.ERROR_DUPLA_NO_EXISTE:
          mensajeError = CACajaServidorMensajes.EX_024;
          break;

        case CAEnumTipoErrorCaja.ERROR_MOVIMIENTO_NO_EXISTE:
          mensajeError = CACajaServidorMensajes.EX_025;
          break;

        case CAEnumTipoErrorCaja.ERROR_NO_CAJA_PARA_APERTURA:
          mensajeError = CACajaServidorMensajes.EX_026;
          break;

        case CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA:
          mensajeError = CACajaServidorMensajes.EX_027;
          break;

        case CAEnumTipoErrorCaja.ERROR_PARAMETRO_CAJA_NO_ENCONTRADO:
          mensajeError = CACajaServidorMensajes.EX_028;
          break;

        case CAEnumTipoErrorCaja.ERROR_BOLSA_SEGIRIDAD_ERRADA:
          mensajeError = CACajaServidorMensajes.EX_029;
          break;

        case CAEnumTipoErrorCaja.ERROR_CAJA_SIN_APERTURA_PARA_CERRAR:
          mensajeError = CACajaServidorMensajes.EX_030;
          break;

        case CAEnumTipoErrorCaja.ERROR_CAJA_NUMERO_NO_ENCONTRADO:
          mensajeError = CACajaServidorMensajes.EX_031;
          break;

        case CAEnumTipoErrorCaja.ERROR_SUMINISTRO_NO_ASIGNADO_A_USTED:
          mensajeError = CACajaServidorMensajes.EX_032;
          break;

        case CAEnumTipoErrorCaja.ERROR_SUMINISTRO_NO_ES_CORRECTO:
          mensajeError = CACajaServidorMensajes.EX_033;
          break;

        case CAEnumTipoErrorCaja.ERROR_REPORTE_DINERO_DE_PUNTO_PENDIENTE_POR_DESCARGAR:
          mensajeError = CACajaServidorMensajes.EX_034;
          break;

          
        default:
          mensajeError = String.Format(CACajaServidorMensajes.EX_000, tipoErrorCaja.ToString());
          break;
      }
      return mensajeError;
    }
  }
}