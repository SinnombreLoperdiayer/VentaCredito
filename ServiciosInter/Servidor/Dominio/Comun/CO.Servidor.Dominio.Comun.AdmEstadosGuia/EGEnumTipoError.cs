using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia
{
  public enum EGEnumTipoError : int
  {
    /// <summary>
    /// No existe posibles estados de transición, asociados al motivo seleccionado
    /// </summary>
    EX_ERROR_MOTIVO_ESTADO = 1,

    /// <summary>
    /// Error desconocido
    /// </summary>
    EX_ERROR_DESCONOCIDO = 0,
  }
}