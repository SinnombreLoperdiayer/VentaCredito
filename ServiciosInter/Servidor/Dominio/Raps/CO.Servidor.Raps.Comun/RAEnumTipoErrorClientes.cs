namespace CO.Servidor.Raps.Comun
{
    public enum RAEnumTipoErrorClientes : int
    {
        /// <summary>
        /// error en los datos a guardar 
        /// </summary>
        EX_INSERTAR_PARAMETRIZACION = 1,

        /// <summary>
        /// eRROR AL GURADAR UNA SOLICITUD
        /// </summary>
        EX_INSERTAR_SOLICITUD = 2,

        /// <summary>
        /// EL PARAMETRO ENVIADO JNO CORRESPONDE CON EL TIPO DE DATO
        /// </summary>
        EX_PARAMETRO_NO_ES_EL_ESPERADO = 3,

        /// <summary>
        /// EL NUEVO ESTADO PARA LA GESTION ES DE CERRADO Y LA SOLICITUD NO SE ENCUENTRA EN ESTADO RESPUESTA
        /// </summary>
        EX_NO_ES_POSIBLE_CERRAR_LA_SOLICITUD = 4,

        /// <summary>
        /// LA SOLICITUD FUE MODIFICADA
        /// </summary>
        EX_SOL_CAMBIO_DE_ESTADO = 5,

        /// <summary>
        /// ERROR AL CONSULTAR LA SOLICITUD
        /// </summary>
        EX_CONSULTA_SOLICITUD = 6,

        /// <summary>
        /// No existe informacion para la guia con ese estado
        /// </summary>
        EX_CONSULTA_TRAZA_POR_ESTADO = 7,

        /// <summary>
        /// No existe responsable para centro de servicio
        /// </summary>
        EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO = 8,

        /// <summary>
        /// No existe información para el mensajero
        /// </summary>
        EX_CONSULTA_DATOS_MENSAJERO = 9,

        /// <summary>
        /// nO EXISTE RESPONSABLE ASOCIADO AL SUMINISTRO
        /// </summary>
        EX_CONSULTA_RESPONSABLE_SUMINISTRO = 10,

        /// <summary>
        /// NO EXISTE GUIA DIGITADA
        /// </summary>
        EX_CONSULTA_GUIA_NO_EXISTE = 11,

        /// <summary>
        /// ERROR AL CONSULTAR NOVEDAD HIJA SEGUN PADRE Y RESPONSABLE
        /// </summary>
        EX_CONSULTA_NOVEDAD_HIJA = 12,

        /// <summary>
        /// NO CUMPLE CON VALIDACION DE UNICO ENVIO
        /// </summary>
        EX_FALLA_YA_REGISTRADA_MISMO_RESPONSABLE = 13,

        /// <summary>
        /// NO EXISTE REGLA PARAMETRIZADA PARA EL ESTADO
        /// </summary>
        EX_NO_EXISTE_REGLA_PARA_ESTADO = 14,

        /// <summary>
        /// No se asigno tipo novedad o parametros
        /// </summary>
        EX_NO_ASIGNO_TIPONOVEDAD = 15,

        /// <summary>
        /// No se encontro responsable manifiesto asignación para el núnmero de guía.
        /// </summary>
        EX_NO_RESPONSABLE_MANIFIESTO = 16,

        /// <summary>
        /// No aplica esta guia para la falla, el responsable debe ser una agencia o un punto.
        /// </summary>
        EX_RESPONSABLE_DIFERENTE_AGE_PTO = 17,

         /// <summary>
         /// No aplica esta guia para la falla, el responsable debe ser una racol.
         /// </summary>
        EX_RESPONSABLE_DIFERENTE_RACOL = 18,

        /// <summary>
        /// No aplica esta guia para la falla, el responsable debe ser una racol.
        /// </summary>
        EX_NO_CONTIENE_ID_CENTRO_SERVICIO_ESTADO= 19,
    }
}
