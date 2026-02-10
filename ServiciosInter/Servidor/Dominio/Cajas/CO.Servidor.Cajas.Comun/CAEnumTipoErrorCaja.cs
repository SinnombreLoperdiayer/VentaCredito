using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Cajas.Comun
{
  /// <summary>
  /// Enumeriacion con los códigos de Cajas
  /// </summary>
  public enum CAEnumTipoErrorCaja : int
  {
    /// <summary>
    /// Error no se encuentra Configurado.
    /// </summary>
    EL_ERROR_NO_SE_ENCUENTRA_CONFIGURADO = 0,

    /// <summary>
    /// Error de en la apertura de Caja
    /// </summary>
    ERROR_DE_APERTURA_CAJA = 1,

    /// <summary>
    /// Error de en la apertura de Caja
    /// </summary>
    ERROR_DE_MOVIMIENTO_CAJA_CERRADA = 2,

    /// <summary>
    /// Error de en la apertura de Caja
    /// </summary>
    ERROR_DE_APERTURA_CAJA_EQUIPO = 3,

    /// <summary>
    /// Error al realizar la transaccion Caja Cerrada
    /// </summary>
    ERROR_CAJA_CERRADA_NO_EFECTUAR_TRANSACCION = 4,

    /// <summary>
    /// Error al realizar insertar un cliente
    /// credito sin datos
    /// </summary>
    ERROR_CLIENTE_CREDITO_SIN_DATOS = 5,

    /// <summary>
    /// Mensaje de Error no existe un cierre Anterior
    /// </summary>
    ERROR_NO_EXISTE_CIERRE = 6,

    /// <summary>
    /// Mensaje de Error el valor de la compra
    /// supera el saldo del prepago
    /// </summary>
    ERROR_VALOR_PINPREPAGO_SUPERA_SALDO = 7,

    /// <summary>
    ///Mensaje de Error pin Prepago no encontrado
    /// </summary>
    ERROR_PINPREPAGO_NO_ENCONTRADO = 8,

    /// <summary>
    ///Mensaje de Error sin pin Prepago asociado.
    /// </summary>
    ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO = 9,

    /// <summary>
    ///Mensaje de Error el valor de cierre
    ///de la empresa es Cero o Negativo
    /// </summary>
    ERROR_VALOR_CIERRE_PUNTO_CERO = 10,

    /// <summary>
    ///Mensaje de Error no existe un cierre
    ///del punto ó agencia registrado.
    /// </summary>
    ERROR_NO_EXISTE_CIERRE_DEL_PUNTO = 11,

    /// <summary>
    ///Mensaje de Error Bolsa Seguridad ya
    ///registrada.
    /// </summary>
    ERROR_BOLSA_SEGURIDAD_YA_REGISTRADA = 12,

    /// <summary>
    ///Mensaje de Error dinero no reportado agencia.
    /// </summary>
    ERROR_DINERO_NO_REPORTADO_AGENCIA = 13,

    /// <summary>
    /// No existe  ningún registro de la transacción en caja, para poder modificar  el cambio de cliente
    /// </summary>
    ERROR_TRANSACCION_CAJA = 14,

    /// <summary>
    /// No existe ningún registro del detalle de la transacción en caja, para poder modificar  el cambio de cliente
    /// </summary>
    ERROR_TRANSACCION_DETALLE_CAJA = 15,

    /// <summary>
    /// No se inserto el registro del detalle de la caja
    /// </summary>
    ERROR_INSERCION_REGISTRO_DETALLE_CAJA = 16,

    /// <summary>
    /// Ya existe un registro de transacción del cliente crédito, no se puede completar la transacción.
    /// </summary>
    ERROR_TRANSACCION_CLIENTE_CREDITO = 17,

    /// <summary>
    /// No existe el concepto de caja para la anulacion de una guia
    /// </summary>
    ERROR_CONCEPTO_ANULACIONGUIA_NOEXISTE = 18,

    /// <summary>
    /// Mensaje de Información reportando un
    /// envio de dinero ya reportado a la Agencia.
    /// </summary>
    ERROR_DINERO_YA_REPORTADO_AGENCIA = 19,

    /// <summary>
    /// Sólo puede realizar esta operación como un usuario de RACOL o GESTION
    /// </summary>
    ERROR_OPERACION_SOLO_DESDE_RACOL = 20,

    /// <summary>
    /// Debe registrar un número de cuenta para realizar la operación sobre la caja de Bancos
    /// </summary>
    ERROR_NO_CUENTA_BANCO = 21,

    /// <summary>
    /// Mensaje de error indicando que el número de pin prepago no pertenece al centro de servicios.
    /// </summary>
    ERROR_PINPREPAGO_ERRADO = 22,

    /// <summary>
    /// No está autorizado para hacer esta operación, confirme con el admisnitrador del sistema que tenga permisos.
    /// </summary>
    ERROR_NO_AUTORIZADO = 23,

    /// <summary>
    /// Indica que un concepto no tiene dupla asociada
    /// </summary>
    ERROR_DUPLA_NO_EXISTE = 24,

    /// <summary>
    /// No existe movimiento asociado previamente a la transacción
    /// </summary>
    ERROR_MOVIMIENTO_NO_EXISTE = 25,

    /// <summary>
    /// No se encontró la caja para la apertura deseada, esto puede deberse a que no se ha hecho una apertura de caja anteriormente.
    /// </summary>
    ERROR_NO_CAJA_PARA_APERTURA = 26,

    /// <summary>
    /// No se encontró el concepto de caja con identificador {0} en la configuración del sistema
    /// </summary>
    ERROR_NO_CONCEPTO_CAJA = 27,

    /// <summary>
    /// No se encuentra configurado parámetro de caja. Nombre parámetro: {0}
    /// </summary>
    ERROR_PARAMETRO_CAJA_NO_ENCONTRADO = 28,

    /// <summary>
    /// Mensaje de error indicando que el número de la bolsa de
    /// seguridad no pertenece al centro de servicios.
    /// </summary>
    ERROR_BOLSA_SEGIRIDAD_ERRADA = 29,

    /// <summary>
    /// El usuario no tiene la Caja abierta para realizar la accion de cierre
    /// </summary>
    ERROR_CAJA_SIN_APERTURA_PARA_CERRAR = 30,

    /// <summary>
    /// Detalle de la transacción de caja no fue encontrada para el número {0}
    /// </summary>
    ERROR_CAJA_NUMERO_NO_ENCONTRADO = 31,

    /// <summary>
    /// El suministro con número {0} no aparece asignado a Usted
    /// </summary>
    ERROR_SUMINISTRO_NO_ASIGNADO_A_USTED = 32,

    /// <summary>
    /// El número del suministro {0} no es correcto
    /// </summary>
    ERROR_SUMINISTRO_NO_ES_CORRECTO = 33,

      /// <summary>
      ///No se puede hacer el descargue manual porque, el punto de servicio tiene reportes de dinero pendiente por descargar. Por favor haga primero el descargue automático.
      /// </summary>
      ERROR_REPORTE_DINERO_DE_PUNTO_PENDIENTE_POR_DESCARGAR =34
  }
}