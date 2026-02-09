using CO.Servidor.Raps.Datos;
using CO.Servidor.Servicios.ContratoDatos.Raps.Citas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace CO.Servidor.Raps
{

    public class RACitas : ControllerBase
    {
        #region Singleton
        private static readonly RACitas instancia = (RACitas)FabricaInterceptores.GetProxy(new RACitas(), COConstantesModulos.MODULO_RAPS);

        public static RACitas Instancia
        {
            get { return RACitas.instancia; }
        }
        #endregion

        #region Insertar

        /// <summary>
        /// Inserta la o las citas de acuerdo al tiempo de repeticion
        /// </summary>
        /// <param name="cita"></param>
        /// 
        public void InsertarCita(RACitaDC cita)
        {

            List<DateTime> lstDiasCitas = new List<DateTime>();
            List<DateTime> lstFestivos = new List<DateTime>();
            long idCitaHija;
            bool fechaValida;
            List<string> users = new List<string>();
            using (TransactionScope scope = new TransactionScope())
            {
                if (cita.ParametrizacionCita.IdPeriodoRepeticion == 0)
                {
                    cita.FechasNoSecuenciales = cita.FechasNoSecuenciales.OrderBy(f => f).ToList();
                    int tamañoLista = cita.FechasNoSecuenciales.Count;
                    cita.ParametrizacionCita.FechaInicio = cita.FechasNoSecuenciales[0].AddHours(-5);//TODO RANCID: FECHAS DESDE JAVASCRIPT
                    cita.ParametrizacionCita.FechaFin = cita.FechasNoSecuenciales[tamañoLista - 1].AddHours(-5); //TODO RANCID: FECHAS DESDE JAVASCRIPT
                    lstFestivos = PAParametros.Instancia.ObtenerFestivosSinCache(cita.ParametrizacionCita.FechaInicio, cita.ParametrizacionCita.FechaFin, "057");
                    cita.ParametrizacionCita.IdPeriodoRepeticion = RARepositorioCitas.Instancia.InsertarPeriodoRepeticion(cita.PeriodoRepeticion);
                    cita.IdParametrizacion = RARepositorioCitas.Instancia.InsertarParametrizacionCitas(cita.ParametrizacionCita);
                    cita.FechasNoSecuenciales.ForEach(f =>
                    {
                        if (f.DayOfWeek != DayOfWeek.Sunday && !lstFestivos.Contains(f.Date))
                        {
                            f = f.AddHours(-5);//TODO RANCID: FECHAS DESDE JAVASCRIPT 
                                               //cita.ParametrizacionCita.FechaInicio = f;
                                               //cita.ParametrizacionCita.FechaFin = f.Date;
                                               //cita.ParametrizacionCita.IdPeriodoRepeticion = RARepositorioCitas.Instancia.InsertarPeriodoRepeticion(cita.PeriodoRepeticion);
                                               //cita.IdParametrizacion = RARepositorioCitas.Instancia.InsertarParametrizacionCitas(cita.ParametrizacionCita);
                            cita.IdEstado = RAEnumEstadoCita.Activa;
                            cita.FechaInicioCita = f;
                            cita.FechaFinCita = f.AddHours(cita.PeriodoRepeticion.DuracionHoras);
                            idCitaHija = 0;
                            idCitaHija = RARepositorioCitas.Instancia.InsertarCitaHija(cita);
                            cita.Integrantes.ForEach(i =>
                            {
                                i.IdCita = idCitaHija;
                                RARepositorioCitas.Instancia.InsertarIntegrantePorCita(i);
                                users.Add(i.DocumentoIntegrante.ToString());
                            });
                            cita.Notificacion.ForEach(n =>
                            {
                                fechaValida = true;
                                n.IdCita = idCitaHija;
                                switch (n.IdPeriodoNotificacion)
                                {
                                    case 1:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddDays((n.TiempoRecordatorio * -1));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    case 2:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddDays(((n.TiempoRecordatorio * -1) * 7));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    case 3:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddHours((n.TiempoRecordatorio * -1));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    case 4:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddMinutes((n.TiempoRecordatorio * -1));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                if (fechaValida)
                                {
                                    RARepositorioCitas.Instancia.InsertarNotificacionCita(n);
                                }
                            });

                        }

                    });
                }
                else
                {
                    cita.ParametrizacionCita.FechaInicio = cita.ParametrizacionCita.FechaInicio.AddHours(-5);//TODO RANCID: FECHAS DESDE JAVASCRIPT
                    cita.ParametrizacionCita.FechaFin = cita.ParametrizacionCita.FechaFin.AddDays(-1); //TODO RANCID: FECHAS DESDE JAVASCRIPT
                    lstFestivos = PAParametros.Instancia.ObtenerFestivosSinCache(cita.ParametrizacionCita.FechaInicio, cita.ParametrizacionCita.FechaFin, "057");
                    cita.ParametrizacionCita.IdPeriodoRepeticion = RARepositorioCitas.Instancia.InsertarPeriodoRepeticion(cita.PeriodoRepeticion);
                    cita.IdParametrizacion = RARepositorioCitas.Instancia.InsertarParametrizacionCitas(cita.ParametrizacionCita);
                    lstDiasCitas = CalculoFechasParaCitas(cita);
                    lstDiasCitas.ForEach(c =>
                    {
                        if (c.DayOfWeek != DayOfWeek.Sunday && !lstFestivos.Contains(c.Date))
                        {
                            cita.IdEstado = RAEnumEstadoCita.Activa;
                            cita.FechaInicioCita = c;
                            cita.FechaFinCita = c.AddHours(cita.PeriodoRepeticion.DuracionHoras);
                            idCitaHija = 0;
                            idCitaHija = RARepositorioCitas.Instancia.InsertarCitaHija(cita);
                            cita.Integrantes.ForEach(i =>
                            {
                                i.IdCita = idCitaHija;
                                RARepositorioCitas.Instancia.InsertarIntegrantePorCita(i);
                                users.Add(i.DocumentoIntegrante.ToString());
                            });
                            cita.Notificacion.ForEach(n =>
                            {
                                fechaValida = true;
                                n.IdCita = idCitaHija;
                                n.IdCita = idCitaHija;
                                switch (n.IdPeriodoNotificacion)
                                {
                                    case 1:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddDays((n.TiempoRecordatorio * -1));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    case 2:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddDays(((n.TiempoRecordatorio * -1) * 7));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    case 3:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddHours((n.TiempoRecordatorio * -1));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    case 4:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddMinutes((n.TiempoRecordatorio * -1));
                                        if (n.HoraNotificacion < DateTime.Now)
                                        {
                                            fechaValida = false;
                                            return;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                RARepositorioCitas.Instancia.InsertarNotificacionCita(n);
                            });
                        }
                    });
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// calcula la lista de fecha para las citas segun el periodo
        /// </summary>
        /// <param name="cita"></param>
        /// <returns></returns>
        public List<DateTime> CalculoFechasParaCitas(RACitaDC cita)
        {
            List<DateTime> lstDiasCitas = new List<DateTime>();
            DateTime fechaCita = cita.ParametrizacionCita.FechaInicio;
            int contador = 0;

            if (cita.PeriodoRepeticion.IdTipoPeriodo == RAEnumTipoPeriodo.Diario)
            {
                for (var i = 0; fechaCita.Date <= cita.ParametrizacionCita.FechaFin; i = i + cita.PeriodoRepeticion.Intervalo)
                {
                    fechaCita = cita.ParametrizacionCita.FechaInicio.AddDays(i);
                    if (fechaCita.Date <= cita.ParametrizacionCita.FechaFin.Date)
                    {
                        lstDiasCitas.Add(fechaCita);
                    }
                }
            }

            else if (cita.PeriodoRepeticion.IdTipoPeriodo == RAEnumTipoPeriodo.Semanal)
            {
                if (cita.PeriodoRepeticion.Lunes != null)
                {
                    lstDiasCitas.AddRange(CalculoFechasPorSemana(cita, DayOfWeek.Monday, cita.PeriodoRepeticion.Lunes.Value.AddHours(-5)));
                }
                if (cita.PeriodoRepeticion.Martes != null)
                {
                    lstDiasCitas.AddRange(CalculoFechasPorSemana(cita, DayOfWeek.Tuesday, cita.PeriodoRepeticion.Martes.Value.AddHours(-5)));
                }
                if (cita.PeriodoRepeticion.Miercoles != null)
                {
                    lstDiasCitas.AddRange(CalculoFechasPorSemana(cita, DayOfWeek.Wednesday, cita.PeriodoRepeticion.Miercoles.Value.AddHours(-5)));
                }
                if (cita.PeriodoRepeticion.Jueves != null)
                {
                    lstDiasCitas.AddRange(CalculoFechasPorSemana(cita, DayOfWeek.Thursday, cita.PeriodoRepeticion.Jueves.Value.AddHours(-5)));
                }
                if (cita.PeriodoRepeticion.Viernes != null)
                {
                    lstDiasCitas.AddRange(CalculoFechasPorSemana(cita, DayOfWeek.Friday, cita.PeriodoRepeticion.Viernes.Value.AddHours(-5)));
                }
                if (cita.PeriodoRepeticion.Sabado != null)
                {
                    lstDiasCitas.AddRange(CalculoFechasPorSemana(cita, DayOfWeek.Saturday, cita.PeriodoRepeticion.Sabado.Value.AddHours(-5)));
                }
            }
            else if (cita.PeriodoRepeticion.IdTipoPeriodo == RAEnumTipoPeriodo.Mensual)
            {
                for (var i = cita.PeriodoRepeticion.Intervalo; fechaCita <= cita.ParametrizacionCita.FechaFin; i = i + cita.PeriodoRepeticion.Intervalo)
                {
                    fechaCita = cita.ParametrizacionCita.FechaInicio.AddMonths(i);
                    fechaCita = new DateTime(fechaCita.Year, fechaCita.Month, 1, fechaCita.Hour, fechaCita.Second, 0);
                    int posicionDia = 0;

                    for (int j = 1; j < 6; j++)
                    {
                        if (NthDayOfMonth(cita.ParametrizacionCita.FechaInicio, cita.ParametrizacionCita.FechaInicio.DayOfWeek, j))
                            posicionDia = j;
                    }

                    fechaCita = fechaCita.Next(cita.ParametrizacionCita.FechaInicio.DayOfWeek).AddDays((posicionDia - 1) * 7);
                    if (fechaCita <= cita.ParametrizacionCita.FechaFin)
                    {
                        lstDiasCitas.Add(fechaCita);
                    }
                }
            }
            return lstDiasCitas;
        }

        /// <summary>
        /// Inserta la gestion de una cita
        /// </summary>
        /// <param name="gestion"></param>
        public void InsertarGestionCita(RACitaDC gestion)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                RARepositorioCitas.Instancia.InsertarGestionCita(gestion);

                if (gestion.Compromisos != null && gestion.Compromisos.Count >= 1)
                {
                    gestion.Compromisos.ForEach(c =>
                    {
                        c.IdCita = gestion.IdCita;
                        RARepositorioCitas.Instancia.InsertarCompromiso(c);
                    });
                }

                if (gestion.Adjuntos != null && gestion.Adjuntos.Count > 1)
                {
                    gestion.Adjuntos.ForEach(adj =>
                    {
                        adj.IdCita = gestion.IdCita;
                        string rutaAdjuntos = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderAdjCitasRAPS");
                        string carpetaDestino = Path.Combine(string.Concat(rutaAdjuntos, "\\", DateTime.Now.ToString("s").Substring(0, 10)));
                        if (!Directory.Exists(carpetaDestino))
                        {
                            Directory.CreateDirectory(carpetaDestino);
                        }
                        byte[] bytebuffer = Convert.FromBase64String(adj.Adjunto);
                        string ruta = string.Concat(carpetaDestino, "\\", adj.NombreArchivo, "_", gestion.IdCita, ".", adj.Extension);
                        adj.UbicacionNombre = ruta;
                        RARepositorioCitas.Instancia.InsertarAdjuntoCita(adj);
                        File.WriteAllBytes(ruta, bytebuffer);
                    });
                }
                scope.Complete();
            }

        }

        /// <summary>
        /// valida que orden tiene el dia
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dow"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private bool NthDayOfMonth(DateTime date, DayOfWeek dow, int n)
        {
            int d = date.Day;
            return date.DayOfWeek == dow && (d - 1) / 7 == (n - 1);
        }

        /// <summary>
        /// calcula las fechas por dia para el periodo semanal
        /// </summary>
        /// <param name="cita"></param>
        /// <param name="diaDeLaSemana"></param>
        /// <param name="horaCita"></param>
        /// <returns></returns>
        public List<DateTime> CalculoFechasPorSemana(RACitaDC cita, DayOfWeek diaDeLaSemana, DateTime horaCita)
        {
            List<DateTime> lstFechasCitas = new List<DateTime>();
            DateTime fechaCalculada = new DateTime();
            DateTime fechaInicio = cita.ParametrizacionCita.FechaInicio;
            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day, horaCita.Hour, horaCita.Minute, 0);
            //int multiploIntervalo = cita.PeriodoRepeticion.IdTipoPeriodo == RAEnumTipoPeriodo.Semanal ? 7 : cita.PeriodoRepeticion.IdTipoPeriodo == RAEnumTipoPeriodo.Mensual? 30 : 0;

            int daysUntil = ((int)diaDeLaSemana - (int)fechaInicio.DayOfWeek + 7) % 7;
            fechaCalculada = fechaInicio.AddDays(daysUntil);

            if (diaDeLaSemana == fechaInicio.DayOfWeek)
            {
                fechaCalculada = fechaInicio;
            }

            lstFechasCitas.Add(fechaCalculada);

            while (fechaCalculada <= cita.ParametrizacionCita.FechaFin)
            {
                fechaCalculada = fechaCalculada.AddDays(cita.PeriodoRepeticion.Intervalo * 7);
                if (fechaCalculada <= cita.ParametrizacionCita.FechaFin)
                {
                    lstFechasCitas.Add(fechaCalculada);
                }
            }
            return lstFechasCitas;
        }

        /// <summary>
        /// Inserta la asistencia de una cita
        /// </summary>
        /// <param name="asistencia"></param>
        public void InsertarAsistencia(List<RAAsistenciaCita> asistencia)
        {
            asistencia.ForEach(a =>
            {
                RARepositorioCitas.Instancia.InsertarAsistencia(a);
            });
        }

        /// <summary>
        /// Inserta la relacion de los raps credos a partir de citas
        /// </summary>
        public void InsertarRelacionRapsCitas(long idCita, long idRaps)
        {
            RARepositorioCitas.Instancia.InsertarRapsPorCita(idCita, idRaps);
        }

        /// <summary>
        /// Inserta la gestios de los compromisos
        /// </summary>
        /// <param name="compromiso"></param>
        public void InsertarGestionCompromisos(List<RACompromisoDC> compromisos)
        {
            compromisos.ForEach(c =>
            {
                RARepositorioCitas.Instancia.ModificarCompromiso(c);
            });
        }

        #endregion

        #region Consultar
        /// <summary>
        /// obtiene todas las citas programadas para cierto rango de fechas y para cierto usuario
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerCitasPorFechaEIntegrante(DateTime fechaInicio, DateTime fechaFin, long documentoIntegrante)
        {
            return RARepositorioCitas.Instancia.ObtenerCitasPorFechaEIntegrante(fechaInicio, fechaFin, documentoIntegrante);
        }

        /// <summary>
        /// Obtiene el detalle de una cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        /// 
        public RACitaDC ObtenerDetalleCita(long idCita)
        {

            RACitaDC cita = new RACitaDC();
            cita = RARepositorioCitas.Instancia.ObtenerDetalleCita(idCita);
            cita.IdCita = idCita;
            cita.PeriodoRepeticion = RARepositorioCitas.Instancia.ObtenerperiodoRepeticionCita(idCita);
            cita.Integrantes = RARepositorioCitas.Instancia.ObtenerIntegrantesPorCita(idCita);
            cita.Notificacion = RARepositorioCitas.Instancia.ObtenerNotificacionPorCita(idCita);
            cita.Adjuntos = RARepositorioCitas.Instancia.ObtenerAdjuntosPorCita(idCita);
            cita.Compromisos = RARepositorioCitas.Instancia.ObtenerCompromisosPorCita(idCita);
            cita.Asistencia = RARepositorioCitas.Instancia.ObtenerAsistenciaPorCita(idCita);
            return cita;
        }

        /// <summary>
        /// Obtiene los consolidados segun el tipod e raps para mostrar en el calendario
        /// </summary>
        /// <param name="agrupamiento"></param>
        /// <returns></returns>
        public List<RAFormatoCalendarioDC> ObtenerConsolidadoPorEstadoDeSolicitudRaps(RAAgrupamientoRapsDC agrupamiento)
        {
            List<RAFormatoCalendarioDC> lstRaps = new List<RAFormatoCalendarioDC>();
            switch (agrupamiento.TipoAgrupamiento)
            {
                case RAEnumTipoAgrupamiento.Pendientes:
                    lstRaps = RARepositorioCitas.Instancia.ObtenerAgrupamientoRapsPendientes(agrupamiento);
                    break;
                case RAEnumTipoAgrupamiento.Resueltos:
                    lstRaps = RARepositorioCitas.Instancia.ObtenerAgrupamientoRapsResueltos(agrupamiento);
                    break;
                case RAEnumTipoAgrupamiento.Vencidos:
                    lstRaps = RARepositorioCitas.Instancia.ObtenerAgrupamientoRapsVencidos(agrupamiento);
                    break;
                default:
                    break;
            }
            return lstRaps;
        }

        /// <summary>
        /// Obtiene los periodo en que se daran las notificaciones de una cita
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ObtenerPeriodoNotificacion()
        {
            return RARepositorioCitas.Instancia.ObtenerPeriodoNotificacion();
        }

        /// <summary>
        /// Obtiene informacion de una lista de empleados
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosNovasoftPorId(List<RAIntegranteCitaDC> integrantes)
        {
            List<RAIdentificaEmpleadoDC> lstEmpleados = new List<RAIdentificaEmpleadoDC>();
            integrantes.ForEach(i =>
            {
                RAIdentificaEmpleadoDC empleado = new RAIdentificaEmpleadoDC();
                empleado = RARepositorioCitas.Instancia.ObtenerEmpleadosNovasoftPorId(i.DocumentoIntegrante.ToString(), i.IdCita);
                lstEmpleados.Add(empleado);
            });
            return lstEmpleados;
        }

        /// <summary>
        /// Obtiene los tipos de integrantes
        /// </summary>
        /// <returns></returns>
        public List<RAIntegranteCitaDC> ObtenertiposIntegrantes()
        {
            return RARepositorioCitas.Instancia.ObtenertiposIntegrantes();
        }

        /// <summary>
        /// Obtiene los compromisos por cita
        /// </summary>
        /// <param name="idCita"></param>
        /// <returns></returns>
        public List<RACompromisoDC> ObtenerCompromisosPorCita(long idCita)
        {
            return RARepositorioCitas.Instancia.ObtenerCompromisosPorCita(idCita);
        }
        #endregion

        #region Modficar

        /// <summary>
        /// Modificar cita especifica
        /// </summary>
        /// <param name="cita"></param>
        public void ModificarCita(RACitaDC cita)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (cita.ParametrizacionCita.IdPeriodoRepeticion == 0)
                {
                    cita.FechasNoSecuenciales.ForEach(f =>
                    {
                        cita.FechaInicioCita = f.AddHours(-5);
                        cita.FechaFinCita = cita.FechaInicioCita.AddHours(cita.PeriodoRepeticion.DuracionHoras);
                    });
                }
                else
                {
                    cita.FechaInicioCita = cita.FechaInicioCita.AddHours(-5);//TODO RANCID: FECHAS DESDE JAVASCRIPT
                    cita.FechaFinCita = cita.FechaInicioCita.AddHours(cita.PeriodoRepeticion.DuracionHoras); ; //TODO RANCID: FECHAS DESDE JAVASCRIPT
                }

                RARepositorioCitas.Instancia.EliminarIntegrantesCita(cita.IdCita);
                RARepositorioCitas.Instancia.EliminarNotificacionesCita(cita.IdCita);

                cita.Integrantes.ForEach(i =>
                {
                    i.IdCita = cita.IdCita;
                    RARepositorioCitas.Instancia.InsertarIntegrantePorCita(i);
                });
                cita.Notificacion.ForEach(n =>
                {
                    n.IdCita = cita.IdCita;
                    switch (n.IdPeriodoNotificacion)
                    {
                        case 1:
                            n.HoraNotificacion = cita.FechaInicioCita.AddDays((n.TiempoRecordatorio * -1));
                            break;
                        case 2:
                            n.HoraNotificacion = cita.FechaInicioCita.AddDays(((n.TiempoRecordatorio * -1) * 7));
                            break;
                        case 3:
                            n.HoraNotificacion = cita.FechaInicioCita.AddHours((n.TiempoRecordatorio * -1));
                            break;
                        case 4:
                            n.HoraNotificacion = cita.FechaInicioCita.AddMinutes((n.TiempoRecordatorio * -1));
                            break;
                        default:
                            break;
                    }
                    RARepositorioCitas.Instancia.InsertarNotificacionCita(n);
                });
                RARepositorioCitas.Instancia.ModificarCitaHija(cita);
                scope.Complete();
            }
        }

        /// <summary>
        /// Modifica todas las citas de acuerdo con la nueva informacion suministrada
        /// </summary>
        public void ModificarCitas(RACitaDC cita)
        {
            List<RAInfoCitasDC> lstComparable = new List<RAInfoCitasDC>();
            bool seModifico;
            long idCitaHija;
            long idParametrizacionAntigua = cita.IdParametrizacion;
            long idPeriodoRepeticionNuevo;
            long idParametrizacionNueva = 0;
            DateTime fechaIni = cita.FechaInicioCita;

            using (TransactionScope scope = new TransactionScope())
            {
                if (cita.ParametrizacionCita.IdPeriodoRepeticion != 0)
                {
                    cita.ParametrizacionCita.FechaInicio = cita.ParametrizacionCita.FechaInicio.AddHours(-5);//TODO RANCID: FECHAS DESDE JAVASCRIPT
                    //cita.ParametrizacionCita.FechaFin = cita.ParametrizacionCita.FechaFin.AddDays(-1); //TODO RANCID: FECHAS DESDE JAVASCRIPT
                    idPeriodoRepeticionNuevo = RARepositorioCitas.Instancia.InsertarPeriodoRepeticion(cita.PeriodoRepeticion);
                    cita.ParametrizacionCita.IdPeriodoRepeticion = idPeriodoRepeticionNuevo;
                    idParametrizacionNueva = RARepositorioCitas.Instancia.InsertarParametrizacionCitas(cita.ParametrizacionCita);
                }


                List<DateTime> lstFechasNuevas = new List<DateTime>();
                if (cita.ParametrizacionCita.IdPeriodoRepeticion == 0)
                {
                    lstFechasNuevas = cita.FechasNoSecuenciales;
                }
                else
                {
                    lstFechasNuevas = CalculoFechasParaCitas(cita);
                }

                lstFechasNuevas.ForEach(f =>
                {

                    if (cita.ParametrizacionCita.IdPeriodoRepeticion == 0)
                    {
                        f = f.AddHours(-5);
                        cita.ParametrizacionCita.FechaInicio = f;//TODO RANCID: FECHAS DESDE JAVASCRIPT
                        cita.ParametrizacionCita.FechaFin = cita.ParametrizacionCita.FechaInicio; //TODO RANCID: FECHAS DESDE JAVASCRIPT
                        idPeriodoRepeticionNuevo = RARepositorioCitas.Instancia.InsertarPeriodoRepeticion(cita.PeriodoRepeticion);
                        cita.ParametrizacionCita.IdPeriodoRepeticion = idPeriodoRepeticionNuevo;
                        idParametrizacionNueva = RARepositorioCitas.Instancia.InsertarParametrizacionCitas(cita.ParametrizacionCita);
                    }

                    seModifico = false;
                    if (f.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (cita.IdTipoEliminacion == RAEnumTipoEliminacion.Todas)
                        {
                            lstComparable = RARepositorioCitas.Instancia.ObtenerCitasPorParametrizacion(cita.IdParametrizacion, null);
                        }
                        else
                        {
                            lstComparable = RARepositorioCitas.Instancia.ObtenerCitasPorParametrizacion(cita.IdParametrizacion, cita.FechaInicioCita);
                        }

                        cita.IdEstado = RAEnumEstadoCita.Activa;
                        cita.IdParametrizacion = idParametrizacionNueva;
                        cita.FechaFinCita = f.AddHours(cita.PeriodoRepeticion.DuracionHoras);
                        cita.FechaInicioCita = f;
                        lstComparable.ForEach(c =>
                        {
                            if (f == c.FechaInicio)
                            {
                                RARepositorioCitas.Instancia.ModificarCitaHija(cita);
                                seModifico = true;
                            }
                        });
                        if (!seModifico)
                        {
                            idCitaHija = RARepositorioCitas.Instancia.InsertarCitaHija(cita);
                            cita.Integrantes.ForEach(i =>
                            {
                                i.IdCita = idCitaHija;
                                RARepositorioCitas.Instancia.InsertarIntegrantePorCita(i);
                            });
                            cita.Notificacion.ForEach(n =>
                            {
                                n.IdCita = idCitaHija;
                                switch (n.IdPeriodoNotificacion)
                                {
                                    case 1:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddDays((n.TiempoRecordatorio * -1));
                                        break;
                                    case 2:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddDays(((n.TiempoRecordatorio * -1) * 7));
                                        break;
                                    case 3:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddHours((n.TiempoRecordatorio * -1));
                                        break;
                                    case 4:
                                        n.HoraNotificacion = cita.FechaInicioCita.AddMinutes((n.TiempoRecordatorio * -1));
                                        break;
                                    default:
                                        break;
                                }
                                RARepositorioCitas.Instancia.InsertarNotificacionCita(n);
                            });
                        }
                    }
                });

                if (cita.IdTipoEliminacion == RAEnumTipoEliminacion.Todas)
                {
                    RARepositorioCitas.Instancia.EliminarCitas(idParametrizacionAntigua);
                }
                else
                {
                    RAInfoCitasDC citaAEliminar = new RAInfoCitasDC { FechaInicio = fechaIni, IdParametrizacionCita = idParametrizacionAntigua };
                    RARepositorioCitas.Instancia.EliminarCitasFuturas(citaAEliminar);
                }

                scope.Complete();
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
            RARepositorioCitas.Instancia.EliminarCitasFuturas(cita);
        }

        /// <summary>
        ///Elimina todas las citas 
        /// </summary>
        /// <param name="cita"></param>
        public void EliminarCitas(long idParametrizacionCita)
        {
            RARepositorioCitas.Instancia.EliminarCitas(idParametrizacionCita);
        }

        /// <summary>
        /// Elimina una cita especifica
        /// </summary>
        /// <param name="idParametrizacionCita"></param>
        public void EliminarCita(RAInfoCitasDC cita)
        {
            RARepositorioCitas.Instancia.EliminarCita(cita.IdCita);
        }
        #endregion

        #region Validacionesd

        /// <summary>
        /// Valida si un determinado usuario es moderador
        /// </summary>
        /// <returns></returns>
        public bool ValidarModerador(long idCita, long idEmpleado)
        {
            return RARepositorioCitas.Instancia.ValidarModerador(idCita, idEmpleado);
        }

        #endregion

        #region MotorCitas
        /// <summary>
        /// Obtiene todos los recordatorios de las citas para enviarlos desde el motorRaps
        /// </summary>
        public List<RANotificacionCitaMotorDC> ObtenerRecordatoriosCitasMotor()
        {
            return RARepositorioCitas.Instancia.ObtenerRecordatoriosCitasMotor();
        }

        /// <summary>
        /// Inserta la ejecucion de un recordatorio
        /// </summary>
        /// <param name="idRecordatorio"></param>
        public void InsertarEjecucionRecordatorio(RANotificacionCitaMotorDC notificacionCita, string usuario)
        {
            RARepositorioCitas.Instancia.InsertarEjecucionRecordatorio(notificacionCita, usuario);
        }
        #endregion

    }


}
