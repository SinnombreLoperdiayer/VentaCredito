using System;
using System.Collections.Generic;
using System.Text;

namespace VentaCredito.Tarifas.Comun
{
    public class TAMensajesTarifas
    {
        /// <summary>
        /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(TAEnumTipoErrorTarifas tipoErrorTarifas)
        {
            string mensajeError;

            switch (tipoErrorTarifas)
            {
                case TAEnumTipoErrorTarifas.EX_EXISTE_TARIFA_PLENA_VIGENTE_ACTIVA:
                    mensajeError = TAMensajes.EX_001;
                    break;

                case TAEnumTipoErrorTarifas.EX_ERROR_RANGO_PRIMA_SEGURO:
                    mensajeError = TAMensajes.EX_002;
                    break;

                case TAEnumTipoErrorTarifas.EX_FECHA_ADICIONADO_LISTA_PRECIO_NO_CUMPLE:
                    mensajeError = TAMensajes.EX_003;
                    break;

                case TAEnumTipoErrorTarifas.EX_FECHA_MODIFICADO_LISTA_PRECIO_NO_CUMPLE:
                    mensajeError = TAMensajes.EX_004;
                    break;

                case TAEnumTipoErrorTarifas.EX_IDENTIFICADOR_NO_SE_CARGO:
                    mensajeError = TAMensajes.EX_005;
                    break;

                case TAEnumTipoErrorTarifas.EX_VALOR_FUERA_DE_RANGOS:
                    mensajeError = TAMensajes.EX_006;
                    break;

                case TAEnumTipoErrorTarifas.EX_VALOR_TOTAL_NO_VALIDO:
                    mensajeError = TAMensajes.EX_007;
                    break;

                case TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE:
                    mensajeError = TAMensajes.EX_008;
                    break;

                case TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_CREADO:
                    mensajeError = TAMensajes.EX_009;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_EX_TRAYECTO:
                    mensajeError = TAMensajes.EX_010;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO:
                    mensajeError = TAMensajes.EX_011;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO:
                    mensajeError = TAMensajes.EX_012;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_CONTRAPAGO:
                    mensajeError = TAMensajes.EX_013;
                    break;

                case TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO:
                    mensajeError = TAMensajes.EX_014;
                    break;

                case TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE_CLIENTE:
                    mensajeError = TAMensajes.EX_015;
                    break;

                case TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES:
                    mensajeError = TAMensajes.EX_016;
                    break;

                case TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES_RECIPROCO:
                    mensajeError = TAMensajes.EX_017;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_LISTA_PRECIOS_PLENA_VIGENTE:
                    mensajeError = TAMensajes.EX_018;
                    break;

                case TAEnumTipoErrorTarifas.EX_EXISTE_MAS_DE_UNA_LISTA_PRECIOS_PLENA_VIGENTE:
                    mensajeError = TAMensajes.EX_019;
                    break;

                case TAEnumTipoErrorTarifas.EX_EL_SEERVICIO_SOLICTADO_NO_ESTA_EN_LA_LISTA_DE_PRECIOS:
                    mensajeError = TAMensajes.EX_020;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_SE_ENCUENTRAN_RANGO_EN_LISTA_DE_PRECIOS:
                    mensajeError = TAMensajes.EX_021;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO:
                    mensajeError = TAMensajes.EX_022;
                    break;

                case TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO:
                    mensajeError = TAMensajes.EX_023;
                    break;

                case TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO:
                    mensajeError = TAMensajes.EX_024;
                    break;

                case TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MAYOR_QUE_MAXIMO_DECLARADO:
                    mensajeError = TAMensajes.EX_025;
                    break;

                case TAEnumTipoErrorTarifas.EX_NO_EXISTE_CONCEPTO_SERVICIO:
                    mensajeError = TAMensajes.EX_026;
                    break;
                case TAEnumTipoErrorTarifas.EX_RAPI_RADICADO_NO_ASIGNADO_SERVICIO:
                    mensajeError = TAMensajes.EX_027;
                    break;
                case TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO:
                    mensajeError = TAMensajes.EX_028;
                    break;

                case TAEnumTipoErrorTarifas.EX_VALOR_COBRAR_FUERA_RANGOS:
                    mensajeError = TAMensajes.EX_029;
                    break;

                case TAEnumTipoErrorTarifas.EX_MENSAJERIA_NO_CONF_LISTA_SERVICIO:
                    mensajeError = TAMensajes.EX_030;
                    break;

                case Comun.TAEnumTipoErrorTarifas.EX_LISTA_PRECIOS_KOMPRECH_INVALIDA:
                    mensajeError = TAMensajes.EX_031;
                    break;

                case TAEnumTipoErrorTarifas.EX_SERVICIO_NO_VALIDO:
                    mensajeError = TAMensajes.EX_032;
                    break;

                case TAEnumTipoErrorTarifas.EX_RUTA_NO_DISPONIBLE:
                    mensajeError = TAMensajes.EX_033;
                    break;

                case TAEnumTipoErrorTarifas.EX_ERROR_CREANDO_LISTA_PRECIOS:
                    mensajeError = TAMensajes.EX_034;
                    break;

                case TAEnumTipoErrorTarifas.EX_ERROR_ACTUALIZAR_TIPO_CUENTA_EXTERNA:
                    mensajeError = TAMensajes.EX_035;
                    break;

                case TAEnumTipoErrorTarifas.EX_TARIFA_CONTRAPAGO_NO_CONFIGURADA:
                    mensajeError = TAMensajes.EX_036;
                    break;


                default:
                    mensajeError = String.Format(TAMensajes.EX_000, tipoErrorTarifas.ToString());
                    break;
            }

            return mensajeError;
        }
    }
}
