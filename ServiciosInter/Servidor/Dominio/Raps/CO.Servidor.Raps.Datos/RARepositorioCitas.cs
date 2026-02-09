using CO.Servidor.Servicios.ContratoDatos.Raps.Citas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CO.Servidor.Raps.Datos
{
    public class RARepositorioCitas : ControllerBase
    {
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;

        #region Singleton
        private static readonly RARepositorioCitas instancia = new RARepositorioCitas();

        public static RARepositorioCitas Instancia
        {
            get { return RARepositorioCitas.instancia; }
        }
        #endregion

        #region Insertar

        /// <summary>
        /// Inserta la cita(hija)
        /// </summary>
        /// <param name="cita"></param>
        /// 
        public long InsertarCitaHija(RACitaDC cita)
        {
            long idCita = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", cita.FechaInicioCita);
                cmd.Parameters.AddWithValue("@FechaFin", cita.FechaFinCita);
                cmd.Parameters.AddWithValue("@IdEstado", cita.IdEstado);
                cmd.Parameters.AddWithValue("@IdParamCita", cita.IdParametrizacion);
                cmd.Parameters.AddWithValue("@DescripcionCita", cita.Descripcion);
                cmd.Parameters.AddWithValue("@Titulo", cita.Titulo);
                cmd.Parameters.AddWithValue("@Lugar", cita.LugarCita);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                idCita = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }
            return idCita;
        }

        /// <summary>
        /// Inserta el periodo de repeticion para la cita
        /// </summary>
        /// <param name="periodo"></param>
        public long InsertarPeriodoRepeticion(RAPeriodoRepeticionDC periodo)
        {
            long idPeriodo = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarPeriodoRepeticion_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoPeriodo", periodo.IdTipoPeriodo);
                cmd.Parameters.AddWithValue("@Intervalo", periodo.Intervalo);
                cmd.Parameters.AddWithValue("@Lunes", periodo.Lunes != null ? periodo.Lunes.Value.AddHours(-5) : periodo.Lunes);
                cmd.Parameters.AddWithValue("@Martes", periodo.Martes != null ? periodo.Martes.Value.AddHours(-5) : periodo.Martes);
                cmd.Parameters.AddWithValue("@Miercoles", periodo.Miercoles != null ? periodo.Miercoles.Value.AddHours(-5) : periodo.Miercoles);
                cmd.Parameters.AddWithValue("@Jueves", periodo.Jueves != null ? periodo.Jueves.Value.AddHours(-5) : periodo.Jueves);
                cmd.Parameters.AddWithValue("@Viernes", periodo.Viernes != null ? periodo.Viernes.Value.AddHours(-5) : periodo.Viernes);
                cmd.Parameters.AddWithValue("@Sabado", periodo.Sabado != null ? periodo.Sabado.Value.AddHours(-5) : periodo.Sabado);
                cmd.Parameters.AddWithValue("@DuracionHoras", periodo.DuracionHoras);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                idPeriodo = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }
            return idPeriodo;
        }

        /// <summary>
        /// Inserta la parametrizaciond e la cita (cita padre)
        /// </summary>
        /// <param name="parametrizacion"></param>
        /// <returns></returns>
        public long InsertarParametrizacionCitas(RAParametrizacionCitaDC parametrizacion)
        {
            long idParametrizacion = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarParametrizacionCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", parametrizacion.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", parametrizacion.FechaFin);
                cmd.Parameters.AddWithValue("@IdperiodoRepeticion", parametrizacion.IdPeriodoRepeticion);
                cmd.Parameters.AddWithValue("@IdEstado", parametrizacion.IdEstado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                idParametrizacion = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }
            return idParametrizacion;
        }

        /// <summary>
        /// Inserta los integrantes de la cita
        /// </summary>
        /// <param name="integrante"></param>
        public void InsertarIntegrantePorCita(RAIntegranteCitaDC integrante)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarIntegrante_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentoIntegrante", integrante.DocumentoIntegrante);
                cmd.Parameters.AddWithValue("@IdCita", integrante.IdCita);
                cmd.Parameters.AddWithValue("@IdTipoIntegrante", integrante.IdTipoIntegrante);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Inserta el periodo en el que se va a notificar a los asistentes de las citas
        /// </summary>
        /// <param name="Notificacion"></param>
        public void InsertarNotificacionCita(RANotificacionCitaDC Notificacion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarNotificacionCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", Notificacion.IdCita);
                cmd.Parameters.AddWithValue("@IdPeriodoNotificacion", Notificacion.IdPeriodoNotificacion);
                cmd.Parameters.AddWithValue("@TiempoRecordatorio", Notificacion.TiempoRecordatorio);
                cmd.Parameters.AddWithValue("@HoraNotificacion", Notificacion.HoraNotificacion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Inserta la gestion de una cita
        /// </summary>
        /// <param name="gestion"></param>
        public void InsertarGestionCita(RACitaDC gestion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paActualizarConclusionCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstado", 2);
                cmd.Parameters.AddWithValue("@IdCita", gestion.IdCita);
                cmd.Parameters.AddWithValue("@OrdenDia", gestion.OrdenDia);
                cmd.Parameters.AddWithValue("@Desarrollo", gestion.Desarrollo);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Inserta el la tabla la relacion de los raps creados a partir de una cita
        /// </summary>
        public void InsertarRapsPorCita(long IdCita, long idSolicitudRaps)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarSolicitudesRapsCitas_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitudRaps", idSolicitudRaps);
                cmd.Parameters.AddWithValue("@IdCita", IdCita);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Inserta la asistencia de una cita
        /// </summary>
        /// <param name="asistencia"></param>
        public void InsertarAsistencia(RAAsistenciaCita asistencia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarAsistenciaCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentoAsistente", asistencia.DocumentoAsistente);
                cmd.Parameters.AddWithValue("@IdCita", asistencia.IdCita);
                cmd.Parameters.AddWithValue("@Observacion", asistencia.Observacion);
                cmd.Parameters.AddWithValue("@Asistio", asistencia.Asistio);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Crear adjunto
        /// </summary>
        /// <param name="adjunto"></param>
        /// <returns></returns>
        public void InsertarAdjuntoCita(RAAdjuntoDC adjunto)
        {
            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarAdjuntoCita_CIT", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", adjunto.IdCita);
                cmd.Parameters.AddWithValue("@Tamaño", adjunto.Tamaño);
                cmd.Parameters.AddWithValue("@Extension", adjunto.Extension);
                cmd.Parameters.AddWithValue("@NombreAdjunto", adjunto.NombreArchivo);
                cmd.Parameters.AddWithValue("@UbicacionNombre", adjunto.UbicacionNombre);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
            }


        }

        /// <summary>
        /// Inserta un nuevo compromiso
        /// </summary>
        /// <param name="c"></param>
        public void InsertarCompromiso(RACompromisoDC compromiso)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarCompromisos_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstado", compromiso.Estado);
                cmd.Parameters.AddWithValue("@Descripcion", compromiso.Descripcion);
                cmd.Parameters.AddWithValue("@IdResponsable", compromiso.Idresponsable);
                cmd.Parameters.AddWithValue("@IdCita", compromiso.IdCita);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        #endregion

        #region Consultar

        /// <summary>
        /// obtiene todas las citas programadas para cierto rango de fechas y para cierto usuario
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerCitasPorFechaEIntegrante(DateTime fechaInicio, DateTime fechaFin, long documentoIntegrante)
        {
            List<RAFormatoCalendarioDC> lstCitas = new List<RAFormatoCalendarioDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCitasPorFecha_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@DocumentoIntegrante", documentoIntegrante);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAFormatoCalendarioDC cita = new RAFormatoCalendarioDC
                    {
                        Start = Convert.ToDateTime(reader["CIT_FechaInicio"]),
                        End = Convert.ToDateTime(reader["CIT_FechaFin"]),
                        Title = reader["CIT_Titulo"].ToString(),
                        Id = Convert.ToInt64(reader["CIT_IdCita"]),
                        Estado = Convert.ToInt32(reader["CIT_IdEstado"])
                    };
                    lstCitas.Add(cita);
                }
                conn.Close();
            }
            return lstCitas;
        }

        /// <summary>
        /// obtioene el consolidado de raps vencidos
        /// </summary>
        /// <param name="agrupamiento"></param>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerAgrupamientoRapsVencidos(RAAgrupamientoRapsDC agrupamiento)
        {
            List<RAFormatoCalendarioDC> lstRaps = new List<RAFormatoCalendarioDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgrupamientoRapsVencidos_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", agrupamiento.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", agrupamiento.FechaFin);
                cmd.Parameters.AddWithValue("@DocumentoResponsable", agrupamiento.IdResponsable);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAFormatoCalendarioDC raps = new RAFormatoCalendarioDC
                    {
                        Title = reader["Descripcion"].ToString() + " (" + reader["Conteo"].ToString() + ")",
                        Start = Convert.ToDateTime(reader["fecha"]),
                        End = Convert.ToDateTime(reader["fecha"]),
                        Id = -3
                    };
                    lstRaps.Add(raps);
                }
                conn.Close();
            }
            return lstRaps;
        }

        /// <summary>
        /// obtioene el consolidado de raps Resueltos
        /// </summary>
        /// <param name="agrupamiento"></param>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerAgrupamientoRapsResueltos(RAAgrupamientoRapsDC agrupamiento)
        {
            List<RAFormatoCalendarioDC> lstRaps = new List<RAFormatoCalendarioDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgrupamientoRapsResueltos_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", agrupamiento.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", agrupamiento.FechaFin);
                cmd.Parameters.AddWithValue("@DocumentoResponsable", agrupamiento.IdResponsable);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAFormatoCalendarioDC raps = new RAFormatoCalendarioDC
                    {
                        Title = reader["Descripcion"].ToString() + " (" + reader["Conteo"].ToString() + ")",
                        Start = Convert.ToDateTime(reader["fecha"]),
                        End = Convert.ToDateTime(reader["fecha"]),
                        Id = -1
                    };
                    lstRaps.Add(raps);
                }
                conn.Close();
            }
            return lstRaps;
        }


        /// <summary>
        /// Obtiene los tipos de integrantes
        /// </summary>
        /// <returns></returns>
        public List<RAIntegranteCitaDC> ObtenertiposIntegrantes()
        {
            List<RAIntegranteCitaDC> lstTipos = new List<RAIntegranteCitaDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposIntegrantes_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAIntegranteCitaDC tipo = new RAIntegranteCitaDC
                    {
                        IdTipoIntegrante = (RAEnumTipoIntegrante)reader["TIN_IdTipoIntegrante"],
                        DescripcionTipo = reader["TIN_Descripcion"].ToString(),
                        EsModerador = Convert.ToBoolean(reader["TIN_Moderador"])
                    };
                    lstTipos.Add(tipo);
                }
                conn.Close();
            }
            return lstTipos;
        }

        /// <summary>
        /// Obtiene la asistencia por cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RAAsistenciaCita> ObtenerAsistenciaPorCita(long idCita)
        {
            List<RAAsistenciaCita> lstAsistencia = new List<RAAsistenciaCita>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAsistenciaPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAAsistenciaCita asistencia = new RAAsistenciaCita
                    {
                        DocumentoAsistente = Convert.ToInt64(reader["ASC_DocumentoAsistente"]),
                        Observacion = reader["ASC_Observacion"].ToString(),
                        Asistio = Convert.ToBoolean(reader["ASC_Asistio"]),
                        Nombre = reader["NombreEmpleado"].ToString()
                    };
                    lstAsistencia.Add(asistencia);
                }
                conn.Close();
            }
            return lstAsistencia;
        }

        /// <summary>
        /// obtioene el consolidado de raps pendientes
        /// </summary>
        /// <param name="agrupamiento"></param>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerAgrupamientoRapsPendientes(RAAgrupamientoRapsDC agrupamiento)
        {
            List<RAFormatoCalendarioDC> lstRaps = new List<RAFormatoCalendarioDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgrupamientoRapsPendientes_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", agrupamiento.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", agrupamiento.FechaFin);
                cmd.Parameters.AddWithValue("@DocumentoResponsable", agrupamiento.IdResponsable);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAFormatoCalendarioDC raps = new RAFormatoCalendarioDC
                    {
                        Title = reader["Descripcion"].ToString() + " (" + reader["Conteo"].ToString() + ")",
                        Start = Convert.ToDateTime(reader["fecha"]),
                        End = Convert.ToDateTime(reader["fecha"]),
                        Id = -2
                    };
                    lstRaps.Add(raps);
                }
                conn.Close();
            }
            return lstRaps;
        }

        /// <summary>
        /// Obtiene 
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RANotificacionCitaDC> ObtenerNotificacionPorCita(long idCita)
        {
            List<RANotificacionCitaDC> lstNotificaciones = new List<RANotificacionCitaDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNotificacionesPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RANotificacionCitaDC notificacion = new RANotificacionCitaDC
                    {
                        IdNotificacion = Convert.ToInt32(reader["NCI_IdNotificacionCita"]),
                        IdPeriodoNotificacion = Convert.ToInt32(reader["NCI_IdPeriodoNotificacion"]),
                        TiempoRecordatorio = Convert.ToInt32(reader["NCI_TiempoRecordatorio"]),
                        HoraNotificacion = reader["NCI_HoraNotificacion"] is DBNull ? new DateTime() : Convert.ToDateTime(reader["NCI_HoraNotificacion"])
                    };
                    lstNotificaciones.Add(notificacion);
                }
                conn.Close();
            }
            return lstNotificaciones;
        }

        /// <summary>
        /// Obtiene los compromisos por cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RACompromisoDC> ObtenerCompromisosPorCita(long idCita)
        {
            List<RACompromisoDC> lstCompromisos = new List<RACompromisoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCompromisosPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RACompromisoDC compromiso = new RACompromisoDC
                    {
                        IdCompromiso = Convert.ToInt64(reader["CPR_IdCompromiso"]),
                        Estado = Convert.ToBoolean(reader["CPR_IdEstado"]),
                        Descripcion = reader["CPR_Descripcion"].ToString(),
                        Idresponsable = Convert.ToInt64(reader["CPR_IdResponsable"]),
                        IdCita = Convert.ToInt64(reader["CPR_IdCita"]),
                        NombreResponsable = reader["NombreEmpleado"].ToString()
                    };
                    lstCompromisos.Add(compromiso);
                }
                conn.Close();
            }
            return lstCompromisos;
        }

        /// <summary>
        /// Obtiene las citas por parametrizacion
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <param name="fechaInicioCita"></param>
        public List<RAInfoCitasDC> ObtenerCitasPorParametrizacion(long idParametrizacion, DateTime? fechaInicioCambio)
        {

            List<RAInfoCitasDC> lstinfoCitas = new List<RAInfoCitasDC>();

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCitaPorParametrizacion_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParamCita", idParametrizacion);
                cmd.Parameters.AddWithValue("@FechaDesde", fechaInicioCambio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAInfoCitasDC info = new RAInfoCitasDC
                    {
                        IdCita = Convert.ToInt64(reader["CIT_IdCita"]),
                        FechaInicio = Convert.ToDateTime(reader["CIT_FechaInicio"])
                    };
                    lstinfoCitas.Add(info);
                }
                conn.Close();
            }
            return lstinfoCitas;
        }

        /// <summary>
        /// Obtiene los integrantes de una cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RAIntegranteCitaDC> ObtenerIntegrantesPorCita(long idCita)
        {
            List<RAIntegranteCitaDC> lstintegrantes = new List<RAIntegranteCitaDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerIntegrantesPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAIntegranteCitaDC integrante = new RAIntegranteCitaDC
                    {
                        DocumentoIntegrante = Convert.ToInt64(reader["INC_DocumentoIntegrante"]),
                        IdTipoIntegrante = (RAEnumTipoIntegrante)reader["INC_IdTipoIntegrante"],
                        IdCita = idCita,
                        TipoIntegrante = reader["TIN_Descripcion"].ToString()
                    };
                    lstintegrantes.Add(integrante);
                }
                conn.Close();
            }
            return lstintegrantes;
        }

        /// <summary>
        /// OBTIENE EL DETALLE DE UNA CITA
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public RAPeriodoRepeticionDC ObtenerperiodoRepeticionCita(long idCita)
        {
            RAPeriodoRepeticionDC periodoRepeticion = new RAPeriodoRepeticionDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPeriodoRepeticion_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    periodoRepeticion.IdTipoPeriodo = (RAEnumTipoPeriodo)reader["PER_IdTipoPeriodo"];
                    periodoRepeticion.Intervalo = reader["PER_Intervalo"] is DBNull ? 0 : Convert.ToInt32(reader["PER_Intervalo"]);
                    periodoRepeticion.Lunes = reader["PER_Lunes"] is DBNull ? new DateTime?() : Convert.ToDateTime(reader["PER_Lunes"]);
                    periodoRepeticion.Martes = reader["PER_Martes"] is DBNull ? new DateTime?() : Convert.ToDateTime(reader["PER_Martes"]);
                    periodoRepeticion.Miercoles = reader["PER_Miercoles"] is DBNull ? new DateTime?() : Convert.ToDateTime(reader["PER_Miercoles"]);
                    periodoRepeticion.Jueves = reader["PER_Jueves"] is DBNull ? new DateTime?() : Convert.ToDateTime(reader["PER_Jueves"]);
                    periodoRepeticion.Viernes = reader["PER_Viernes"] is DBNull ? new DateTime?() : Convert.ToDateTime(reader["PER_Viernes"]);
                    periodoRepeticion.Sabado = reader["PER_Sabado"] is DBNull ? new DateTime?() : Convert.ToDateTime(reader["PER_Sabado"]);
                    periodoRepeticion.DuracionHoras = Convert.ToInt32(reader["PER_DuracionHoras"]);
                }
                conn.Close();
            }
            return periodoRepeticion;
        }

        /// <summary>
        /// Obtiene los periodo en que se daran las notificaciones de una cita
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ObtenerPeriodoNotificacion()
        {
            List<RATipoPeriodoDC> lstPeriodos = new List<RATipoPeriodoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPeriodoNotificacion_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RATipoPeriodoDC periodo = new RATipoPeriodoDC
                    {
                        IdTipoPeriodo = Convert.ToInt32(reader["PEN_IdPeriodoNotificacion"]),
                        Descripcion = reader["PEN_DescripcionPeriodo"].ToString()
                    };
                    lstPeriodos.Add(periodo);
                }
                conn.Close();
            }
            return lstPeriodos;
        }

        /// <summary>
        /// obitne la informaciond e la cita junto con la parametrizacion
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public RACitaDC ObtenerDetalleCita(long idCita)
        {
            RACitaDC cita = new RACitaDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cita.FechaInicioCita = Convert.ToDateTime(reader["CIT_FechaInicio"]);///fechas javascript
                    cita.Titulo = reader["CIT_Titulo"].ToString();
                    cita.FechaFinCita = Convert.ToDateTime(reader["CIT_FechaFin"]);///fechas javascript
                    cita.IdEstado = (RAEnumEstadoCita)reader["CIT_IdEstado"];
                    cita.IdParametrizacion = Convert.ToInt64(reader["CIT_IdParamCita"]);
                    cita.Descripcion = reader["CIT_DescripcionCita"].ToString();
                    cita.LugarCita = reader["CIT_LugarCita"].ToString();
                    cita.OrdenDia = reader["CIT_OrdenDia"].ToString();
                    cita.Desarrollo = reader["CIT_Desarrollo"].ToString();
                    cita.ParametrizacionCita = new RAParametrizacionCitaDC
                    {
                        FechaInicio = Convert.ToDateTime(reader["PCI_FechaInicio"]),///fechas javascript,
                        FechaFin = Convert.ToDateTime(reader["PCI_FechaFin"]),///fechas javascript,
                        IdPeriodoRepeticion = Convert.ToInt64(reader["PCI_IdperiodoRepeticion"]),
                        IdEstado = (RAEnumEstadoCita)reader["PCI_IdEstado"],

                    };
                }
                conn.Close();
            }
            return cita;
        }

        /// <summary>
        /// Obtiene los adjuntosd por cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RAAdjuntoDC> ObtenerAdjuntosPorCita(long idCita)
        {
            List<RAAdjuntoDC> lstAdjuntos = new List<RAAdjuntoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObteneradjuntosCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAAdjuntoDC adjunto = new RAAdjuntoDC
                    {
                        IdAdjunto = Convert.ToInt32(reader["ADC_IdAdjunto"]),
                        Tamaño = Convert.ToDecimal(reader["ADC_Tamaño"]),
                        Extension = reader["ADC_Extension"].ToString(),
                        NombreArchivo = reader["ADC_NombreAdjunto"].ToString(),
                        UbicacionNombre = reader["ADC_UbicacionNombre"].ToString(),
                        FechaCreacion = Convert.ToDateTime(reader["ADC_FechaCreacion"]),
                        CreadoPor = reader["ADC_CreadoPor"].ToString()
                    };
                    using (FileStream fs = File.OpenRead(reader["ADC_UbicacionNombre"].ToString()))
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                        fs.Close();
                        adjunto.AdjuntoBase64 = Convert.ToBase64String(bytes);
                    }
                    lstAdjuntos.Add(adjunto);
                }
                conn.Close();
            }
            return lstAdjuntos;
        }

        /// <summary>
        /// Obtiene informacion de una lista de empleados
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public RAIdentificaEmpleadoDC ObtenerEmpleadosNovasoftPorId(string idEmpleado, long idCita)
        {
            RAIdentificaEmpleadoDC empleado = new RAIdentificaEmpleadoDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadoNovasoftPorId_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    empleado.Nombre = reader["NombreEmpleado"].ToString();
                    empleado.NumeroIdentificacion = reader["COD_EMP"].ToString();
                    empleado.email = reader["E_MAIL_ALT"].ToString();
                    empleado.IdTipoIntegrante = (RAEnumTipoIntegrante)reader["INC_IdTipoIntegrante"];
                    empleado.TipoIntegrante = reader["TIN_Descripcion"].ToString();
                }
                conn.Close();
            }
            return empleado;
        }
        #endregion

        #region Modficar

        /// <summary>
        /// Modifica la informacion de una cita
        /// </summary>
        /// <param name="cita"></param>
        public void ModificarCitaHija(RACitaDC cita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paModificarCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", cita.IdCita);
                cmd.Parameters.AddWithValue("@FechaInicio", cita.FechaInicioCita);
                cmd.Parameters.AddWithValue("@FechaFin", cita.FechaFinCita);
                cmd.Parameters.AddWithValue("@IdEstado", cita.IdEstado);
                cmd.Parameters.AddWithValue("@IdParamCita", cita.IdParametrizacion);
                cmd.Parameters.AddWithValue("@Lugar", cita.LugarCita);
                cmd.Parameters.AddWithValue("@DescripcionCita", cita.Descripcion);
                cmd.Parameters.AddWithValue("@Titulo", cita.Titulo);
                cmd.Parameters.AddWithValue("@Duracion", cita.PeriodoRepeticion.DuracionHoras);
                cmd.Parameters.AddWithValue("@IdPeriodo", cita.PeriodoRepeticion.IdPeriodoRepeticion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }


        /// <summary>
        /// Modifica una parametrizacion especifica
        /// </summary>
        /// <param name="parametrizacionCita"></param>
        public void ModificarParametrizacioCita(RAParametrizacionCitaDC parametrizacionCita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paModificarParametrizacionCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", parametrizacionCita.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", parametrizacionCita.FechaFin);
                cmd.Parameters.AddWithValue("@IdEstado", parametrizacionCita.IdEstado);
                cmd.Parameters.AddWithValue("@IdParamCita", parametrizacionCita.IdParametrizacion);
                cmd.Parameters.AddWithValue("@IdPeriodo", parametrizacionCita.IdPeriodoRepeticion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Modifica el compromiso segun la cita
        /// </summary>
        /// <param name="c"></param>
        public void ModificarCompromiso(RACompromisoDC compromiso)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paModificarCompromiso_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstado", compromiso.Estado);
                cmd.Parameters.AddWithValue("@IdCompromiso", compromiso.IdCompromiso);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        #endregion

        #region Eliminar

        /// <summary>
        /// Elimina las citas futuras
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        public void EliminarCitasFuturas(RAInfoCitasDC cita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarCitasFuturas_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", cita.FechaInicio);
                cmd.Parameters.AddWithValue("@IdParametrizacion", cita.IdParametrizacionCita);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        ///Elimina todas las citas 
        /// </summary>
        /// <param name="cita"></param>
        public void EliminarCitas(long idParametrizacionCita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarCitasActivas_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacion", idParametrizacionCita);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Elimina una cita especifica
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        public void EliminarCita(long idCita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarCitaEspecifica_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Elimina todos os integrantes asociados a una cita
        /// </summary>
        /// <param name="idCita"></param>
        public void EliminarIntegrantesCita(long idCita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarIntegrantesPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Elimina todas las notificaciones asociadas a una cita
        /// </summary>
        /// <param name="idCita"></param>
        public void EliminarNotificacionesCita(long idCita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarNotificacionesPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Elimina todos los adjuntos asociados a una cita
        /// </summary>
        /// <param name="idCita"></param>
        public void EliminarAdjuntosCita(long idCita)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarAdjuntosPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        #endregion

        #region Validaciones
        /// <summary>
        /// Valida si un determinado usuario es moderador
        /// </summary>
        /// <returns></returns>
        public bool ValidarModerador(long idCita, long idEmpleado)
        {
            bool esModerador = false;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerModeradorPorCita_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                conn.Open();
                var resultado = cmd.ExecuteScalar();

                if (resultado != null && (int)resultado > 0)
                {
                    esModerador = true;
                }
                conn.Close();
            }

            return esModerador;
        }
        #endregion

        #region MotorCitas
        /// <summary>
        /// Obtiene todos los recordatorios de las citas para enviarlos desde el motorRaps
        /// </summary>
        public List<RANotificacionCitaMotorDC> ObtenerRecordatoriosCitasMotor()
        {
            List<RANotificacionCitaMotorDC> lstNotificaciones = new List<RANotificacionCitaMotorDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCitasParaNotificar_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RANotificacionCitaMotorDC notificacion = new RANotificacionCitaMotorDC
                    {
                        IdNotificacionCita = Convert.ToInt64(reader["NCI_IdNotificacionCita"]),
                        IdCita = Convert.ToInt64(reader["CIT_IdCita"]),
                        DocumentoIntegrante = Convert.ToInt64(reader["INC_DocumentoIntegrante"]),
                        CorreoNotificar = reader["CorreoCorporativo"].ToString(),
                        DescripcionCita = reader["CIT_DescripcionCita"].ToString(),
                        FechaInicioCita = Convert.ToDateTime(reader["CIT_FechaInicio"]),
                        LugarCita = reader["CIT_LugarCita"].ToString()
                    };
                    lstNotificaciones.Add(notificacion);
                }
                conn.Close();
            }
            return lstNotificaciones;
        }
        /// <summary>
        /// Inserta la ejecucion de un recordatorio
        /// </summary>
        /// <param name="idRecordatorio"></param>
        public void InsertarEjecucionRecordatorio(RANotificacionCitaMotorDC notificacionCita, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarEjecucionRecordatorio_CIT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NCE_IdNotificacionCita", notificacionCita.IdNotificacionCita);
                cmd.Parameters.AddWithValue("@NCE_CorreoNotificado", notificacionCita.CorreoNotificar);
                cmd.Parameters.AddWithValue("@NCE_CreadoPor", usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conexionStringRaps.Clone();
            }
        }

        #endregion

    }
}
