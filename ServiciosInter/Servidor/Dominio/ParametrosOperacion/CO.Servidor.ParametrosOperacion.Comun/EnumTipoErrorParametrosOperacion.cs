using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ParametrosOperacion.Comun
{
  public enum EnumTipoErrorParametrosOperacion
  {
    /// <summary>
    /// Mensaje de error {0} no se encuentra configurado
    /// </summary>
    EX_MENSAJE_NO_CONFIGURADO = 0,

    /// <summary>
    /// Indica la opcion del estado suspendido
    /// </summary>
    IN_ESTADO_SUSPENDIDO = 1,

    /// <summary>
    /// Mensaje de error indicando que el registro no fue encontrado en novasoft
    /// </summary>
    EX_REGISTRO_NO_ENCONTRADO_NOVASOFT = 2,

    /// <summary>
    /// Estado Activo.
    /// </summary>
    IN_ACTIVO = 3,

    /// <summary>
    /// Estado Inactivo
    /// </summary>
    IN_INACTIVO = 4,

    /// <summary>
    ///  La fecha de terminacion del contrato no esta vigente
    /// </summary>
    EX_CONTRATO_NO_ESTA_VIGENTE = 5,

    /// <summary>
    /// El tipo de vinculacion seleccionado no coincide con el tipo de vinculacion registrado
    /// </summary>
    EX_NO_CUMPLE_TIPO_VINCULACION = 6,

    /// <summary>
    /// El mensajero no tiene informacion de soat y revision tecnomecanica en el sistema
    /// </summary>
    EX_NO_EXISTE_INFO_SOAT_TECNO_MECANICA_MENSAJERO = 7,

    /// <summary>
    /// Ya existe un vehiculo con la placa ingresada
    /// </summary>
    EX_YA_EXISTE_VEHICULO = 8,
    /// <summary>
    /// Error cuando falla la integracion
    /// </summary>
    EX_ERROR_EJECUTANDO_INTEGRACION=9
  }
}