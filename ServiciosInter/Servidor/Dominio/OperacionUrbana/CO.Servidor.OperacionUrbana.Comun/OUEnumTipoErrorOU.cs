using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana.Comun
{
    /// <summary>
    /// Enumeración con los códigos de mensajes de Operacion Urbana
    /// </summary>
    public enum OUEnumTipoErrorOU : int
    {
        /// <summary>
        /// Estado Activo.
        /// </summary>
        IN_ACTIVO = 1,

        /// <summary>
        /// Estado Inactivo
        /// </summary>
        IN_INACTIVO = 2,

        /// <summary>
        /// Estado Suspendido
        /// </summary>
        IN_SUSPENDIDO = 3,

        /// <summary>
        /// El tipo de vinculacion seleccionado no coincide con el tipo de vinculacion registrado
        /// </summary>
        EX_NO_CUMPLE_TIPO_VINCULACION,

        /// <summary>
        /// El numero de cedula ingresado, no existe en la base de datos
        /// </summary>
        EX_NO_EXISTE_PERSONA,

        /// <summary>
        /// El mensajero Seleccionado no existe en la base de datos
        /// </summary>
        EX_MENSAJERO_NO_EXISTE,

        /// <summary>
        /// La fecha de terminacion del contrato no esta vigente
        /// </summary>
        EX_CONTRATO_NO_ESTA_VIGENTE,

        /// <summary>
        /// EL estado del empaque esta vacio y el cliente es credito con suministro de bolsa de seguridad
        /// </summary>
        EX_FALTA_ESTADO_EMPAQUE,

        /// <summary>
        /// LA guia ya se encuentra registrada en el sistema
        /// </summary>
        EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA,

        /// <summary>
        /// Se encuetran diferencias en el peso de ingreso al centro de acopio y el registrado en el sistema
        /// </summary>
        EX_DIFERENCIAS_PESO,

        /// <summary>
        /// La guía a ingresar no fue provisionada al punto
        /// </summary>
        EX_GUIA_NO_PROVISIONADA,

        /// <summary>
        /// El cliente crédito no tiene presupuesto para el envio
        /// </summary>
        EX_CLIENTE_CON_PRESUPUESO_VENCIDO,

        /// <summary>
        /// El cliente credito tiene bolsas de seguridad
        /// </summary>
        EX_SIN_BOLSA_DE_SEGURIDAD,

        /// <summary>
        /// Guias que no fueron planilladas
        /// </summary>
        EX_GUIAS_SIN_PLANILLAR,

        /// <summary>
        /// Guias que no se encuentran capturadas en sistema
        /// </summary>
        EX_GUIA_NO_ESTA_REGISTRADA,
        /// <summary>
        /// La ciudad de asignacion es diferente a la ciudad destino de la guia
        /// </summary>
        EX_CIUDAD_ASIGNACION_DIFERENTE_CIUDAD_DESTINO_GUIA,

        /// <summary>
        /// La guia se encuentra planillada
        /// </summary>
        EX_GUIA_ESTA_EN_PLANILLA_ASIGNACION,

        /// <summary>
        /// El envio se encuentra en un estado no valido para planillar
        /// </summary>
        EX_EL_ENVIO_SE_ENCUENTRA_EN_UN_ESTADO_NO_VALIDO,

        /// <summary>
        /// El envio no esta asignado o esta asignado a otra planilla
        /// </summary>
        EX_EL_ENVIO_NO_ESTA_ASIGNADO,

        /// <summary>
        /// La planilla tiene envios sin verificar
        /// </summary>
        EX_ENVIOS_SIN_VERIFICAR,

        /// <summary>
        /// Usuario que verifica debe ser diferente al usuario q asigna
        /// </summary>
        EX_ERROR_USUARIO_VERIFICA,

        /// <summary>
        /// No se puede abrir la planilla porque, la fecha de creación es diferente a la fecha de apertura
        /// </summary>
        EX_NO_SE_PUEDE_ABRIR_LA_PLANILLA_ASIGNACION,

        /// <summary>
        /// La guía ya se encuentra verificada
        /// </summary>
        EX_LA_GUIA_ESTA_VERIFICADA,

        /// <summary>
        /// La guía ya se encuentra registrada para la planilla y agencia ingresada
        /// </summary>
        EX_GUIA_INGRESADA_PLANILLA_AGENCIA,

        /// <summary>
        /// la guía no se puede ingresar por logica de negocios en el estado
        /// </summary>
        EX_NO_SE_PUEDE_INGRESAR_GUIA_POR_ESTADOS,

        /// <summary>
        /// Error en la creación de la recogida
        /// </summary>
        EX_ERROR_RECOGIDA,

        /// <summary>
        /// Error en la asignacion del consecutivo de la planilla
        /// </summary>
        EX_ERROR_CONSECUTIVO_PLANILLA_RECOGIDA,

        /// <summary>
        /// Error no existe la planilla ingresada
        /// </summary>
        EX_NO_EXISTE_PLANILLA,

        /// <summary>
        /// Error para la recogida descargada
        /// </summary>
        EX_RECOGIDA_DESCARGADA,

        /// <summary>
        /// Error para las recogidas reabiertas
        /// </summary>
        EX_RECOGIDA_ABIERTA,

        /// <summary>
        /// Error cuando realiza un cambio de estado
        /// </summary>
        EX_CAMBIO_ESTADO_NO_VALIDO,

        /// <summary>
        /// Mensaje de error al intentar deshacer la entrega de una guía alcobro
        /// </summary>
        EX_DESHACER_DESCARGUE_INVALIDO,

        /// <summary>
        /// Mensaje de error para los envios que no se apoyan en el centro logistico de la asignacion
        /// </summary>
        EX_DESTINO_ENVIO_NO_SE_APOYA_COL_ASIGNACION,

        /// <summary>
        /// Mensaje de la guía no se encuentra supervisadaon
        /// </summary>
        EX_GUIA_NO_SUPERVISADA,

        EX_INGRESE_ENVIO_EN_SECCION_CARGA,

        EX_INGRESE_ENVIO_EN_SECCION_MENSAJERIA,

        /// <summary>
        /// eRROR PARA LA REASIGANACION DE LA PLANILLA
        /// </summary>
        EX_PLANILLA_CERRADA_O_NO_EXISTE,

        /// <summary>
        /// Error cuando
        /// </summary>
        EX_ENVIO_ALCOBRO_NO_PAGO,
        /// <summary>
        /// Indica que la guia ya se encuentra descargada
        /// </summary>
        EX_LA_GUIA_YA_ESTA_DESCARGADA,

        /// <summary>
        /// Indica que el parametro que esta buscando no existe
        /// </summary>
        EX_PARAMETRO_NO_EXISTE,

        /// <summary>
        /// Mensaje de error al asignar una tula/contenedor
        /// </summary>
        EX_ERROR_ASIGNACION,
        /// <summary>
        /// El número de guía no pertenece a los envíos admitidos por el punto de servicio
        /// </summary>
        EX_GUIA_NO_PERTENECE_PUNTO,
        /// <summary>
        /// El número de guía ya se encuentra cargado en la planilla
        /// </summary>
        EX_GUIA_YA_PLANILLADA_VENTAS,
        /// <summary>
        /// La pieza ingresada ya fue planillada
        /// </summary>
        EX_PIEZA_ROTULO_YA_PLANILLADA,

        EX_ERROR_ESTADO_CONSOLIDADO,
        /// <summary>
        /// La guía ya fué ingresada al centro de acopio
        /// </summary>
        EX_GUIA_YA_INGRESADA_CENTRO_ACOPIO,
        /// <summary>
        /// La planilla ya fue asignada al mensajero: {0}
        /// </summary>
        EX_PLANILLA_YA_ASIGNADA,

        /// <summary>
        /// Este numero de guia ya fue ingresado como sobrante
        /// </summary>
        EX_SOBRANTE_YA_AUDITADO,

        /// <summary>
        /// Este numero de guia ya fue ingresado como Reclame en Oficina
        /// </summary>
        EX_GUIA_RECLAME_OFICINA,

        /// <summary>
        /// El envío no se encuentra en centro de acopio para asignación a mensajero.
        /// </summary>
        EX_ENVIO_NO_SE_ENCUENTRA_CENTRO_ACOPIO_ASIG_MENSAJERO,

        /// <summary>
        /// No se puedo guardar el envío. La guía está asignada a la planilla {0}
        /// </summary>
        EX_GUIA_ESTA_PLANILLADA_ASIGNACION_MESAJERO,

        /// <summary>
        /// No es posible asignar a Mensajero. El envío se encuentra en estado Auditoría, su asignación debe ser realizada por Salida de Logística Inversa.
        /// </summary>
        EX_GUIA_EN_AUDITORIA_ASIGNACION_MENSAJERO,
        /// <summary>
        /// El envío no es posible asignarlo a Mensajero, verifique el movimiento de inventario de bodegas a donde se encuentra asignado
        /// </summary>
        EX_GUIA_ASIGNADA_A_OTRO_INVENTARIO,
        
        /// <summary>
        /// No es posible asignar a auditor. El envío no se encuentra en Auditoria, su asignacion debe ser realizada por asignación a mensajero. 
        /// </summary>
        EX_GUIA_NO_SE_ENCUENTRA_EN_AUDITORIA

    }
}