using CO.Servidor.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Citas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CO.Servidor.Servicios.Implementacion.Raps
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RACitasRapsSvc
    {

        #region Insertar

        /// <summary>
        /// Inserta la o las citas de acuerdo al tiempo de repeticion
        /// </summary>
        /// <param name="cita"></param>
        /// 
        public void InsertarCita(RACitaDC cita)
        {
            RACitas.Instancia.InsertarCita(cita);
        }

        /// <summary>
        /// Inserta la asistencia de una cita
        /// </summary>
        /// <param name="asistencia"></param>
        public void InsertarAsistencia(List<RAAsistenciaCita> asistencia)
        {
            RACitas.Instancia.InsertarAsistencia(asistencia);
        }

        /// <summary>
        /// Inserta la gestion de una cita
        /// </summary>
        /// <param name="gestion"></param>
        public void InsertarGestionCita(RACitaDC gestion)
        {
            RACitas.Instancia.InsertarGestionCita(gestion);
        }

        /// <summary>
        /// Inserta la gestios de los compromisos
        /// </summary>
        /// <param name="compromiso"></param>
        public void InsertarGestionCompromisos(List<RACompromisoDC> compromisos)
        {
            RACitas.Instancia.InsertarGestionCompromisos(compromisos);
        }

        #endregion

        #region Consultar
        /// <summary>
        /// obtiene todas las citas programadas para cierto rango de fechas y para cierto usuario
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerCitasPorFechaEIntegrante(DateTime fechaInicio, DateTime fechaFin, long documentoIntegrante)
        {
            return RACitas.Instancia.ObtenerCitasPorFechaEIntegrante(fechaInicio, fechaFin, documentoIntegrante);
        }

        /// <summary>
        /// Obtiene el detalle de una cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public RACitaDC ObtenerDetalleCita(long idCita)
        {
            return RACitas.Instancia.ObtenerDetalleCita(idCita);
        }

        /// <summary>
        /// Obtiene los consolidados segun el tipod e raps para mostrar en el calendario
        /// </summary>
        /// <param name="agrupamiento"></param>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerConsolidadoPorEstadoDeSolicitudRaps(RAAgrupamientoRapsDC agrupamiento)
        {
            return RACitas.Instancia.ObtenerConsolidadoPorEstadoDeSolicitudRaps(agrupamiento);
        }

        /// <summary>
        /// Obtiene los periodo en que se daran las notificaciones de una cita
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ObtenerPeriodoNotificacion()
        {
            return RACitas.Instancia.ObtenerPeriodoNotificacion();
        }


        /// <summary>
        /// Obtiene informacion de una lista de empleados
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosNovasoftPorId(List<RAIntegranteCitaDC> integrantes)
        {
            return RACitas.Instancia.ObtenerEmpleadosNovasoftPorId(integrantes);
        }

        /// <summary>
        /// Obtiene los tipos de integrantes
        /// </summary>
        /// <returns></returns>
        public List<RAIntegranteCitaDC> ObtenertiposIntegrantes()
        {
            return RACitas.Instancia.ObtenertiposIntegrantes();
        }
        #endregion

        #region Modficar

        /// <summary>
        /// Modificar cita especifica
        /// </summary>
        /// <param name="cita"></param>
        public void ModificarCita(RACitaDC cita)
        {
            RACitas.Instancia.ModificarCita(cita);
        }

        /// <summary>
        /// Modifica todas las citas de acuerdo con la nueva informacion suministrada
        /// </summary>
        public void ModificarCitas(RACitaDC cita)
        {
            RACitas.Instancia.ModificarCitas(cita);
        }

        /// <summary>
        /// Obtiene los compromisos por cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RACompromisoDC> ObtenerCompromisosPorCita(long idCita)
        {
            return RACitas.Instancia.ObtenerCompromisosPorCita(idCita);
        }


        #endregion

        #region Eliminar

        /// <summary>
        /// Elimina las citas futuras
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        public void EliminarCitasFuturas(RAInfoCitasDC cita)
        {
            RACitas.Instancia.EliminarCitasFuturas(cita);
        }

        /// <summary>
        ///Elimina todas las citas 
        /// </summary>
        /// <param name="cita"></param>
        public void EliminarCitas(long idParametrizacionCita)
        {
            RACitas.Instancia.EliminarCitas(idParametrizacionCita);
        }

        /// <summary>
        /// Elimina una cita especifica
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        public void EliminarCita(RAInfoCitasDC cita)
        {
            RACitas.Instancia.EliminarCita(cita);
        }
        #endregion

        #region Validaciones
        /// <summary>
        /// Valida si un determinado usuario es moderador
        /// </summary>
        /// <returns></returns>
        /// 
        public bool ValidarModerador(long idCita, long idEmpleado)
        {
            return RACitas.Instancia.ValidarModerador(idCita, idEmpleado);
        }
        #endregion

    }
}
