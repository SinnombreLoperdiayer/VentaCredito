using CO.Servidor.Servicios.ContratoDatos.Raps.Citas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiCitasRaps : ApiDominioBase
    {
        #region Singleton
        private static readonly ApiCitasRaps instancia = (ApiCitasRaps)FabricaInterceptorApi.GetProxy(new ApiCitasRaps(), COConstantesModulos.MODULO_RAPS);
        #endregion

        #region Constructor
        public static ApiCitasRaps Instancia
        {
            get { return ApiCitasRaps.instancia; }
        }

        private ApiCitasRaps()
        { }
        #endregion

        #region Insertar
        /// <summary>
        /// Inserta la o las citas de acuerdo al tiempo de repeticion
        /// </summary>
        /// <param name="cita"></param>
        public void InsertarCita(RACitaDC cita)
        {
            FabricaServicios.ServicioCitasRaps.InsertarCita(cita);
            foreach (var integrante in cita.Integrantes)
            {
                string mensaje = string.Concat("Se le ha asignado una nueva cita ", cita.Titulo, " para el ", cita.FechaInicioCita.ToShortDateString());
                Notificar(mensaje, integrante.DocumentoIntegrante);
            }
        }

        /// <summary>
        /// Inserta la asistencia de una cita
        /// </summary>
        /// <param name="asistencia"></param>
        public void InsertarAsistencia(List<RAAsistenciaCita> asistencia)
        {
            FabricaServicios.ServicioCitasRaps.InsertarAsistencia(asistencia);
        }

        /// <summary>
        /// Inserta la gestion de una cita
        /// </summary>
        /// <param name="gestion"></param>
        public void InsertarGestionCita(RACitaDC gestion)
        {
            FabricaServicios.ServicioCitasRaps.InsertarGestionCita(gestion);
        }

        /// <summary>
        /// Inserta la gestios de los compromisos
        /// </summary>
        /// <param name="compromiso"></param>
        public void InsertarGestionCompromisos(List<RACompromisoDC> compromisos)
        {
            FabricaServicios.ServicioCitasRaps.InsertarGestionCompromisos(compromisos);
        }
        #endregion

        #region Consultar
        /// <summary>
        /// obtiene todas las citas programadas para cierto rango de fechas y para cierto usuario
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerCitasPorFechaEIntegrante(DateTime fechaInicio, DateTime fechaFin, long documentoIntegrante)
        {
            return FabricaServicios.ServicioCitasRaps.ObtenerCitasPorFechaEIntegrante(fechaInicio, fechaFin, documentoIntegrante);
        }

        /// <summary>
        /// Obtiene el detalle de una cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public RACitaDC ObtenerDetalleCita(long idCita)
        {
            return FabricaServicios.ServicioCitasRaps.ObtenerDetalleCita(idCita);
        }

        /// <summary>
        /// Obtiene los consolidados segun el tipod e raps para mostrar en el calendario
        /// </summary>
        /// <param name="agrupamiento"></param>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerConsolidadoPorEstadoDeSolicitudRaps(RAAgrupamientoRapsDC agrupamiento)
        {
            return FabricaServicios.ServicioCitasRaps.ObtenerConsolidadoPorEstadoDeSolicitudRaps(agrupamiento);
        }

        /// <summary>
        /// Obtiene los periodo en que se daran las notificaciones de una cita
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ObtenerPeriodoNotificacion()
        {
            return FabricaServicios.ServicioCitasRaps.ObtenerPeriodoNotificacion();
        }

        /// <summary>
        /// Obtiene informacion de una lista de empleados
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosNovasoftPorId(List<RAIntegranteCitaDC> integrantes)
        {
            return FabricaServicios.ServicioCitasRaps.ObtenerEmpleadosNovasoftPorId(integrantes);
        }

        /// <summary>
        /// Obtiene los tipos de integrantes
        /// </summary>
        /// <returns></returns>
        public List<RAIntegranteCitaDC> ObtenertiposIntegrantes()
        {
            return FabricaServicios.ServicioCitasRaps.ObtenertiposIntegrantes();
        }

        /// <summary>
        /// Obtiene los compromisos por cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RACompromisoDC> ObtenerCompromisosPorCita(long idCita)
        {
            return FabricaServicios.ServicioCitasRaps.ObtenerCompromisosPorCita(idCita);
        }

        #endregion

        #region Modficar

        /// <summary>
        /// Modificar cita especifica
        /// </summary>
        /// <param name="cita"></param>
        public void ModificarCita(RACitaDC cita)
        {
            FabricaServicios.ServicioCitasRaps.ModificarCita(cita);
        }

        /// <summary>
        /// Modifica todas las citas de acuerdo con la nueva informacion suministrada
        /// </summary>
        public void ModificarCitas(RACitaDC cita)
        {
            FabricaServicios.ServicioCitasRaps.ModificarCitas(cita);
        }


        #endregion

        #region Eliminar
        /// <summary>
        /// Elimina las citas futuras
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        public void EliminarCitasFuturas(RAInfoCitasDC cita)
        {
            RACitaDC detalleCita = new RACitaDC();
            detalleCita = ObtenerDetalleCita(cita.IdCita);
            foreach (var integrante in detalleCita.Integrantes)
            {
                string mensaje = string.Concat("Se Ha Eliminado la cita " + detalleCita.Titulo);
                Notificar(mensaje, integrante.DocumentoIntegrante);
            }
            FabricaServicios.ServicioCitasRaps.EliminarCitasFuturas(cita);
        }

        /// <summary>
        ///Elimina todas las citas 
        /// </summary>
        /// <param name="cita"></param>
        public void EliminarCitas(RAInfoCitasDC cita)
        {
            RACitaDC detalleCita = new RACitaDC();
            detalleCita = ObtenerDetalleCita(cita.IdCita);
            foreach (var integrante in detalleCita.Integrantes)
            {
                string mensaje = string.Concat("El Moderador ha eliminado algunas de las citas programadas");
                Notificar(mensaje, integrante.DocumentoIntegrante);
            }
            FabricaServicios.ServicioCitasRaps.EliminarCitas(cita.IdParametrizacionCita);
        }

        /// <summary>
        /// Elimina una cita especifica
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        internal void EliminarCita(RAInfoCitasDC cita)
        {
            RACitaDC detalleCita = new RACitaDC();
            detalleCita = ObtenerDetalleCita(cita.IdCita);
            foreach (var integrante in detalleCita.Integrantes)
            {
                string mensaje = string.Concat("El Moderador ha eliminado algunas de las citas programadas");
                Notificar(mensaje, integrante.DocumentoIntegrante);
            }
            FabricaServicios.ServicioCitasRaps.EliminarCita(cita);
        }
        #endregion

        #region validaciones
        /// <summary>
        /// Valida si un determinado usuario es moderador
        /// </summary>
        /// <returns></returns>
        /// 
        public bool ValidarModerador(long idCita, long idEmpleado)
        {
            return FabricaServicios.ServicioCitasRaps.ValidarModerador(idCita, idEmpleado);
        }
        #endregion

        #region Notificaciones
        public void Notificar(string mensaje, long idUsusario)
        {
            ApiConfiguracionRaps.Instancia.NotificarSolicitudUsuario(new ParametrosSignalR()
            {
                Documento = idUsusario,
                Mensaje = mensaje,
                InsertarNotificacion = false
            });
        }
        #endregion
    }
}