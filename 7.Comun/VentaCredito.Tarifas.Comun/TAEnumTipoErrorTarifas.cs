using System;
using System.Collections.Generic;
using System.Text;

namespace VentaCredito.Tarifas.Comun
{
    /// <summary>
    /// Enumeración con los códigos de mensajes de tarifas
    /// </summary>
    public enum TAEnumTipoErrorTarifas : int
    {
        /// <summary>
        /// Ya existe una lista de precios con tarifa plena, estado activa y fecha vigente en el sistema.
        /// </summary>
        EX_EXISTE_TARIFA_PLENA_VIGENTE_ACTIVA = 1,

        /// <summary>
        /// El valor de la prima de seguro debe estar entre 0% - 100%.
        /// </summary>
        EX_ERROR_RANGO_PRIMA_SEGURO = 2,

        /// <summary>
        /// La fecha Inicial debe ser mayor o igual a la fecha actual y la fecha final debe ser mayor que la inicial.
        /// </summary>
        EX_FECHA_ADICIONADO_LISTA_PRECIO_NO_CUMPLE = 3,

        /// <summary>
        /// La fecha final debe ser mayor que la fecha inicial.
        /// </summary>
        EX_FECHA_MODIFICADO_LISTA_PRECIO_NO_CUMPLE = 4,

        /// <summary>
        /// El identificador interno del servicio no fue definido correctamente.
        /// </summary>
        EX_IDENTIFICADOR_NO_SE_CARGO = 5,

        /// <summary>
        /// El valor a cobrar no se encuentra en ningún rango de precios
        /// </summary>
        EX_VALOR_FUERA_DE_RANGOS = 6,

        /// <summary>
        /// Cuando el valor total es menos a la sumatoria de impuestos mas servicios
        /// </summary>
        EX_VALOR_TOTAL_NO_VALIDO = 7,

        /// <summary>
        /// El Trayecto no existe para ese servicio
        /// </summary>
        EX_TRAYECTO_NO_EXISTE = 8,

        /// <summary>
        /// El trayecto no se encuentra configurado
        /// </summary>
        EX_TRAYECTO_NO_CREADO = 9,

        /// <summary>
        /// No existe precio para la excepción de trayecto del servicio.
        /// </summary>
        EX_NO_EXISTE_PRECIO_PARA_EX_TRAYECTO = 10,

        /// <summary>
        /// No existe precio para el trayecto del servicio.
        /// </summary>
        EX_NO_EXISTE_PRECIO_PARA_TRAYECTO = 11,

        /// <summary>
        /// No existe precio.
        /// </summary>
        EX_NO_EXISTE_PRECIO = 12,

        /// <summary>
        /// El valor contra pago no está dentro de un rango.
        /// </summary>
        EX_NO_EXISTE_PRECIO_CONTRAPAGO = 13,

        /// <summary>
        /// El kilo inicial no ha sido configurado.
        /// </summary>
        EX_KILO_INICIAL_NO_CONFIGURADO = 14,

        /// <summary>
        /// El trayecto no existe para el cliente crédito.
        /// </summary>
        EX_TRAYECTO_NO_EXISTE_CLIENTE = 15,

        /// <summary>
        /// La localidad de origen y destino ya tienen un tipo trayecto y tipo subtrayecto asignado.
        /// </summary>
        EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES = 16,

        /// <summary>
        /// La localidad de origen y destino del recíproco ya tienen un tipo trayecto y tipo subtrayecto asignado.
        /// </summary>
        EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES_RECIPROCO = 17,

        //No existe una lista de precios vigente valida
        EX_NO_EXISTE_LISTA_PRECIOS_PLENA_VIGENTE = 18,

        //Hay mas de una lista de precios plena vigente, no se puede calcular precios y parámetros de la lista de precios
        EX_EXISTE_MAS_DE_UNA_LISTA_PRECIOS_PLENA_VIGENTE = 19,

        //El servicio no se encuentra en la lista de precios.
        EX_EL_SEERVICIO_SOLICTADO_NO_ESTA_EN_LA_LISTA_DE_PRECIOS = 20,

        //No se encuentran configurados los rangos de precios para el servicio en la lista de precios.
        EX_NO_SE_ENCUENTRAN_RANGO_EN_LISTA_DE_PRECIOS = 21,

        /// <summary>
        /// No existe una prima de seguro para la lista de precio y servicio.
        /// </summary>
        EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO = 22,

        /// <summary>
        /// El peso no se encuentra dentro de un rango.
        /// </summary>
        EX_PESO_FUERA_RANGO = 23,

        /// <summary>
        /// El valor declarado es menor que el valor mínimo declarado.
        /// </summary>
        EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO = 24,

        /// <summary>
        /// El valor declarado es mayor que el valor máximo declarado.
        /// </summary>
        EX_VALOR_DECLARADO_MAYOR_QUE_MAXIMO_DECLARADO = 25,

        /// <summary>
        /// No está configurado un concepto de caja para el servicio
        /// </summary>
        EX_NO_EXISTE_CONCEPTO_SERVICIO = 26,

        /// <summary>
        /// El Valor Adicional no está asignado al servicio actual.
        /// </summary>
        EX_RAPI_RADICADO_NO_ASIGNADO_SERVICIO = 27,

        /// <summary>
        /// El servicio actual no tiene precio configurado.
        /// </summary>
        EX_PRECIO_SERVICIO_NO_CONFIGURADO = 28,

        /// <summary>
        /// El valor a cobrar no se encuentra en ningún rango de precios
        /// </summary>
        EX_VALOR_COBRAR_FUERA_RANGOS = 29,

        /// <summary>
        /// El servicio de mensajeria  no esta configurado el servicio de mensajeria
        /// </summary>
        EX_MENSAJERIA_NO_CONF_LISTA_SERVICIO = 30,

        EX_LISTA_PRECIOS_KOMPRECH_INVALIDA = 31,
        EX_SERVICIO_NO_VALIDO = 32,
        EX_RUTA_NO_DISPONIBLE,

        EX_ERROR_CREANDO_LISTA_PRECIOS,

        /// <summary>
        /// Tipo de Cuenta Externa No Existe
        /// </summary>
        EX_ERROR_ACTUALIZAR_TIPO_CUENTA_EXTERNA = 35,

        /// <summary>
        /// La tarifa contra pago no está parametrizada correctamente
        /// </summary>
        EX_TARIFA_CONTRAPAGO_NO_CONFIGURADA = 36


    }
}
