using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Servidor.OperacionNacional.Comun
{
    /// <summary>
    /// Enumeracion con los codigos de error del modulo de operacion nacional
    /// </summary>
    public enum EnumTipoErrorOperacionNacional : int
    {
        /// <summary>
        /// Mensaje de error {0} no se encuentra configurado
        /// </summary>
        EX_MENSAJE_NO_CONFIGURADO = 0,

        /// <summary>
        /// Cuando no se pudo insertar un consolidado
        /// </summary>
        EX_FALLO_CREACION_CONSOLIDADO = 1,

        /// <summary>
        /// Cuando no se pudo insertar un consolidado detalle
        /// </summary>
        EX_FALLO_CREACION_CONSOLIDADO_DETALLE = 2,

        /// <summary>
        /// Error cuando se intenta volver a  ingresar una guia a un consolidado
        /// </summary>
        EX_ERROR_GUIA_YA_CONSOLIDADA = 3,

        /// <summary>
        /// Error cuando no se pudo insertar una guia suelta
        /// </summary>
        EX_ERROR_CREACION_GUIA_SUELTA = 4,

        /// <summary>
        /// Error cuando no se encuentra la guia en el centro de acopio de una ciudad
        /// </summary>
        EX_GUIA_NO_INGRESADA_CENTRO_ACOPIO = 5,

        /// <summary>
        /// Error cuando no se puede crear el motivo de eliminacion de una guia de un manifiesto
        /// </summary>
        EX_FALLO_CREACION_MOTIVO_ELIMINACION_GUIA_MANIFIESTO = 6,

        /// <summary>
        /// Error cuando no se puede crear un manifiesto
        /// </summary>
        EX_FALLO_CREACION_MANIFIESTO = 7,

        /// <summary>
        /// Error cuando la guia ya a sido manifestada
        /// </summary>
        EX_ERROR_GUIA_YA_MANIFESTADA = 8,

        /// <summary>
        /// Error cuando la ciudad de destino de la guia no pertenece a la ruta
        /// </summary>
        EX_ERROR_GUIA_NO_RUTA = 9,

        /// <summary>
        /// Error cuando la ciudad de destino de la ruta no pertenece la ciudad que se esta manifestando ni a su area de influencia
        /// </summary>
        EX_ERROR_GUIA_NO_CIUDAD_MANIFIESTA = 10,

        /// <summary>
        /// Error cuando el vehiculo no ha registrado ingreso a la agencia
        /// </summary>
        EX_ERROR_FALTA_INGRESO_VEHICULO = 11,

        /// <summary>
        /// Error cuando la ciudad del ingreso no esta en la ruta
        /// </summary>
        EX_ERROR_CIUDAD_INGRESO_NO_RUTA = 12,

        /// <summary>
        /// Error, al consultar los estados del empaque;
        /// </summary>
        EX_ERROR_ESTADOS_EMPAQUE = 13,

        /// <summary>
        /// Error, al crear el ingreso del operativo a la agencia
        /// </summary>
        EX_ERROR_INGRESO_OPERATIVO = 14,

        /// <summary>
        /// Error para la falla por novedad del precinto
        /// </summary>
        EX_FALLA_NOVEDAD_PRECINTO_CONSOLIDADO = 15,

        /// <summary>
        /// Error, para la falla del estado del empaque sin bolsa de seguridad
        /// </summary>
        EX_FALLA_SIN_BOLSA_SEGURIDAD = 16,

        /// <summary>
        /// Errror, para la diferencia en el peso
        /// </summary>
        EX_FALLA_DIFERENCIA_PESO,

        /// <summary>
        /// No se encuentra ninguna ruta ni conductor asociado a la placa.
        /// </summary>
        EX_NO_EXISTE_CONDUCTOR_RUTA_ASOCIADO_A_PLACA,

        /// <summary>
        /// El sistema valida que el registro a seleccionar (Llegada, Salida)
        /// no sea igual al ultimo registro ingresado para el mismo Vehículo
        /// </summary>
        EX_VEHICULO_YA_REGISTRADO,

        /// <summary>
        /// El envio ingresado no es envio transito
        /// </summary>
        EX_ENVIO_NO_ES_TRANSITO,

        /// <summary>
        /// El envio ingresado no esta manifestado al vehiculo
        /// </summary>
        EX_ENVIO_TRANSITO_NO_MANIFESTADO,

        /// <summary>
        /// Indica que ingreso un transportador
        /// </summary>
        IN_EL_INGRESO,

        /// <summary>
        /// Indica que salio un transportador
        /// </summary>
        IN_LA_SALIDA,

        /// <summary>
        /// Error cuando el envio ya se ingreso en centro acopio
        /// </summary>
        EX_ENVIO_YA_INGRESO,

        /// <summary>
        /// Error cuando el estado de la guia actual, no permiete el cambio a un estado de
        /// de guia segun el diagrama
        /// </summary>
        EX_ERROR_ESTADO_GUIA,

        /// <summary>
        /// No existe ningún consolidado con ese número de guía interna.
        /// </summary>
        EX_NO_EXISTE_CONSOLIDADO_NUM_GUIA,

        /// <summary>
        /// Error cuando la ciudad destino del envio tiene configurada una ruta
        /// </summary>
        EX_CIUDAD_DESTINO_ENVIO_TIENE_RUTA,

        /// <summary>
        /// Mensaje de error al consultar del detalle del manifiesto
        /// </summary>
        EX_ERROR_DETALLE_MANIFIESTO,

        /// <summary>
        /// La guía no se encuentra dentro de un centro de acopio
        /// </summary>
        EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO,

        /// <summary>
        /// La guia no es guia interna
        /// </summary>
        EX_GUIA_NO_ES_INTERNA,

        /// <summary>
        /// No se puede volver a abrir el manifiesto.
        /// </summary>
        EX_NO_SE_PUEDE_ABRIR_MANIFIESTO,

        /// <summary>
        /// No se pueden obtener los datos del conductor
        /// </summary>
        EX_NO_SE_PUEDE_OBTENER_DATOS_CONDUCTOR,

        /// <summary>
        /// No se pueden obtener los datos del vehiculo
        /// </summary>
        EX_NO_SE_PUEDE_OBTENER_DATOS_VEHICULO,

        /// <summary>
        /// Manifiesto sin vehiculo configurado
        /// </summary>
        EX_MANIFIESTO_SIN_VEHICULO_CONFIGURADO,

        /// <summary>
        /// Error cuando una guía interna no esta aprovisionada
        /// </summary>
        EX_GUIA_INTERNA_NO_APROVISIONADA,

        /// <summary>
        /// Error cuando el operativo no ha iniciado
        /// </summary>
        EX_ERROR_OPERATIVO_NO_INICIADO,

        /// <summary>
        /// No existe informacion de la agencia o de la localidad
        /// </summary>
        EX_NO_INFORMACION_AGENCIA_O_LOCALIDAD,

        /// <summary>
        /// No se puede manifestar porque el destino de la guía esta antes de la ciudad a manifestar
        /// </summary>
        EX_NO_SE_PUEDE_MANIFESTAR_DESTINO_GUIA_ANTES_DE_LOCALIDAD_A_MANIFESTAR,

        /// <summary>
        /// No se puede llegar al destino de la guía desde la agencia a la que esta manifestando
        /// </summary>
        EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR,

        /// <summary>
        /// Indica que el parametro que esta buscando no existe
        /// </summary>/// </summary>
        EX_PARAMETRO_NO_EXISTE,

        EX_INGRESO_A_AGENCIA_CERRADO,

        /// <summary>
        /// Indica que el consolidado
        /// </summary>
        EX_CONSOLIDADO_NO_ENCONTRADO,

        /// <summary>
        /// Indica que la guia no existe
        /// </summary>
        EX_GUIA_NO_EXISTE,

        EX_CAMBIO_ESTADO_NO_VALIDO,

        EX_INGRESO_INVALIDO,
        /// <summary>
        /// Indica que un contenedor o una tula ya fueron manifestados
        /// </summary>
        EX_CONTENEDORTULA_YA_MANIFESTADO,
        /// <summary>
        /// La guía ya fué ingresada al centro de acopio
        /// </summary>
        EX_GUIA_YA_INGRESADA_CENTRO_ACOPIO,
        /// <summary>
        /// La Tula/Consolidado no esta activo o no pertenece a la ciudad
        /// </summary>
        EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA,
        
        /// <summary>
        /// La Tula/Consolidado ya esta siendo utilizado en otro proceso
        /// </summary>
        EX_TULA_YA_ESTA_UTILIZADA_OTRO_PROCESO,

        /// <summary>
        /// El vehículo está asignado a uno o varios manifiestos abiertos {0}
        /// </summary>
        EX_VEHICULO_ASIGNADO_A_MANIFIESTO_ABIERTO,
        /// <summary>
        /// El número de  tula o consolidado no existe.
        /// </summary>
        EX_NO_EXISTE_TULA_CONSOLIDADO,

    }
}