using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Servidor.OperacionNacional.Comun
{
    /// <summary>
    /// Clase para manejar los mensajes de operacion nacional
    /// </summary>
    public class MensajesOperacionNacional
    {
        /// <summary>
        /// Carga un mensaje de error de operacion nacional desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(EnumTipoErrorOperacionNacional tipoErrorOperacionNacional)
        {
            string mensajeError;

            switch (tipoErrorOperacionNacional)
            {
                #region Mensajes de Excepción

                case EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_CONSOLIDADO:
                    mensajeError = Mensajes.EX_001;
                    break;

                case EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_CONSOLIDADO_DETALLE:
                    mensajeError = Mensajes.EX_002;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA:
                    mensajeError = Mensajes.EX_003;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_CREACION_GUIA_SUELTA:
                    mensajeError = Mensajes.EX_004;
                    break;

                case EnumTipoErrorOperacionNacional.EX_GUIA_NO_INGRESADA_CENTRO_ACOPIO:
                    mensajeError = Mensajes.EX_005;
                    break;

                case EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_MOTIVO_ELIMINACION_GUIA_MANIFIESTO:
                    mensajeError = Mensajes.EX_006;
                    break;

                case EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_MANIFIESTO:
                    mensajeError = Mensajes.EX_007;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA:
                    mensajeError = Mensajes.EX_008;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_RUTA:
                    mensajeError = Mensajes.EX_009;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_CIUDAD_MANIFIESTA:
                    mensajeError = Mensajes.EX_010;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_FALTA_INGRESO_VEHICULO:
                    mensajeError = Mensajes.EX_011;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_CIUDAD_INGRESO_NO_RUTA:
                    mensajeError = Mensajes.EX_012;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_ESTADOS_EMPAQUE:
                    mensajeError = Mensajes.EX_013;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_INGRESO_OPERATIVO:
                    mensajeError = Mensajes.EX_014;
                    break;

                case EnumTipoErrorOperacionNacional.EX_FALLA_NOVEDAD_PRECINTO_CONSOLIDADO:
                    mensajeError = Mensajes.EX_015;
                    break;

                case EnumTipoErrorOperacionNacional.EX_FALLA_SIN_BOLSA_SEGURIDAD:
                    mensajeError = Mensajes.EX_016;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_EXISTE_CONDUCTOR_RUTA_ASOCIADO_A_PLACA:
                    mensajeError = Mensajes.EX_017;
                    break;

                case EnumTipoErrorOperacionNacional.EX_VEHICULO_YA_REGISTRADO:
                    mensajeError = Mensajes.EX_018;
                    break;

                case EnumTipoErrorOperacionNacional.EX_FALLA_DIFERENCIA_PESO:
                    mensajeError = Mensajes.EX_019;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ENVIO_NO_ES_TRANSITO:
                    mensajeError = Mensajes.EX_020;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ENVIO_TRANSITO_NO_MANIFESTADO:
                    mensajeError = Mensajes.EX_021;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO:
                    mensajeError = Mensajes.EX_022;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_ESTADO_GUIA:
                    mensajeError = Mensajes.EX_023;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_EXISTE_CONSOLIDADO_NUM_GUIA:
                    mensajeError = Mensajes.EX_024;
                    break;

                case EnumTipoErrorOperacionNacional.EX_CIUDAD_DESTINO_ENVIO_TIENE_RUTA:
                    mensajeError = Mensajes.EX_025;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_DETALLE_MANIFIESTO:
                    mensajeError = Mensajes.EX_026;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO:
                    mensajeError = Mensajes.EX_027;
                    break;

                case EnumTipoErrorOperacionNacional.EX_GUIA_NO_ES_INTERNA:
                    mensajeError = Mensajes.EX_028;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_ABRIR_MANIFIESTO:
                    mensajeError = Mensajes.EX_029;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_OBTENER_DATOS_CONDUCTOR:
                    mensajeError = Mensajes.EX_030;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_OBTENER_DATOS_VEHICULO:
                    mensajeError = Mensajes.EX_031;
                    break;

                case EnumTipoErrorOperacionNacional.EX_MANIFIESTO_SIN_VEHICULO_CONFIGURADO:
                    mensajeError = Mensajes.EX_032;
                    break;

                case EnumTipoErrorOperacionNacional.EX_GUIA_INTERNA_NO_APROVISIONADA:
                    mensajeError = Mensajes.EX_033;
                    break;

                case EnumTipoErrorOperacionNacional.EX_ERROR_OPERATIVO_NO_INICIADO:
                    mensajeError = Mensajes.EX_034;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_INFORMACION_AGENCIA_O_LOCALIDAD:
                    mensajeError = Mensajes.EX_035;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_MANIFESTAR_DESTINO_GUIA_ANTES_DE_LOCALIDAD_A_MANIFESTAR:
                    mensajeError = Mensajes.EX_036;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR:
                    mensajeError = Mensajes.EX_037;
                    break;

                case EnumTipoErrorOperacionNacional.EX_PARAMETRO_NO_EXISTE:
                    mensajeError = Mensajes.EX_038;
                    break;

                case EnumTipoErrorOperacionNacional.EX_INGRESO_A_AGENCIA_CERRADO:
                    mensajeError = Mensajes.EX_039;
                    break;

                case EnumTipoErrorOperacionNacional.EX_CONSOLIDADO_NO_ENCONTRADO:
                    mensajeError = Mensajes.EX_040;
                    break;

                case EnumTipoErrorOperacionNacional.EX_GUIA_NO_EXISTE:
                    mensajeError = Mensajes.EX_041;
                    break;

                case EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO:
                    mensajeError = Mensajes.EX_042;
                    break;

                case EnumTipoErrorOperacionNacional.EX_INGRESO_INVALIDO:
                    mensajeError = Mensajes.EX_043;
                    break;

                case EnumTipoErrorOperacionNacional.EX_CONTENEDORTULA_YA_MANIFESTADO:
                    mensajeError = Mensajes.EX_044;
                    break;

                case EnumTipoErrorOperacionNacional.EX_GUIA_YA_INGRESADA_CENTRO_ACOPIO:
                    mensajeError = Mensajes.EX_045;
                    break;
                case EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA:
                    mensajeError = Mensajes.EX_046;
                    break;

                case  EnumTipoErrorOperacionNacional.EX_TULA_YA_ESTA_UTILIZADA_OTRO_PROCESO:
                    mensajeError = Mensajes.EX_047;
                    break;
                case EnumTipoErrorOperacionNacional.EX_VEHICULO_ASIGNADO_A_MANIFIESTO_ABIERTO:
                    mensajeError = Mensajes.EX_048;
                    break;

                case EnumTipoErrorOperacionNacional.EX_NO_EXISTE_TULA_CONSOLIDADO:
                    mensajeError = Mensajes.EX_049;
                    break;

                    

                #endregion Mensajes de Excepción

                #region Mensajes de Informacion

                case EnumTipoErrorOperacionNacional.IN_EL_INGRESO:
                    mensajeError = Mensajes.IN_001;
                    break;

                case EnumTipoErrorOperacionNacional.IN_LA_SALIDA:
                    mensajeError = Mensajes.IN_002;
                    break;

                #endregion Mensajes de Informacion

                default:
                    mensajeError = String.Format(Mensajes.EX_000, tipoErrorOperacionNacional.ToString());
                    break;
            }

            return mensajeError;
        }
    }
}