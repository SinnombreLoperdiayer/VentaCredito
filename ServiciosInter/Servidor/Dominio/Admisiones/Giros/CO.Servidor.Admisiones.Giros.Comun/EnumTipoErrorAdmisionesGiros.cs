namespace CO.Servidor.Admisiones.Giros.Comun
{
    /// <summary>
    /// Enumeración con los códigos de mensajes de Admision de giros
    /// </summary>
    public enum EnumTipoErrorAdmisionesGiros : int
    {
        #region Mensajes de Excepción

        /// <summary>
        /// Mensaje de error {0} no se encuentra configurado
        /// </summary>
        EX_MENSAJE_NO_CONFIGURADO,

        /// <summary>
        /// El número del giro no se encuentra asociado a ninguna Agencia
        /// </summary>
        EX_NUMERO_GIRO_NO_ASOCIADO_A_AGENCIA,

        /// <summary>
        /// El número de giro ya fue creado con anterioridad en el sistema.
        /// </summary>
        EX_NUMERO_GIRO_EXISTE,

        /// <summary>
        /// Cuando el cliente ha superao el monto maximo a enviar en un dia
        /// </summary>
        EX_SUPERO_VALOR_MAXIMO_GIRO,

        /// <summary>
        /// Cuando existen errores al crear el numero de giro en suministros
        /// </summary>
        EX_NO_SE_PUEDE_CREAR_NUM_GIRO_AUTOMATICO,

        /// <summary>
        /// Para poder realizar el giro es obligatorio el Formato de declaración voluntaria de fondos
        /// </summary>
        EX_DECLARACION_VALUNTARIA_FONDOS,

        /// <summary>
        /// Cuando el giro se quiere pagar pero tiene solicitudes activas
        /// </summary>
        EX_NO_SE_PUEDE_PAGAR_SOLICITUDES_ACTIVAS,

        /// <summary>
        /// Error cuando el estado no esta activo, "se encuentra en estado pagado o reservado"
        /// </summary>
        EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO,

        /// <summary>
        /// Cuando no se encuentra el giro por numero de giro
        /// </summary>
        EX_GIRO_NO_EXISTE,

        /// <summary>
        /// El giro no se pudo realizar, Por favor intente nuevamente
        /// Cuando se consulta el giro y este no esta pagado
        /// </summary>
        EX_GIRO_NO_SE_PUDO_PAGAR,

        /// <summary>
        /// No se encontro ningun pago
        /// </summary>
        EX_PAGO_NO_EXISTE,

        /// <summary>
        /// No se encontró ningún giro con los criterios de búsqueda.
        /// </summary>
        EX_NO_SE_ENCONTRO_GIROS,

        /// <summary>
        /// Mensaje de Giro ya transmitido a agencia.
        /// </summary>
        EX_GIRO_TRANSMITIDO,

        /// <summary>
        /// Mensaje de Error de El número de giro no puede ser
        /// admitido por este centro de servicio. Verifique que la
        /// guía pertenezca al punto de servicio autenticado o que esté
        /// autenticado como el RACOL al que pertenece el centro
        /// de servicio propietario de la guía
        /// </summary>
        EX_CENTRO_SERVICIO_NO_PUEDE_ADMITIR_GUIA_SERVIDOR,

        #endregion Mensajes de Excepción

        #region Mensajes Informativos

        /// <summary>
        /// Escribe en la tabla ArchivosGiros_GIR el nombre del adjunto Declaracion voluntaria de fondos
        /// </summary>
        IN_ARCHIVO_DECLARACION_VOLUNTARIA_FONDOS,

        /// <summary>
        /// Escribe en la tabla ArchivosGiros_GIR la descripción del adjunto  Declaracion voluntaria de fondos
        /// </summary>
        IN_ARCHIVO_DESC_DECLARACION_VOLUNTARIA_FONDOS,

        /// <summary>
        /// Escribe en la tabla ArchivosGiros_GIR el nombre del adjunto Cedula del destinatario
        /// </summary>
        IN_ARCHIVO_CEDULA_DESTINATARIO,

        /// <summary>
        /// Escribe en la tabla ArchivosGiros_GIR la descripción del adjunto Cedula del destinatario
        /// </summary>
        IN_ARCHIVO_DESC_CEDULA_DESTINATARIO,

        /// <summary>
        /// Documento de Autorización del pago
        /// </summary>
        IN_ARCHIVO_AUTORIZACION_PAGO,

        /// <summary>
        /// Descripcion del Documento de Autorización del pago
        /// </summary>
        IN_ARCHIVO_DESC_AUTORIZACION_PAGO,

        /// <summary>
        /// Documento Certificado Empresarial
        /// </summary>
        IN_ARCHIVO_CERTIFICADO_EMPRESARIAL,

        /// <summary>
        /// Descripcion Documento Certificado Empresarial
        /// </summary>
        IN_ARCHIVO_DESC_CERTIFICADO_EMPRESARIAL,

        #endregion Mensajes Informativos
    }
}