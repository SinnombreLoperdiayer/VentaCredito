using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia
{
  public class EGMensajesEstadoGuia
  {
    /// <summary>
    /// Carga un mensaje de error de logistica inversa desde el recurso del lenguaje
    /// </summary>
    /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
    /// <returns>Mensaje de error</returns>
    public static string CargarMensaje(EGEnumTipoError tipoErrorEstadosGuia)
    {
      string mensajeError;

      switch (tipoErrorEstadosGuia)
      {
        case EGEnumTipoError.EX_ERROR_MOTIVO_ESTADO:
          mensajeError = EGMensajes.EX_001;
          break;

        default:
          mensajeError = String.Format(EGMensajes.EX_000, tipoErrorEstadosGuia.ToString());
          break;
      }

      return mensajeError;
    }
  }
}