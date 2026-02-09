using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.ControlCuentas.Comun;

namespace CO.Servidor.ControlCuentas.Comun
{
    /// <summary>
    /// Clase para manejar los mensajes de Control de Cuentas
    /// </summary>
    public class MensajesControlCuentas
    {
        /// <summary>
        /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(EnumTipoErrorControlCuentas tipoErrorControlCuentas)
        {
            string mensajeError = string.Empty;

            switch (tipoErrorControlCuentas)
            {
                #region Mensajes de Excepción

                case EnumTipoErrorControlCuentas.EX_PERSONA_NO_EXISTE:
                    mensajeError = Mensajes.EX_001;
                    break;

                case EnumTipoErrorControlCuentas.EX_PUNTO_NO_ADMITE_FORMA_PAGO_ALCOBRO:
                    mensajeError = Mensajes.EX_002;
                    break;

                case EnumTipoErrorControlCuentas.EX_FORMA_PAGO_CREDITO_INVALIDA:
                    mensajeError = Mensajes.EX_003;
                    break;

                case EnumTipoErrorControlCuentas.EX_FORMA_PAGO_INVALIDA:
                    mensajeError = Mensajes.EX_004;
                    break;

                case EnumTipoErrorControlCuentas.EX_CONTRATO_INVALIDO:
                    mensajeError = Mensajes.EX_005;
                    break;

                case EnumTipoErrorControlCuentas.EX_ESTADO_GUIA_NO_VALIDO:
                    mensajeError = Mensajes.EX_006;
                    break;

                case EnumTipoErrorControlCuentas.EX_ERROR_FORMA_DE_PAGO_NO_ENCONTRADA:
                    mensajeError = Mensajes.EX_007;
                    break;

                #endregion Mensajes de Excepción

                #region Mensajes Informativos

                case EnumTipoErrorControlCuentas.IN_MODIFICACION_DESTINO:
                    mensajeError = Mensajes.IN_001;
                    break;

                case EnumTipoErrorControlCuentas.IN_DESCUENTO_POR_CAMBIO_FORMA_PAGO:
                    mensajeError = Mensajes.IN_003;
                    break;

                case EnumTipoErrorControlCuentas.IN_ANULACION_GUIA:
                    mensajeError = Mensajes.IN_004;
                    break;

                case EnumTipoErrorControlCuentas.IN_CAMBIO_SERVICIO:
                    mensajeError = Mensajes.IN_005;
                    break;

                case EnumTipoErrorControlCuentas.IN_GUIA_NO_EXISTE:
                    mensajeError = Mensajes.IN_006;
                    break;

                case EnumTipoErrorControlCuentas.IN_GUIA_NO_APROVISIONADA:
                    mensajeError = Mensajes.IN_007;
                    break;

                case EnumTipoErrorControlCuentas.IN_AJUSTE_CAMBIO_DESTINO:
                    mensajeError = Mensajes.IN_008;
                    break;

                #endregion Mensajes Informativos

                default:
                    mensajeError = String.Format(Mensajes.EX_000, tipoErrorControlCuentas.ToString());
                    break;
            }

            return mensajeError;
        }
    }
}