namespace CO.Servidor.ControlCuentas.Comun
{
    /// <summary>
    /// Enumeración con los códigos de mensajes de Admision de giros
    /// </summary>
    public enum EnumTipoErrorControlCuentas : int
    {
        #region Mensajes de Excepción

        /// <summary>
        /// Mensaje de error {0} no se encuentra configurado
        /// </summary>
        EX_MENSAJE_NO_CONFIGURADO,

        /// <summary>
        /// No se encontro ninguna persona en NovaSoft
        /// </summary>
        EX_PERSONA_NO_EXISTE,

        /// <summary>
        /// El punto de servicio de destino no admite la forma de pago al cobro
        /// </summary>
        EX_PUNTO_NO_ADMITE_FORMA_PAGO_ALCOBRO,

        /// <summary>
        /// La forma de pago Crédito no se puede cambiar a ninguna otra forma de pago.
        /// </summary>
        EX_FORMA_PAGO_CREDITO_INVALIDA,

        /// <summary>
        /// No se puede realizar el cambio de forma de pago { 0 }  a { 1 } .
        /// </summary>
        EX_FORMA_PAGO_INVALIDA,

        /// <summary>
        /// No se ha seleccionado el contrato del cliente crédito.
        /// </summary>
        EX_CONTRATO_INVALIDO,

        /// <summary>
        /// El estado actual de la guía no permite anularla.
        /// </summary>
        EX_ESTADO_GUIA_NO_VALIDO,

        /// <summary>
        /// No fué posible efectuar el cambio.No se detectó la forma de pago de la factura/guía
        /// </summary>
        EX_ERROR_FORMA_DE_PAGO_NO_ENCONTRADA,

        #endregion Mensajes de Excepción

        #region Mensajes Informativos

        /// <summary>
        ///MODIFICACIÓN DESTINO ->Texto de la tabla NovedadGuiaDatosAdicionale_MEN  nombre
        /// </summary>
        IN_MODIFICACION_DESTINO,

        IN_DESCUENTO_POR_CAMBIO_FORMA_PAGO,

        /// <summary>
        /// Anulación de la una guia
        /// </summary>
        IN_ANULACION_GUIA,

        /// <summary>
        /// Modificación de tipo de servicio de la admisión
        /// </summary>
        IN_CAMBIO_SERVICIO,

        IN_GUIA_NO_EXISTE,
        IN_GUIA_NO_APROVISIONADA,
        IN_ANULADO,

        /// <summary>
        /// Ajuste x Cambio de Destino guía/factura No.
        /// </summary>
        IN_AJUSTE_CAMBIO_DESTINO

        #endregion Mensajes Informativos
    }
}