using System;

namespace CO.Servidor.Produccion.Comun
{
  public class PRMensajesProduccion
  {
    /// <summary>
    /// Carga un mensaje de error de Produccion desde el recurso del lenguaje
    /// </summary>
    /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
    /// <returns>Mensaje de error</returns>
    public static string CargarMensaje(PREnumTipoError tipoError)
    {
      string mensajeError;

      switch (tipoError)
      {
        case PREnumTipoError.EX_NO_CONFIGURADO_PARAMETRO:
          mensajeError = PRMensajes.EX_001;
          break;

        case PREnumTipoError.IN_EJECUTADO:
          mensajeError = PRMensajes.IN_001;
          break;

        case PREnumTipoError.IN_NO_EJECUTADO:
          mensajeError = PRMensajes.IN_002;
          break;

        default:
          mensajeError = String.Format(PRMensajes.EX_000, tipoError.ToString());
          break;
      }

      return mensajeError;
    }
  }
}