
using CO.Servidor.Dominio.Comun.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using System.Collections.Generic;

namespace CO.Servidor.Raps
{
    public class RAFachadaRaps : IRAFachadaRaps
    {

        /// <summary>
        /// Instancia Singleton
        /// </summary>
        private static readonly RAFachadaRaps instancia = new RAFachadaRaps();


        /// <summary>
        /// Retorna una instancia de la fabrica de Dominio
        /// </summary>
        public static RAFachadaRaps Instancia
        {
            get { return RAFachadaRaps.instancia; }
        }


        /// <summary>
        /// Obtiene el responsable asignado para responder los raps de las fallas causadas por determinado centro de servicio Modi
        /// </summary>
        /// <param name="identificacionResponsableFalla"></param>
        /// <returns></returns>
        public RACargoEscalarDC ObtenerResponsableCentroServicioParaAsignarRaps(string identificacionResponsableFalla)
        {
            return RASolicitudes.Instancia.ObtenerResponsableCentroServicioParaAsignarRaps(identificacionResponsableFalla);
        }


        /// <summary>
        /// Obtiene el horario de un empleado por su numero de identificacion
        /// </summary>
        /// <param name="documentoEmpleado"></param>
        /// <returns></returns>
        public List<RAHorarioEmpleadoDC> ObtenerHorariosEmpleadoPorIdentificacion(string documentoEmpleado)
        {
            return RASolicitudes.Instancia.ObtenerHorariosEmpleadoPorIdentificacion(documentoEmpleado);
        }

    }
}
