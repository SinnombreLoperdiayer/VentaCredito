using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    /// <summary>
    /// Contiene enumeración asociada a los suministros de la aplicación
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public enum SUEnumSuministro : int
    {
        [EnumMember]
        NOCONFIGURADO = 0,
        [EnumMember]
        GUIA_TRANSPORTE_MANUAL = 1,
        [EnumMember]
        BOLSA_SEGURIDAD_CARTA = 2,
        [EnumMember]
        FACTURA_VENTA_GIRO_POSTAL_MANUAL = 3,
        [EnumMember]
        GUIA_CORRESPONDENCIA_INTERNA = 4,
        [EnumMember]
        FACTURA_VENTA_GIRO_POSTAL_AUTOMATICO = 5,
        [EnumMember]
        COMPROBANTE_PAGO_GIRO_POSTAL_AUTOMATICO = 6,
        [EnumMember]
        FACTURA_VENTA_MENSAJERIA_AUTOMATICA = 7,
        [EnumMember]
        GUIA__TRANSPORTE_AUTOMATICA = 8,
        [EnumMember]
        FACTURA_VENTA_MENSAJERIA_MANUAL = 9,
        [EnumMember]
        GUIA_TRANSPORTE_MANUAL_OFFLINE = 10,
        [EnumMember]
        PIN_PREPAGO = 11,
        [EnumMember]
        BOLSA_SEGURIDAD_GIGANTE = 12,
        [EnumMember]
        BOLSA_SEGURIDAD_MANILA = 13,
        [EnumMember]
        BOLSA_SEGURIDAD_MEDIANA = 14,
        [EnumMember]
        BOLSA_TRANPARENTE_GUIA = 15,
        [EnumMember]
        BOLSA_CORPORATIVA_CARTA = 16,
        [EnumMember]
        BOLSA_CORPORATIVA_GIGANTE = 17,
        [EnumMember]
        BOLSA_CORPORATIVA_MANILA = 18,
        [EnumMember]
        BOLSA_CORPORATIVA_MEDIANA = 19,
        [EnumMember]
        ROLLO_CINTA_100MTS = 20,
        [EnumMember]
        PLANILLA_DESPACHO_MANUAL = 21,
        [EnumMember]
        TALONARIO_DESPACHO_PEQUENO = 22,
        [EnumMember]
        TALONARIO_ENVIOS_DEVUELTOS = 23,
        [EnumMember]
        GUIAS_MASIVAS = 24,
        [EnumMember]
        TALONARIO_MANIFIESTO_NACIONAL = 25,
        [EnumMember]
        VOLANTE_DEVOLUCIÓN_AVISO_PRIMER_INTENTO = 26,
        [EnumMember]
        VOLANTE_DEVOLUCIÓN_AVISO_SEGUNDO_INTENTO = 27,
        [EnumMember]
        PLANILLA_DIARIA_ALCOBROS = 28,
        [EnumMember]
        RELACION_COMPROBANTES_PAGO = 29,
        [EnumMember]
        ROTULOS = 30,
        [EnumMember]
        TALONARIO_REPORTE_DIARIO_VENTAS = 31,
        [EnumMember]
        VOLANTE_CONTRATO = 32,
        [EnumMember]
        COMPROBANTE_PAGO_GIRO_MANUAL = 33,
        [EnumMember]
        RELACION_GIROS_POSTALES_VENDIDOS = 34,
        [EnumMember]
        RECEPCION_TELEFONICA = 35,
        [EnumMember]
        SOLICITUD_MODIFICACION_GIROS_POSTALES = 36,
        [EnumMember]
        MOVIMIENTO_DIARIO_PORTES_GIROS_POSTALES = 37,
        [EnumMember]
        COMPROBANTE_ABONO_EFECTIVO = 38,
        [EnumMember]
        FACTURA_COBRO_CLIENTE_CREDITO = 39,
        [EnumMember]
        GIROS_PRODUCCION = 40,
        [EnumMember]
        PRECINTO_SEGURIDAD = 41,
        [EnumMember]
        VOLANTE_DEVOLUCION_SUPERVISION = 42,
        [EnumMember]
        ADMISION_NN = 761,
        //Pruebas 764, Producción 762
        [EnumMember]
        SUMINISTRO_CONSECUTIVO_TAPA_AUDITORIA = 762,
        [EnumMember]
        IMPRESION_NOTIFICACION_PUNTO = 769
    }
}