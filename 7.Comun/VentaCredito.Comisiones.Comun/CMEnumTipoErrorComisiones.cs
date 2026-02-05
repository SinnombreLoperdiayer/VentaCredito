using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Comisiones.Comun
{
    /// <summary>
    /// Enumerador de los Errores de
    /// Comisiones
    /// </summary>
    public enum CMEnumTipoErrorComisiones : int
    {
        /// <summary>
        /// Error no se encuentra Configurado.
        /// </summary>
        EL_ERROR_NO_SE_ENCUENTRA_CONFIGURADO = 0,

        /// <summary>
        /// Error de en la apertura de Caja
        /// </summary>
        EX_ERROR_PORCENTAJE_COMISION_CENTROSERVICIO = 1,

        /// <summary>
        /// Error en el porcentaje de la comisión del responsable
        /// </summary>
        EX_ERROR_PORCENTAJE_COMISION_RESPONSABLE = 2,
        /// <summary>
        /// Error del punto no tiene una comision asignada
        /// </summary>
        EX_ERROR_NO_TIENE_COMISION_ASIGNADA = 3,

        /// <summary>
        /// Error al recibir el parametro en cero de la base de la comisión
        /// </summary>
        EX_ERROR_VALOR_BASE_COMISION_ES_CERO = 4,

        /// <summary>
        /// Error al recibir el parametro en cero de la base de la comisión
        /// </summary>
        EX_ERROR_VALOR_COMISION_RESPONSABLE = 5,

        /// <summary>
        /// La comisión no está configurada para el punto de servicio
        /// </summary>
        EX_COMISION_NO_CONFIGURADA = 6
    }
}
