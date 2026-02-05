using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.CentroServicios.Comun
{
    /// <summary>
    /// Enumeración con los códigos de mensajes de Centro de Servicios
    /// </summary>
    public enum EnumTipoErrorCentroServicios : int
    {
        /// <summary>
        /// Mensaje de error {0} no se encuentra configurado
        /// </summary>
        EX_MENSAJE_NO_CONFIGURADO = 0,

        /// <summary>
        /// Cuando la agencia o Punto de Servicio no se encuentran en estado ACT
        /// </summary>
        EX_CENTRO_SERVICIOS_NO_ACTIVO = 1,

        /// <summary>
        /// Cuando la agencia esta bloqueada para vender giros
        /// </summary>
        EX_CENTRO_SERVICIOS_NO_PUEDE_VENDER_GIROS = 2,

        /// <summary>
        /// Cuando la agencia no tiene asociado el servicio de giros
        /// </summary>
        EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS = 3,

        /// <summary>
        /// Cuando el centro de servicion no se encuentra en el sistema
        /// </summary>
        EX_CENTRO_SERVICIOS_NO_EXISTE = 4,
        /// <summary>
        /// Cuando una identificación se encuentra dentro de las listas restrictivas
        /// </summary>
        EX_ALERTA_LISTAS_RESTRICTIVAS = 5,

        /// <summary>
        /// Indica la opcion del estado en liquidacion para los centros de servicio
        /// </summary>
        IN_ESTADO_LIQUIDACION = 6,

        /// <summary>
        /// Indica el tipo de centro de servicio Racol
        /// </summary>
        IN_TIPO_CENTRO_SERVICIO_RACOL = 7,

        /// <summary>
        /// Indica el tipo de centro de servicio Col
        /// </summary>
        IN_TIPO_CENTRO_SERVICIO_COL = 8,

        /// <summary>
        /// Indica el tipo de centro de servicio agencia
        /// </summary>
        IN_TIPO_CENTRO_SERVICIO_AGENCIA = 9,

        /// <summary>
        /// Indica el tipo de centro de servicio Punto
        /// </summary>
        IN_TIPO_CENTRO_SERVICIO_PUNTO = 10,

        /// <summary>
        /// Cuando la agencia esta bloqueada para pagar giros
        /// </summary>
        EX_CENTRO_SERVICIOS_NO_PUEDE_PAGAR_GIROS = 11,
        /// <summary>
        /// Cuando no existe una agencia configurada para un municipio
        /// </summary>
        EX_NO_AGENCIA_EN_MUNICIPIO = 12,

        /// <summary>
        /// Cuando no existe un racol configurado para una ciudad
        /// </summary>
        EX_NO_RACOL_EN_CIUDAD = 13,

        /// <summary>
        /// Cuando ya existe un col en una ciudad
        /// </summary>
        EX_EXISTE_COL_EN_CIUDAD = 14,

        /// <summary>
        /// Cuando ya existe un racol en una ciudad
        /// </summary>
        EX_EXISTE_RACOL_EN_CIUDAD = 15,

        /// <summary>
        /// Cuando ya existe un agencia en una ciudad
        /// </summary>
        EX_EXISTE_AGENCIA_EN_CIUDAD = 16,

        /// <summary>
        /// El Centro de servicios a superado el monto maximo permitidos de giros
        /// </summary>
        EX_AGENCIA_SUPERO_MAX_VENTA_GIROS = 17,

        /// <summary>
        /// Ya existe una lista de precios con tarifa plena, estado activa y fecha vigente en el sistema.
        /// </summary>
        EX_EXISTE_PERSONA_CONTRATO = 18,

        /// <summary>
        /// Error al adjuntar un archivo
        /// </summary>
        EX_FALLO_ADJUNTAR_ARCHIVO = 19,

        /// <summary>
        /// Error al eliminar un archivo
        /// </summary>
        EX_FALLO_ELIMINAR_ARCHIVO = 20,

        /// <summary>
        ///
        /// </summary>
        EX_LOCALIDAD_SIN_AGENCIAS = 21,

        /// <summary>
        /// El trayecto seleccionado no es válido
        /// </summary>
        EX_TRAYECTO_NO_VALIDO = 22,
        /// <summary>
        /// Indica el tipo para quien se esta creando el documento de referencia
        /// </summary>
        IN_TIPO_DOCUMENTO_CENTRO_SERVICIOS = 23,
        /// <summary>
        /// Indica el tipo para quien se esta creando el documento de referencia
        /// </summary>
        IN_TIPO_DOCUMENTO_AGENTE_COMERCIAL = 24,

        /// <summary>
        /// Indica que no esta configurada la plantilla para la divulgacion del centro de servicio
        /// </summary>
        EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA = 25,

        /// <summary>
        /// Indica que no existe un centro de servicio configurado
        /// </summary>
        EX_CENTRO_SERVICIO_NO_EXISTE = 26,

        /// <summary>
        /// Indica que no existe un centro de servicio configurado
        /// </summary>
        EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE = 27,

        /// <summary>
        /// Indica que no existen agencias asociadas al centro de servicio
        /// </summary>
        EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS = 28,

        /// <summary>
        /// La agencia asociada al número del giro no existe o esta inactiva
        /// </summary>
        EX_AGENCIA_NO_EXISTE_INACTIVA,


    }
}
