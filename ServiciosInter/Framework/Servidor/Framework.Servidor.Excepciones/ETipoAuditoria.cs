using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Excepciones
{
    /// <summary>
    /// Enumeracion que indica el tipo de auditoria para el trace
    /// </summary>
    public enum ETipoAuditoria
    {
        /// <summary>
        /// Indica si el mensaje de auditoria es de tipo error
        /// </summary>
        Error,
        /// <summary>
        /// Indica si el mensaje de auditoria es informativo
        /// </summary>
        Info
    }
}
