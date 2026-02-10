using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
    public enum LOIEnumTipoErrorLogisticaInversa : int
    {
        /// <summary>
        /// No existen manifiestos que cumplan los criterios de busqueda
        /// </summary>
        EX_NO_EXISTEN_MANIFIESTOS = 1,

        /// <summary>
        /// El suministro no estra provisionado
        /// </summary>
        EX_SUMINISTRO_SIN_PROVISION = 2,

        /// <summary>
        /// Error al cambiar estado
        /// </summary>
        EX_CAMBIO_ESTADO_NO_VALIDO = 3,

        /// <summary>
        /// Error al ingresar un detalle en manifiesto
        /// </summary>
        EX_PRUEBA_NO_VALIDA = 4,

        /// <summary>
        /// Error de guía manifestada no descargada
        /// </summary>
        EX_GUIA_NO_DESCARGADA = 5,

        /// <summary>
        /// Error de guía descargada mal diligenciada
        /// </summary>
        EX_GUIA_MAL_DILIGENCIADA = 6,

        /// <summary>
        /// El usuario actual no pertenece a un COL
        /// </summary>
        EX_USUARIO_NO_COL = 7,

        /// <summary>
        /// La guía no está digitalizada
        /// </summary>
        EX_GUIA_NO_DIGITALIZADA = 8,

        /// <summary>
        /// La guía no se encontró en el sistema
        /// </summary>
        EX_GUIA_NO_EXISTE = 9,

        /// <summary>
        /// No existe una caja activa para el COL.
        /// </summary>
        EX_CAJA_ACTIVA_COL_NO_EXISTE = 10,

        /// <summary>
        /// La guía ya fue archivada.
        /// </summary>
        EX_GUIA_YA_FUE_ARCHIVADA = 11,

        /// <summary>
        /// Error cuando un manifiesto no tiene guías asociadas
        /// </summary>
        EX_MANIFIESTO_SIN_GUIAS = 12,

        /// <summary>
        /// Error de telemercadeo al cambiar el estado, de axcuerdo a un resultado de gestión
        /// </summary>
        EX_ERROR_CAMBIO_ESTADO = 13,

        EX_ERROR_ESTADO_DEV = 14,

        EX_ERROR_CLIENTE_GUIA = 15,

        EX_ERROR_ALCOBRO_NOPAGO = 16,

        /// <summary>
        /// Error de rexpedicion cuando el envio no se encuentra en estado EnCentroAcopio
        /// </summary>
        EX_ERROR_ESTADO_DIF_CENTROACOPIO = 17,

        /// <summary>
        /// Error para los servicios que no tienen configurada una reexpedicion
        /// </summary>
        EX_ERROR_SERVICIO_NO_CONFIGURADO_REEXPEDICION = 18,

        EX_ARCHIVO_NO_ENCONTRADO = 19,

        EX_ERROR_GUIA_INTERNA_MANIFIESTO = 20,

        /// <summary>
        /// Error al crear la planilla de certificacion
        /// </summary>
        EX_ERROR_PLANILLA_CERTIFICACION = 21,

        /// <summary>
        /// Error cuando el envio no tiene digitalizada la prueba de entrega
        /// </summary>
        EX_ERROR_FALTA_DIGITALIZAR_PRUEBA_ENTREGA = 22,

        /// <summary>
        /// Error cuando
        /// </summary>
        EX_ERROR_FALTA_CAPTURAR_DATOS_ENVIO = 23,

        /// <summary>
        /// Error cuando la guia ya se encuentra planillada
        /// </summary>
        EX_ERROR_GUIA_YA_PLANILLADA = 24,

        /// <summary>
        /// Error cuando la planilla no existe
        /// </summary>
        EX_PLANILLA_NO_EXISE = 25,

        /// <summary>
        /// Contenido de la guia interna de la certificacion
        /// </summary>
        IN_CONTENIDO_GUIA_INTERNA_CERTIFICACION,

        /// <summary>
        /// Contenido de la guia interna de la planilla
        /// </summary>
        IN_CONTENIDO_GUIA_INTERNA_PLANILLA,

        /// <summary>
        /// La gestión no se recibió correctamente, esta vacia o fue nula, por favor valide los datos de la gestión, si todo está correcto por favor comunicarse con servicio técnico
        /// </summary>
        EX_OBJETO_GESTION_NULL,

        /// <summary>
        /// Los datos de recibido de una guía dada ya han sido capturados previamente
        /// </summary>
        EX_ERROR_DATOS_RECIBIDO_YA_CAPTURADOS,

        /// <summary>
        /// La guía ya fue ingresada en una planilla de devolución
        /// </summary>
        EX_ERROR_GUIA_YA_DEVUELTA,

        EX_GUIA_NO_CREDITO,

        EX_ERROR_GUIA_DESTINO,

        EX_ERROR_NOTIFICACION,

        EX_ERROR_SUCURSAL_ACTIVA,

        EX_ERROR_DIRECCION_NO_EXISTE,

        EX_ERROR_CENTROACOPIO_CESDIFERENTE,

        EX_ERROR_NO_ESTADO_PENDIENTE_INGRESO_CUSTODIA,

        EX_ERROR_NO_INGRESADO_CUSTODIA,

        EX_ERROR_FTP,

        EX_ERROR_PESO_GUIA,
        EX_ERROR_CLIENTE_GUIA_DEVOLUCION,
        /// <summary>
        /// La guia es credito
        /// </summary>
        EX_GUIA_CREDITO,

        /// <summary>
        /// La Guía no tiene Captura Recibido
        /// </summary>
        EX_ERROR_GUIA_SIN_RECIBIDO_CAPTURADO,

        /// <summary>
        /// La Guia no tiene forma de pago Contado
        /// </summary>
        EX_ERROR_GUIA_NO_ES_CONTADO,

        /// <summary>
        /// La Guia no corresponde al servicio de Notificaciones
        /// </summary>
        EX_ERROR_GUIA_NO_ES_NOTIFICACION,
    }
}