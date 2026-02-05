using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Clientes.Comun
{
    public class CLMensajesClientes
    {

        /// <summary>
        /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(CLEnumTipoErrorCliente tipoErrorCliente)
        {
            string mensajeError;

            switch (tipoErrorCliente)
            {
                #region Mensajes de Excepción

                case CLEnumTipoErrorCliente.EX_EXISTE_PERSONA_CONTRATO:
                    mensajeError = CLMensajes.EX_001;
                    break;

                case CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO:
                    mensajeError = CLMensajes.EX_002;
                    break;

                case CLEnumTipoErrorCliente.EX_FALLO_ELIMINAR_ARCHIVO:
                    mensajeError = CLMensajes.EX_003;
                    break;

                case CLEnumTipoErrorCliente.EX_FALLO_ELIMINAR_REQUISITO:
                    mensajeError = CLMensajes.EX_004;
                    break;

                case CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO:
                    mensajeError = CLMensajes.EX_005;
                    break;

                case CLEnumTipoErrorCliente.EX_FALLO_PRESU_MES:
                    mensajeError = CLMensajes.EX_006;
                    break;

                case CLEnumTipoErrorCliente.EX_FALLO_PRESU_CONTRATO:
                    mensajeError = CLMensajes.EX_007;
                    break;

                case CLEnumTipoErrorCliente.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA:
                    mensajeError = CLMensajes.EX_008;
                    break;
                case CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE:
                    mensajeError = CLMensajes.EX_009;
                    break;
                case CLEnumTipoErrorCliente.EX_CONTRATO_NO_EXISTE:
                    mensajeError = CLMensajes.EX_011;
                    break;
                case CLEnumTipoErrorCliente.EX_SUCURSAL_O_CONTRATO_NO_EXISTEN:
                    mensajeError = CLMensajes.EX_012;
                    break;
                case CLEnumTipoErrorCliente.EX_NIT_YA_EXISTE:
                    mensajeError = CLMensajes.EX_013;
                    break;
                case CLEnumTipoErrorCliente.EX_NIT_LISTA_NEGRA:
                    mensajeError = CLMensajes.EX_014;
                    break;
                case CLEnumTipoErrorCliente.EX_SUCURSAL_SIN_CONTRATOS:
                    mensajeError = CLMensajes.EX_015;
                    break;
                #endregion Mensajes de Excepción

                #region Mensajes Informativos

                case CLEnumTipoErrorCliente.IN_ARCHIVO_CEDULA_DESTINATARIO:
                    mensajeError = CLMensajes.IN_001;
                    break;

                case CLEnumTipoErrorCliente.IN_ARCHIVO_DESC_CEDULA_DESTINATARIO:
                    mensajeError = CLMensajes.IN_002;
                    break;

                #endregion Mensajes Informativos

                default:
                    mensajeError = String.Format(CLMensajes.EX_000, tipoErrorCliente.ToString());
                    break;
            }
            return mensajeError;
        }

    }
}
