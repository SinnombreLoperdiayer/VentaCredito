using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Facturacion.Comun
{
    /// <summary>
    /// Clase para manejar los mensajes de facturación
    /// </summary>
    public class FAMensajesFacturacion
    {
        /// <summary>
        /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(FAEnumTipoErrorFacturacion tipoErrorFacturacion)
        {
            string mensajeError = string.Empty;

            switch (tipoErrorFacturacion)
            {
                #region Mensajes de Excepción

                case FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_NOASIGNADO:
                    mensajeError = FAMensajes.EX_001;
                    break;

                case FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_YAFACTURADO:
                    mensajeError = FAMensajes.EX_002;
                    break;

                case FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_SUPERAFECHA:
                    mensajeError = FAMensajes.EX_003;
                    break;

                case FAEnumTipoErrorFacturacion.EX_FACTURA_ANULADA:
                    mensajeError = FAMensajes.EX_004;
                    break;

                case FAEnumTipoErrorFacturacion.EX_ERROR_VALOR_NOTA:
                    mensajeError = FAMensajes.EX_005;
                    break;

                case FAEnumTipoErrorFacturacion.EX_VALOR_NEGATIVO_EN_FACTURA_POR_NOTA:
                    mensajeError = FAMensajes.EX_006;
                    break;

                #endregion Mensajes de Excepción

                #region Mensajes Informativos

                #endregion Mensajes Informativos

                default:
                    mensajeError = String.Format(FAMensajes.EX_000, tipoErrorFacturacion.ToString());
                    break;
            }

            return mensajeError;
        }
    }
}