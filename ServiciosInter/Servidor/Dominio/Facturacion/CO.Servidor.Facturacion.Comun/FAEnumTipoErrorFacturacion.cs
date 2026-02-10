using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Facturacion.Comun
{
    public enum FAEnumTipoErrorFacturacion : int
    {
        #region Mensajes de Excepción

        /// <summary>
        /// El movimiento que intenta excluir no pertenece a un cliente crédito
        /// </summary>
        EX_MOVIMIENTO_NOASIGNADO,

        /// <summary>
        /// El movimiento que intenta excluir ya se encuentra facturado
        /// </summary>
        EX_MOVIMIENTO_YAFACTURADO,

        /// <summary>
        /// La fecha del movimiento que intenta excluir es superior a la fecha de corte de la programación.
        /// </summary>
        EX_MOVIMIENTO_SUPERAFECHA,

        /// <summary>
        /// La factura que intenta anular ya se encuentra anulada
        /// </summary>
        EX_FACTURA_ANULADA,

        /// <summary>
        /// El valor de la nota credito no es valido
        /// </summary>
        EX_ERROR_VALOR_NOTA,

        /// <summary>
        /// La nota no se puede eliminar quedaria un valor negativo en la factura
        /// </summary>
        EX_VALOR_NEGATIVO_EN_FACTURA_POR_NOTA,

        #endregion Mensajes de Excepción
    }
}