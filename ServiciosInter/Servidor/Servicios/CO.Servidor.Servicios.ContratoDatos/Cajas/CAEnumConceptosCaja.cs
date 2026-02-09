using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Numeracion de Conceptos de Caja del mensajero
    /// </summary>
    public enum CAEnumConceptosCaja : int
    {
        [EnumMember]
        ADMISION_GIROS = 8,

        /// <summary>
        /// Pago de Envio al Cobro
        /// </summary>
        [EnumMember]
        PAGO_DE_ENVIO_AL_COBRO = 18,

        /// <summary>
        /// Translado de dinero de la Caja ppal a la Caja Aux
        /// </summary>
        [EnumMember]
        GASTOS_GENERALES = 19,

        /// <summary>
        /// Descuento x Nómina
        /// </summary>
        [EnumMember]
        DESCUENTO_X_NOMINA = 20,

        /// <summary>
        /// Reporte dinero agencia x Ventas
        /// </summary>
        [EnumMember]
        REPORTE_DINERO_AGENCIA_X_VENTAS = 21,

        /// <summary>
        /// Reporte dinero agencia x Entregas al Cobro
        /// </summary>
        [EnumMember]
        REPORTE_DINERO_MENSAJERO_X_ENTREGAS_AL_COBRO = 22,

        [EnumMember]
        VENTA_PIN_PREPAGO = 23,

        /// <summary>
        /// Reporte Dinero de Punto Servicio (id = 27)
        /// </summary>
        [EnumMember]
        REPORTE_DINERO_PUNTO_DE_SERVICIO = 27,

        /// <summary>
        /// Traslado de dinero desde Agencia hacia un RACOL (id = 29)
        /// </summary>
        [EnumMember]
        CONCEPTO_TRASLADO_AGENCIA_A_RACOL = 29,

        /// <summary>
        /// Traslado de dinero desde RACOL hacia cualquier centro de servicios (id = 30)
        /// </summary>
        [EnumMember]
        CONCEPTO_TRASLADO_RACOL_A_CENTRO_SERVICIOS = 30,

        /// <summary>
        /// Traslado de dinero Operación Nacional a RACOL
        /// </summary>
        [EnumMember]
        CONCEPTO_TRASLADO_OPENAL_A_RACOL = 32,

        /// <summary>
        /// Traslado de dinero Operación Nacional a Casa Matriz (id = 33)
        /// </summary>
        [EnumMember]
        CONCEPTO_TRASLADO_OPENAL_A_CASA_MATRIZ = 33,

        /// <summary>
        /// Traslado de dinero desde RACOL hacia cualquier Operación Nacional (id = 39)
        /// </summary>
        [EnumMember]
        CONCEPTO_TRASLADO_RACOL_A_OPN = 39,

        /// <summary>
        /// Concepto de la caja para una guia anulada
        /// </summary>
        [EnumMember]
        CONCEPTOCAJA_GUIA_ANULADA = 42,

        /// <summary>
        /// Translado de dinero de la Caja ppal a la Caja Aux
        /// </summary>
        [EnumMember]
        TRANS_DINERO_ENTRE_CAJAS = 43,

        /// <summary>
        /// Cuando el mensajero no puede entregar un envio AlCobro
        /// </summary>
        [EnumMember]
        TRANS_DESCUENTO_ALCOBRO_DEVUELTO = 46,

        [EnumMember]
        DESCUENTO_POR_DESCARGUE_AL_COBRO_OTRO_CS = 45,

        [EnumMember]
        DESCUENTO_POR_CAMBIO_FORMA_PAGO = 47,

        /// <summary>
        /// Resumen cierres de caja Auxiliar (id = 48)
        /// </summary>
        [EnumMember]
        RESUMEN_CIERRES_CAJA_AUXILIAR = 48,

        /// <summary>
        /// Anulación de Giro
        /// </summary>
        [EnumMember]
        ANULACION_GIRO = 49,

        /// <summary>
        /// Devolucion de giro afectando el porte Motivos:
        /// - Agencia sin Servicio - Agencia sin Dinero - Error Operativo
        /// </summary>
        [EnumMember]
        DEVOLUCION_GIRO_POSTAL = 50,

        /// <summary>
        /// Devolucion de giro sin afectar el porte Motivo:
        /// - Solicitada por Remitente
        /// </summary>
        [EnumMember]
        DEVOLUCION_PORTE_DE_GIRO = 51,

        /// <summary>
        ///Ajuste x Reliquidación en el cambio de destino de una guía o factura
        /// </summary>
        [EnumMember]
        AJUSTE_CAMBIO_DESTINO = 52,

        /// <summary>
        /// Ajuste a valor de giro
        /// puede ser Ingreso ó Egreso dependiendo de la transacción
        /// </summary>
        [EnumMember]
        AJUSTE_A_VALOR_DE_GIRO = 53,

        /// <summary>
        /// Reliquidación porte en ajuste de valor
        /// puede ser Ingreso ó Egreso dependiendo de la transacción
        /// </summary>
        [EnumMember]
        RELIQUIDACION_PORTE_EN_AJUSTE_DE_VALOR = 54,

        /// <summary>
        /// Descuento por cambio de tipo de servicio
        /// </summary>
        [EnumMember]
        DESCUENTO_POR_CAMBIO_TIPO_SERVICIO = 55,

        /// <summary>
        /// Gasto por devolución de porte de giros
        /// </summary>
        [EnumMember]
        GASTO_POR_DEVOLUCION_DE_PORTE_DE_GIROS = 56,

        /// <summary>
        /// Diferencia por cambio en valor total
        /// </summary>
        [EnumMember]
        DIFERENCIA_EN_VALOR_TOTAL = 57,

        /// <summary>
        /// Pago de Clientes Crédito
        /// </summary>
        [EnumMember]
        PAGO_DE_CLIENTES_CREDITO = 59,

        /// <summary>
        /// Anticipo de Clientes Confg
        /// </summary>
        [EnumMember]
        ANTICIPO_DE_CLIENTES = 63,

        /// <summary>
        ///Ajuste x Reliquidación en el cambio de peso de una guía o factura
        /// </summary>
        [EnumMember]
        AJUSTE_CAMBIO_PESO = 65,
    }
}