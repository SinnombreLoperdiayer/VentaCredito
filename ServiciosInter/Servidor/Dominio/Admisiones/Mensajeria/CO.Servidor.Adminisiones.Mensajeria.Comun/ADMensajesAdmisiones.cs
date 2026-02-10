using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Adminisiones.Mensajeria.Comun
{
    public class ADMensajesAdmisiones
    {
        /// <summary>
        /// Carga un mensaje de error de admisiones mensajería desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(ADEnumTipoErrorMensajeria tipoErrorAdmisiones)
        {
            string mensajeError;
            switch (tipoErrorAdmisiones)
            {
                case ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA:
                    mensajeError = ADMensajes.EX_001;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_NO_EXISTE:
                    mensajeError = ADMensajes.EX_002;
                    break;

                case ADEnumTipoErrorMensajeria.EX_CAJA_CERRADA:
                    mensajeError = ADMensajes.EX_003;
                    break;

                case ADEnumTipoErrorMensajeria.EX_CONSECUTIVO_NO_DISPONIBLE:
                    mensajeError = ADMensajes.EX_004;
                    break;

                case ADEnumTipoErrorMensajeria.EX_RUTA_NO_DISPONIBLE:
                    mensajeError = ADMensajes.EX_005;
                    break;

                case ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE:
                    mensajeError = ADMensajes.EX_006;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_ENTREGADA:
                    mensajeError = ADMensajes.EX_007;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_CUSTODIA:
                    mensajeError = ADMensajes.EX_008;
                    break;

                case ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA:
                    mensajeError = ADMensajes.EX_009;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_AL_COBRO_YA_PAGADA:
                    mensajeError = ADMensajes.EX_010;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_NO_ES_AL_COBRO:
                    mensajeError = ADMensajes.EX_011;
                    break;

                case ADEnumTipoErrorMensajeria.EX_NO_HAY_GUIAS_AL_COBRO_ENTRE_FECHAS:
                    mensajeError = ADMensajes.EX_012;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_ARCHIVO:
                    mensajeError = ADMensajes.EX_013;
                    break;

                case ADEnumTipoErrorMensajeria.EX_INFO_SUCURSAL_NO_DISPONIBLE:
                    mensajeError = ADMensajes.EX_014;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_GUIA_MENSAJERO:
                    mensajeError = ADMensajes.EX_002 + ADMensajes.EX_015;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_BOLSA_NOVALIDA:
                    mensajeError = ADMensajes.EX_016;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_GUIA_NO_EXISTE_NO_ES_NOTIFICACION:
                    mensajeError = ADMensajes.EX_017;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_ENVIO_NO_ESTA_ENTREGADO_O_DEVOLUCION:
                    mensajeError = ADMensajes.EX_018;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA:
                    mensajeError = ADMensajes.EX_019;
                    break;

                case ADEnumTipoErrorMensajeria.IN_ANULADO:
                    mensajeError = ADMensajes.IN_002;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_NO_NOTIFICACION:
                    mensajeError = ADMensajes.EX_020;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_NO_DEVUELTA:
                    mensajeError = ADMensajes.EX_021;
                    break;

                case ADEnumTipoErrorMensajeria.EX_GUIA_YA_PLANILLADA:
                    mensajeError = ADMensajes.EX_022;
                    break;

                case ADEnumTipoErrorMensajeria.EX_NO_SE_PUDO_GENERAR_NUMERO_GUIA:
                    mensajeError = ADMensajes.EX_023;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_PROPIETARIO_GUIA:
                    mensajeError = ADMensajes.EX_024;
                    break;

                case ADEnumTipoErrorMensajeria.EX_ERROR_TRANSACCION_INVENT_DEVOLUCION_CLIENTE_CREDITO:
                    mensajeError = ADMensajes.EX_025;
                    break;

                default:
                    mensajeError = ADMensajes.EX_000;
                    break;
            }
            return mensajeError;
        }

        public static string MensajeFallaDiferenciaValorCobrado = ADMensajes.IN_001;
    }
}