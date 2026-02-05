using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Suministros.Comun
{
    /// <summary>
    /// Enumeración con los códigos de mensajes de Admision de giros
    /// </summary>
    public enum EnumTipoErrorSuministros : int
    {
        /// <summary>
        /// Ninguna agencia tiene asignado el número de giro.
        /// </summary>
        EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_GIRO = 0,

        /// <summary>
        /// Ninguna agencia tiene asignado el número de giro.
        /// </summary>
        EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_COMPROBANTE_PAGO = 1,

        EX_CONSECUTIVO_NO_DISPONIBLE = 2,

        /// <summary>
        /// Cuando existen errores al crear el numero de giro o pago en suministros
        /// </summary>
        EX_NO_SE_PUEDE_CREAR_NUM_AUTOMATICO = 3,

        /// <summary>
        /// Se lanza cuando la guía no ha sido asignada a nadie y tampoco ha sido usada previamente
        /// </summary>
        EX_GUIA_SIN_ASIGNAR = 4,

        /// <summary>
        /// Se lanza cuando la guía ha sido usada previamente
        /// </summary>
        EX_GUIA_USADA = 5,

        /// <summary>
        /// La agencia asociada al número del giro no existe o esta inactiva
        /// </summary>
        EX_AGENCIA_NO_EXISTE_INACTIVA = 6,

        /// <summary>
        /// No existe ningún número de suministro valido.
        /// </summary>
        EX_NUM_SUMINISTRO_NO_EXISTE = 7,

        /// <summary>
        /// El prefijo del suministro no es válido.
        /// </summary>
        EX_PREFIJO_INVALIDO = 8,

        /// <summary>
        /// El número del suministro no es válido.
        /// </summary>
        EX_NUM_SUMINISTRO_INVALIDO = 9,

        /// <summary>
        /// El rango ingresado ya esta configurado
        /// </summary>
        EX_RANGO_NO_VALIDO = 10,

        /// <summary>
        /// El suministro seleccionado no tiene una numeracion vigente.
        /// </summary>
        EX_NO_EXISTE_NUMERACION_VIGENTE_SUMINISTRO = 11,

        /// <summary>
        /// Error cuando el rango que se ingresa para el suministro no esta configurado
        /// </summary>
        EX_RANGO_INGRESADO_NO_ESTA_CONFIGURADO = 12,

        /// <summary>
        /// Error cuando se detecta que el suministro que se quiere usar está anulado.
        /// </summary>
        EX_SUMINISTRO_ANULADO = 13,

        /// <summary>
        /// Error cuando algun valor del rango ingresado ya fue asignado
        /// </summary>
        EX_SUMINISTRO_ASIGNADO = 14,

        /// <summary>
        /// Error cuando ya existe un suministro configurado con el codigo de novasoft
        /// </summary>
        EX_ERROR_SUMINISTRO_EXISTE_CODIGO_NOVASOFT = 15,

        /// <summary>
        /// Error cuando ya existe un suministro configurado con el codigo alterno
        /// </summary>
        EX_ERROR_SUMINISTRO_EXISTE_CODIGO_ALTERNO = 16,

        /// <summary>
        /// Los Suministros no fueron aprovisionados para el Mensajero
        /// </summary>
        EX_ERROR_SUMINISTROS_NO_APROVISIONADO_MENSAJERO,

        /// <summary>
        /// Los Suministros no fueron aprovisionados para el Centro de Servicio
        /// </summary>
        EX_ERROR_SUMINISTROS_NO_APROVISIONADO_CENTRO_SERVICIO,

        /// <summary>
        /// Los Suministros no fueron aprovisionados para la Sucursal
        /// </summary>
        EX_ERROR_SUMINISTROS_NO_APROVISIONADO_SUCURSAL,

        /// <summary>
        /// Los Rangos entre los diferentes suministros no son consecutivos
        /// </summary>
        EX_ERROR_RANGOS_NO_CONSECUTIVO,

        /// <summary>
        /// Error cuando falta configurar un grupo de suministros
        /// </summary>
        EX_GRUPO_NO_CONFIGURADO,

        /// <summary>
        /// Error cuando el suministro no esta aprobado
        /// </summary>
        EX_SUMINISTRO_NO_APROBADO,

        /// <summary>
        /// Error cuando no existe suficiente unidades en bodega
        /// </summary>
        EX_EXISTENCIA_INSUFICIENTE_SUMINISTRO,

        /// <summary>
        /// Error en la fecha de la resolucion
        /// </summary>
        EX_ERROR_FECHA_RESOLUCION = 25,

        /// <summary>
        /// Error cuando el rango que se ingresa no esta configurado
        /// </summary>
        EX_ERROR_RANGO_NO_CONFIGURADO_RESOLUCIONES = 26,

        /// <summary>
        /// Error cuando no existe una numeración configurada
        /// </summary>
        EX_ERROR_NUMERACION_NO_CONFIGURADA = 27,

        /// <summary>
        /// Error cuando se desasigna un suministro
        /// </summary>
        EX_ERROR_DESASIGNAR_SUMINISTRO = 28,

        /// <summary>
        /// El número de bolsa de seguridad no es válido
        /// </summary>
        EX_ERROR_BOLSA_SEG_NO_VALIDA = 29,

        /// <summary>
        /// La fecha de la resolucion no es valida
        /// </summary>
        EX_ERROR_FECHA_RANGO_RESOLUCION = 30,

        /// <summary>
        /// Error cuando la fecha final de vigencia del suministro es inferior a la fecha actual
        /// </summary>
        EX_ERROR_FECHA_RANGO_VIGENCIA_RESOLUCION,

        EX_ERROR_CONTENEDOR_NO_EXISTE,

        EX_ERROR_CONTENEDOR_CENTROSERVICIOORIGEN,

        /// <summary>
        /// Error cuando el código del consolidado no existe o no es válido
        /// </summary>
        EX_ERROR_CODIGO_CONSOLIDADO_NO_EXISTE,

        EX_ERROR_CLIENTE_INACTIVO,

        /// <summary>
        /// El suministro guia manual offline no ha sido asignado al centro de servicio
        /// </summary>
        EX_ERROR_FALTA_ASIGNAR_SUMINISTRO_MANUAL_OFFLINE

    }
}
