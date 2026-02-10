using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Clientes.Comun
{
  /// <summary>
  /// enumeracion de errores de cliente
  /// </summary>
  public enum CLEnumTipoErrorCliente : int
  {
    #region Mensajes de Excepción

    /// <summary>
    /// Ya existe una lista de precios con tarifa plena, estado activa y fecha vigente en el sistema.
    /// </summary>
    EX_EXISTE_PERSONA_CONTRATO = 1,

    /// <summary>
    /// Error al adjuntar un archivo
    /// </summary>
    EX_FALLO_ADJUNTAR_ARCHIVO = 2,

    /// <summary>
    /// Error al eliminar un archivo
    /// </summary>
    EX_FALLO_ELIMINAR_ARCHIVO = 3,

    /// <summary>
    /// Error al eliminar un archivo
    /// </summary>
    EX_FALLO_ELIMINAR_REQUISITO = 4,

    /// <summary>
    ///Error al ingresar valor del contrato
    /// </summary>
    EX_FALLO_VALOR_CONTRATO = 5,

    /// <summary>
    ///Error al validar el presupuesto mensual
    /// </summary>
    EX_FALLO_PRESU_MES = 6,

    /// <summary>
    /// Error al validar el valor del contrato
    /// </summary>
    EX_FALLO_PRESU_CONTRATO = 7,

    /// <summary>
    /// Indica que no esta configurada la plantilla para la divulgacion del centro de servicio
    /// </summary>
    EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA = 8,

    EX_CLIENTE_NO_EXISTE = 9,

    /// <summary>
    /// El contrato no existe
    /// </summary>
    EX_CONTRATO_NO_EXISTE = 11,

    EX_SUCURSAL_O_CONTRATO_NO_EXISTEN = 12,

    EX_NIT_YA_EXISTE = 13,

    EX_NIT_LISTA_NEGRA = 14,
      /// <summary>
      /// El cliente no tiene sucursales con contratos activos
      /// </summary>
    EX_SUCURSAL_SIN_CONTRATOS =15,   


    #endregion Mensajes de Excepción

    #region Mensajes Informativos

    /// <summary>
    /// Escribe en la tabla ArchivoCedulasClientes_CLI el nombre del adjunto
    /// </summary>
    IN_ARCHIVO_CEDULA_DESTINATARIO,

    /// <summary>
    /// Escribe en la tabla ArchivoCedulasClientes_CLI la descripción del adjunto
    /// </summary>
    IN_ARCHIVO_DESC_CEDULA_DESTINATARIO,

    #endregion Mensajes Informativos
  }
}