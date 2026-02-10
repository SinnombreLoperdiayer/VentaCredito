
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using System.Collections.Generic;

namespace CO.Servidor.Dominio.Comun.Raps
{
    public interface IRAFachadaRaps
    {

        /// <summary>
        /// Obtiene el responsable asignado para responder los raps de las fallas causadas por determinado centro de servicio Modi
        /// </summary>
        /// <param name="identificacionResponsableFalla"></param>
        /// <returns></returns>
        RACargoEscalarDC ObtenerResponsableCentroServicioParaAsignarRaps(string identificacionResponsableFalla);


        /// <summary>
        /// Obtiene el horario de un empleado por su numero de identificacion
        /// </summary>
        /// <param name="documentoEmpleado"></param>
        /// <returns></returns>
        List<RAHorarioEmpleadoDC> ObtenerHorariosEmpleadoPorIdentificacion(string documentoEmpleado);
    }
}
