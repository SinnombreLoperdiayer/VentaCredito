using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Suministros.Comun;

namespace CO.Servidor.Suministros.Comun
{
  /// <summary>
  /// Clase para manejar los mensajes de suministros
  /// </summary>
  public class MensajesSuministros
  {
    /// <summary>
    /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
    /// </summary>
    /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
    /// <returns>Mensaje de error</returns>
    public static string CargarMensaje(EnumTipoErrorSuministros tipoErrorSuministros)
    {
      string mensajeError = string.Empty;

      switch (tipoErrorSuministros)
      {
        #region Mensajes de Excepción

        case EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_GIRO:
          mensajeError = Mensajes.EX_001;
          break;

        case EnumTipoErrorSuministros.EX_CONSECUTIVO_NO_DISPONIBLE:
          mensajeError = Mensajes.EX_002;
          break;

        case EnumTipoErrorSuministros.EX_NO_SE_PUEDE_CREAR_NUM_AUTOMATICO:
          mensajeError = Mensajes.EX_003;
          break;

        case EnumTipoErrorSuministros.EX_GUIA_SIN_ASIGNAR:
          mensajeError = Mensajes.EX_004;
          break;

        case EnumTipoErrorSuministros.EX_GUIA_USADA:
          mensajeError = Mensajes.EX_005;
          break;

        case EnumTipoErrorSuministros.EX_AGENCIA_NO_EXISTE_INACTIVA:
          mensajeError = Mensajes.EX_006;
          break;

        case EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_COMPROBANTE_PAGO:
          mensajeError = Mensajes.EX_007;
          break;

        case EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE:
          mensajeError = Mensajes.EX_008;
          break;

        case EnumTipoErrorSuministros.EX_PREFIJO_INVALIDO:
          mensajeError = Mensajes.EX_009;
          break;

        case EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO:
          mensajeError = Mensajes.EX_010;
          break;

        case EnumTipoErrorSuministros.EX_RANGO_NO_VALIDO:
          mensajeError = Mensajes.EX_011;
          break;

        case EnumTipoErrorSuministros.EX_NO_EXISTE_NUMERACION_VIGENTE_SUMINISTRO:
          mensajeError = Mensajes.EX_012;
          break;

        case EnumTipoErrorSuministros.EX_RANGO_INGRESADO_NO_ESTA_CONFIGURADO:
          mensajeError = Mensajes.EX_013;
          break;

        case EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO:
          mensajeError = Mensajes.EX_014;
          break;

        case EnumTipoErrorSuministros.EX_SUMINISTRO_ASIGNADO:
          mensajeError = Mensajes.EX_015;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_NOVASOFT:
          mensajeError = Mensajes.EX_016;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_SUMINISTRO_EXISTE_CODIGO_ALTERNO:
          mensajeError = Mensajes.EX_017;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_MENSAJERO:
          mensajeError = Mensajes.EX_017;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_CENTRO_SERVICIO:
          mensajeError = Mensajes.EX_018;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_SUMINISTROS_NO_APROVISIONADO_SUCURSAL:
          mensajeError = Mensajes.EX_019;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_RANGOS_NO_CONSECUTIVO:
          mensajeError = Mensajes.EX_021;
          break;

        case EnumTipoErrorSuministros.EX_GRUPO_NO_CONFIGURADO:
          mensajeError = Mensajes.EX_022;
          break;

        case EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO:
          mensajeError = Mensajes.EX_023;
          break;

        case EnumTipoErrorSuministros.EX_EXISTENCIA_INSUFICIENTE_SUMINISTRO:
          mensajeError = Mensajes.EX_024;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_FECHA_RESOLUCION:
          mensajeError = Mensajes.EX_025;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_RANGO_NO_CONFIGURADO_RESOLUCIONES:
          mensajeError = Mensajes.EX_026;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_NUMERACION_NO_CONFIGURADA:
          mensajeError = Mensajes.EX_027;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO:
          mensajeError = Mensajes.EX_028;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_BOLSA_SEG_NO_VALIDA:
          mensajeError = Mensajes.EX_029;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_FECHA_RANGO_RESOLUCION:
          mensajeError = Mensajes.EX_030;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_FECHA_RANGO_VIGENCIA_RESOLUCION:
          mensajeError = Mensajes.EX_031;
          break;

          case EnumTipoErrorSuministros.EX_ERROR_CONTENEDOR_NO_EXISTE:
          mensajeError = Mensajes.EX_032;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_CONTENEDOR_CENTROSERVICIOORIGEN:
          mensajeError = Mensajes.EX_033;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_CODIGO_CONSOLIDADO_NO_EXISTE:
          mensajeError = Mensajes.EX_034;
          break;

        case EnumTipoErrorSuministros.EX_ERROR_CLIENTE_INACTIVO:
          mensajeError = Mensajes.EX_035;
          break;

          case EnumTipoErrorSuministros.EX_ERROR_FALTA_ASIGNAR_SUMINISTRO_MANUAL_OFFLINE:
          mensajeError = Mensajes.EX_036;
          break;

        #endregion Mensajes de Excepción

        default:
          mensajeError = String.Format(Mensajes.EX_000, tipoErrorSuministros.ToString());
          break;
      }

      return mensajeError;
    }
  }
}