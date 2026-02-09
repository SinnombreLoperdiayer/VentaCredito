using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Areas.Comun
{
  public class ARResolverMensajes
  {
    public static string CargarMensaje(AREnumTipoErrorAreas tipoErrorArea)
    {
      string mensajeError = String.Empty;

      switch (tipoErrorArea)
      {
        case AREnumTipoErrorAreas.ERROR_CASA_MATRIZ_NO_CONFIGURADA:
          mensajeError = ARAreaServidorMensajes.EX_001;
          break;

        case AREnumTipoErrorAreas.ERROR_MACROPROCESO_SIN_CODIGO:
          mensajeError = ARAreaServidorMensajes.EX_002;
          break;
      }

      return mensajeError;
    }
  }
}