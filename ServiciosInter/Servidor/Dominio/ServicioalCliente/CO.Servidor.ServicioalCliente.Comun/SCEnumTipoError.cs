using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ServicioalCliente.Comun
{
    public enum SCEnumTipoError : int
    {
        /// <summary>
        /// Error cuando se desea generar solicitud de reclamación para retiro de una guia reclame en oficina.
        /// </summary>
        EX_ERROR_GUIA_NO_RECLAME_EN_OFICINA = 0
    }
}
