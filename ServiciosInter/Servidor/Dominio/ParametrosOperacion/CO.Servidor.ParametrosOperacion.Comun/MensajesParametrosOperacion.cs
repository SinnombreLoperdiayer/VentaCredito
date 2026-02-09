using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ParametrosOperacion.Comun
{
  /// <summary>
  /// Clase para manejar los mensajes de los parametros de operacion
  /// </summary>
  public class MensajesParametrosOperacion
  {
    /// <summary>
    /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
    /// </summary>
    /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
    /// <returns>Mensaje de error</returns>
    public static string CargarMensaje(EnumTipoErrorParametrosOperacion tipoErrorCentroServicios)
    {
      string mensajeError;

      switch (tipoErrorCentroServicios)
      {
        #region Mensajes de Excepción

        case EnumTipoErrorParametrosOperacion.EX_REGISTRO_NO_ENCONTRADO_NOVASOFT:
          mensajeError = Mensajes.EX_001;
          break;

        case EnumTipoErrorParametrosOperacion.EX_CONTRATO_NO_ESTA_VIGENTE:
          mensajeError = Mensajes.EX_002;
          break;

        case EnumTipoErrorParametrosOperacion.EX_NO_CUMPLE_TIPO_VINCULACION:
          mensajeError = Mensajes.EX_003;
          break;

        case EnumTipoErrorParametrosOperacion.EX_NO_EXISTE_INFO_SOAT_TECNO_MECANICA_MENSAJERO:
          mensajeError = Mensajes.EX_004;
          break;

        case EnumTipoErrorParametrosOperacion.EX_YA_EXISTE_VEHICULO:
          mensajeError = Mensajes.EX_005;
          break;

        #endregion Mensajes de Excepción

        #region Mensajes de Informacion

        case EnumTipoErrorParametrosOperacion.IN_ESTADO_SUSPENDIDO:
          mensajeError = Mensajes.IN_001;
          break;

        case EnumTipoErrorParametrosOperacion.IN_ACTIVO:
          mensajeError = Mensajes.IN_002;
          break;

        case EnumTipoErrorParametrosOperacion.IN_INACTIVO:
          mensajeError = Mensajes.IN_003;
          break;

        #endregion Mensajes de Informacion

        default:
          mensajeError = String.Format(Mensajes.EX_000, tipoErrorCentroServicios.ToString());
          break;
      }

      return mensajeError;
    }
  }
}