using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Adminisiones.Mensajeria.Comun
{
    public enum ADEnumTipoErrorMensajeria
    {
        #region Mensajes de Excepción

        EX_PARAMETRIZACION_INVALIDA = 1,
        EX_GUIA_NO_EXISTE = 2,
        EX_CAJA_CERRADA = 3,
        EX_CONSECUTIVO_NO_DISPONIBLE = 4,
        EX_RUTA_NO_DISPONIBLE = 5,

        /// <summary>
        /// El número del Guía no existe
        /// </summary>
        EX_NUMERO_GUIA_NO_EXISTE = 6,

        /// <summary>
        /// La guía ya fue entregada
        /// </summary>
        EX_GUIA_ENTREGADA = 7,

        /// <summary>
        /// La guía está en estado custodia
        /// </summary>
        EX_GUIA_CUSTODIA = 8,

        /// <summary>
        /// No se encontró ningún cliente convenio con  ese número de guía
        /// </summary>
        EX_CLIENTE_CONVENIO_NO_ESTA = 9,

        /// <summary>
        /// Guia al Cobro ya pagada
        /// </summary>
        EX_GUIA_AL_COBRO_YA_PAGADA = 10,

        /// <summary>
        /// No es una Guia al Cobro
        /// </summary>
        EX_GUIA_NO_ES_AL_COBRO = 11,

        /// <summary>
        /// Guia al Cobro ya pagada
        /// </summary>
        EX_NO_HAY_GUIAS_AL_COBRO_ENTRE_FECHAS = 12,

        /// <summary>
        /// Error al guardar el archivo
        /// </summary>
        EX_ERROR_GRABAR_ARCHIVO,

        /// <summary>
        /// Informacio´n de la sucursal no se encuentra
        /// </summary>
        EX_INFO_SUCURSAL_NO_DISPONIBLE,

        /// <summary>
        /// Indica que no se pudo grabar la guía en el sistema "Mensajero"
        /// </summary>
        EX_ERROR_GRABAR_GUIA_MENSAJERO,

        /// <summary>
        /// Error que indica al usuario que el número de bolsa de seguridad ingresado no es válido.
        /// </summary>
        EX_ERROR_BOLSA_NOVALIDA,

        /// <summary>
        /// Indica que el concepto solicitado no tiene dupla asociada
        /// </summary>
        EX_ERROR_CONCEPTO_DUPLA_NO_EXISTE,

        EX_ERROR_GUIA_NO_EXISTE_NO_ES_NOTIFICACION,

        EX_ERROR_ENVIO_NO_ESTA_ENTREGADO_O_DEVOLUCION,

        IN_ANULADO,

        EX_GUIA_NO_DIGITALIZADA,

        EX_GUIA_NO_NOTIFICACION,

        EX_GUIA_NO_DEVUELTA,

        EX_GUIA_YA_PLANILLADA,

        EX_NO_SE_PUDO_GENERAR_NUMERO_GUIA,

        EX_ERROR_PROPIETARIO_GUIA,

        EX_ERROR_TRANSACCION_INVENT_DEVOLUCION_CLIENTE_CREDITO


        #endregion Mensajes de Excepción
    }
}