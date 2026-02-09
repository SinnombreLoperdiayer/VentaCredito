using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Admisiones.Giros.Comun;

namespace CO.Servidor.Admisiones.Giros.Comun
{
    /// <summary>
    /// Clase para manejar los mensajes de Admisiones
    /// </summary>
    public class MensajesAdmisionesGiros
    {
        /// <summary>
        /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(EnumTipoErrorAdmisionesGiros tipoErrorAdmisionGiros)
        {
            string mensajeError = string.Empty;

            switch (tipoErrorAdmisionGiros)
            {
                #region Mensajes de Excepción

                case EnumTipoErrorAdmisionesGiros.EX_NUMERO_GIRO_NO_ASOCIADO_A_AGENCIA:
                    mensajeError = Mensajes.EX_001;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_NUMERO_GIRO_EXISTE:
                    mensajeError = Mensajes.EX_002;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_SUPERO_VALOR_MAXIMO_GIRO:
                    mensajeError = Mensajes.EX_003;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_CREAR_NUM_GIRO_AUTOMATICO:
                    mensajeError = Mensajes.EX_004;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_DECLARACION_VALUNTARIA_FONDOS:
                    mensajeError = Mensajes.EX_005;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_SOLICITUDES_ACTIVAS:
                    mensajeError = Mensajes.EX_006;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO:
                    mensajeError = Mensajes.EX_007;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE:
                    mensajeError = Mensajes.EX_008;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_SE_PUDO_PAGAR:
                    mensajeError = Mensajes.EX_009;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_PAGO_NO_EXISTE:
                    mensajeError = Mensajes.EX_010;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS:
                    mensajeError = Mensajes.EX_011;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_GIRO_TRANSMITIDO:
                    mensajeError = Mensajes.EX_012;
                    break;

                case EnumTipoErrorAdmisionesGiros.EX_CENTRO_SERVICIO_NO_PUEDE_ADMITIR_GUIA_SERVIDOR:
                    mensajeError = Mensajes.EX_013;
                    break;

                #endregion Mensajes de Excepción

                #region Mensajes Informativos

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_DECLARACION_VOLUNTARIA_FONDOS:
                    mensajeError = Mensajes.IN_001;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_DESC_DECLARACION_VOLUNTARIA_FONDOS:
                    mensajeError = Mensajes.IN_002;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_CEDULA_DESTINATARIO:
                    mensajeError = Mensajes.IN_003;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_DESC_CEDULA_DESTINATARIO:
                    mensajeError = Mensajes.IN_004;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_AUTORIZACION_PAGO:
                    mensajeError = Mensajes.IN_005;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_DESC_AUTORIZACION_PAGO:
                    mensajeError = Mensajes.IN_006;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_CERTIFICADO_EMPRESARIAL:
                    mensajeError = Mensajes.IN_007;
                    break;

                case EnumTipoErrorAdmisionesGiros.IN_ARCHIVO_DESC_CERTIFICADO_EMPRESARIAL:
                    mensajeError = Mensajes.IN_008;
                    break;

                #endregion Mensajes Informativos

                default:
                    mensajeError = String.Format(Mensajes.EX_000, tipoErrorAdmisionGiros.ToString());
                    break;
            }

            return mensajeError;
        }
    }
}