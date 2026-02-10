using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModelosRequest.recogidas;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using CO.Servidor.Servicios.WebApi.ProcesosAutomaticos;
using CO.Servidor.Servicios.WebApiHub;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiRecogidas : ApiDominioBase
    {
        private static readonly ApiRecogidas instancia = (ApiRecogidas)FabricaInterceptorApi.GetProxy(new ApiRecogidas(), COConstantesModulos.MODULO_RECOGIDAS);
        public static ApiRecogidas Instancia
        {
            get { return ApiRecogidas.instancia; }
        }

        private ApiRecogidas()
        {
        }

        /// <summary>
        /// Consulta las cantidad de recogidas fijas pendientes de asignar mensajero
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        internal long ObtenerCantidadFijasPorAsignar(string idCentroServicio)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerCantidadFijasPorAsignar(idCentroServicio);
        }


        /// <summary>
        /// Obtiene las recogidas que no se han asignado
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <param name="idCol"></param>
        /// <param name="idClienteCredito"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasPorAsignar(string idCiudad, string idCol, int? idClienteCredito, long? idCentroServicio, int numeroPagina, int tamanioPagina)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerRecogidasPorAsignar(idCiudad, idCol, idClienteCredito, idCentroServicio, numeroPagina, tamanioPagina);
        }

        /// <summary>
        /// Obtener Motivos Estado Solicitud Recogida X Actor
        /// </summary>
        /// <param name="idActor"></param>
        /// <returns></returns>
        internal List<RGMotivoEstadoSolRecogidaDC> ObtenerMotivoEstadoSolRecogidaXActor(long idActor)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerMotivoEstadoSolRecogidaXActor(idActor);
        }

        /// <summary>
        /// Gestionar Con Motivo estado Solicitud Recogida
        /// </summary>
        /// <param name="solicitud"></param>
        /// <param name="idMotivo"></param>
        /// <param name="idActor"></param>
        internal void CancelarConMotivoSolRecogida(RGAsignarRecogidaDC solicitud, int idMotivo, int idActor)
        {
            FabricaServicios.ServicioRecogidas.CancelarConMotivoSolRecogida(solicitud, idMotivo, idActor);
            try
            {
                long documento = 0;
                long.TryParse(solicitud.DocPersonaResponsable, out documento);
                var mensaje = new ParametrosSignalR
                {
                    IdSolicitud = solicitud.IdSolicitudRecogida,
                    Documento = documento,
                    IdLocalidad = solicitud.IdSolicitudRecogida.ToString(),
                    IdAplicacion = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), solicitud.IdAplicacion.ToString()),
                    Mensaje = "se asigno la recogida :" + solicitud.IdSolicitudRecogida.ToString() + "\n Mensajero :" + solicitud.DocPersonaResponsable
                };

                NotificarAdministradores(mensaje);
                NotificaCancelacionAppsRecogidas(mensaje);
            }
            catch
            {
            }


        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona sin importar que esten activos en la empresa
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaReAsignarRecogidas(string idLocalidad)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerEmpleadosParaReAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// obtiene la lista de clientes credito
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerClientesCredito(string idCol)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerClientesCredito(idCol);
        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        internal List<RGEmpleadoDC> ObtenerEmpleadosParaAsignarRecogidas(string idLocalidad)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerEmpleadosParaAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// Obtiene todos los datos de un empleado por su cedula
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public RGEmpleadoDC ObtenerDatosDeEmpleadoPorCedula(string idEmpleado)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerDatosDeEmpleadoPorCedula(idEmpleado);
        }

        /// <summary>
        /// obtiene todos los centros de serivcio que pertenezcan a determinado col y a determinada ciudad
        /// </summary>
        /// <param name="idCol"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerCentrosDeServicio(string idCol, string idCiudad)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerCentrosDeServicio(idCol, idCiudad);
        }

        /// <summary>
        /// Inserta la programacion de recogidas fijas a un mensajero
        /// </summary>
        public void InsertarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            FabricaServicios.ServicioRecogidas.InsertarProgramacionRecogidasFijas(programacion);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ObtenerHorarioRecogida(RGRecogidasDC datosUbicacion)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerHorarioRecogida(datosUbicacion);
        }

        public RGEstadisticas ObtenerEstadisticas(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerEstadisticas(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerMensajerosActivosPorAplicacion(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerMensajerosActivosPorAplicacion(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }
        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerPosicionesMensajerosActivos(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerPosicionesMensajerosActivos(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public RGDetalleMensajeroBalance ObtenerDetalleMensajeroPorId(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idMensajero)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerDetalleMensajeroPorId(fechaInicial, fechaFinal, idMensajero);
        }

        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerMisRecogidasClientePeaton(idUsuario);
        }


        /// <summary>
        /// Obtiene las recogidas programadas a cierto empleado
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasPorEmpleado(string idEmpleado)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerRecogidasPorEmpleado(idEmpleado);
        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        public RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerUltimaSolicitud(numeroDocumento);
        }

        /// <summary>
        /// Edita las recogidas asignadas a un empleado con el fin de asignarselas a un nuevo empleado
        /// </summary>
        /// <param name="programacion"></param>
        public void EditarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            FabricaServicios.ServicioRecogidas.EditarProgramacionRecogidasFijas(programacion);
        }

        /// <summary>
        /// Obtiene la informacion de los mensajeros de la localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        internal List<RGMensajeroLocalidadDC> ObtenerMensajerosLocalidad(string idLocalidad)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerMensajerosLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtine las posiciones de dispositivos cercanos a la coordenada cercana
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        internal List<RGDispositivoMensajeroDC> ObtenerMensajerosCercanos(decimal latitud, decimal longitud)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerMensajerosCercanos(latitud, longitud);
        }

        internal List<RGDispositivoMensajeroDC> ObtenerMensajerosForzarRecogida(string ubicaciones)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerMensajerosForzarRecogida(ubicaciones);
        }


        /// <summary>
        /// Obtienes los registros a mostrar en el tablero de administracion de  solicitud de recogidas y generar los conteos
        /// </summary>
        /// <param name="idCentroservicio"></param>
        /// <param name="fechaConteo"></param>
        /// <returns></returns>
        internal RGDetalleyConteoRecogidasDC ConsultaDetConteosAdminRecogidas(string idCentroservicio, DateTime fechaConteo, long documento, string municipio)
        {
            return FabricaServicios.ServicioRecogidas.ConsultaDetConteosAdminRecogidas(idCentroservicio, fechaConteo, documento, municipio);
        }

        /// <summary>
        /// Consulta el detalle de una solicitud de recogida basada en la clase de solicitud
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="claseSolicitud"></param>
        /// <returns></returns>
        internal RGDetalleSolicitudRecogidaDC ObtenerDetalleSolRecogida(long idSolicitudRecogida, RGEnumClaseSolicitud claseSolicitud)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerDetalleSolrecogida(idSolicitudRecogida, claseSolicitud);
        }

        /// <summary>
        /// Consulta agencias de las territoriales de los centros de servicios
        /// </summary>
        /// <returns></returns>
        internal List<RGAgenciaDC> ObtenerAgencias()
        {
            return FabricaServicios.ServicioRecogidas.ObtenerAgencias();
        }

        /// <summary>
        /// Consulta las territoriales de los centros de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        internal List<RGTerritorialDC> ObtenerTerritoriales(string idCentroServicio)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerTerritoriales(idCentroServicio);
        }

        /// <summary>
        /// registra el forzado de un recogida esporadica
        /// </summary>
        /// <param name="solicitud"></param>
        internal void AsignarRecogida(RGAsignarRecogidaDC solicitud)
        {
            FabricaServicios.ServicioRecogidas.AsignarRecogida(solicitud);

            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();

                long documento = 0;
                long.TryParse(solicitud.DocPersonaResponsable, out documento);
                var mensaje = new ParametrosSignalR
                {
                    IdSolicitud = solicitud.IdSolicitudRecogida,
                    Documento = documento,
                    IdLocalidad = solicitud.IdSolicitudRecogida.ToString(),
                    IdAplicacion = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), solicitud.IdAplicacion.ToString()),
                    Mensaje = "se asigno la recogida :" + solicitud.IdSolicitudRecogida.ToString() + "\n Mensajero :" + solicitud.DocPersonaResponsable
                };

                NotificarAdministradores(mensaje);
                NotificaAsignacionAppsRecogidas(mensaje);
            }
            catch
            {
            }
        }


        /// <summary>
        /// Inserta una gestion de telemercadeo
        /// </summary>
        /// <param name="telemercadeo">Gestion del telemercadeo</param>
        internal void InsertarTelemercadeo(RGTelemercadeo telemercadeo)
        {
            FabricaServicios.ServicioRecogidas.InsertarTelemercadeo(telemercadeo);
        }

        /// <summary>
        /// inserta un cambio de estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        public void InsertarEstadoSolRecogidaTraza(RGAsignarRecogidaDC solicitud, EnumEstadoSolicitudRecogida nuevoEstado)
        {
            FabricaServicios.ServicioRecogidas.InsertarEstadoSolRecogidaTraza(solicitud, nuevoEstado);
        }

        /// <summary>
        /// ModificarRecogidaEsporadica
        /// </summary>
        /// <param name="recogida"></param>
        internal void ModificarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            FabricaServicios.ServicioRecogidas.ModificarRecogidaEsporadica(recogida);
        }

        #region Recogidas Controller App
        /// <summary>
        /// Metodo para obtener las recogidas disponibles por mensajero
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasDisponibles(string idMunicipio)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerRecogidasDisponibles(idMunicipio);
        }

        /// <summary>
        /// Obtener recogidas reservadas por mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasReservadasMensajero(string numIdentificacion)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerRecogidasReservadasMensajero(numIdentificacion);
        }

        /// <summary>
        /// metodo para obtener las recogidas efectivas por mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        internal List<RGRecogidasDC> ObtenerRecogidasEfectivasMensajero(string numIdentificacion)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerRecogidasEfectivasMensajero(numIdentificacion);
        }

        internal void ModificarCoordenadasRecogidaEsporadica(RGRecogidaEsporadicaDC recogida)
        {
            FabricaServicios.ServicioRecogidas.ModificarCoordenadasRecogidaEsporadica(recogida);
        }
        #endregion

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            long id = FabricaServicios.ServicioRecogidas.InsertarRecogidaEsporadica(recogida);

            if (recogida.FechaRecogida.Year > 1 && recogida.FechaRecogida.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString())
            {
                MotorRecogidas.Instancia.NotificarNuevaRecogida(recogida.Ciudad, true, recogida.Direccion);
            }

            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();

                long documento = 0;
                long.TryParse(recogida.NumeroDocumento, out documento);
                var mensaje = new ParametrosSignalR
                {
                    IdSolicitud = recogida.IdAsignacion,
                    Documento = documento,
                    IdLocalidad = id.ToString(),
                    IdAplicacion = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), "1" /*recogida.IdAplicacion.ToString()*/),
                    Mensaje = "se asigno la recogida :" + id.ToString() + "\n Mensajero :" + recogida.NumeroDocumento
                };

                NotificarAdministradores(mensaje);
                NotificaNuevaAppsRecogidas(mensaje);
            }
            catch
            {
            }
            return id;
        }

        /// <summary>
        /// Inserta las recogidas esporadicas IVR
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarRecogidaEsporadicaIVR(RecogidaIVRRequest recogida)
        {
            recogida.Recogida.FechaRecogida = new DateTime(recogida.Anio, recogida.Mes, recogida.Dia, recogida.Hora, recogida.Minuto, 0);
            long id = FabricaServicios.ServicioRecogidas.InsertarRecogidaEsporadica(recogida.Recogida);
            if (recogida.Recogida.FechaRecogida.Year > 1 && recogida.Recogida.FechaRecogida.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString())
            {
                MotorRecogidas.Instancia.NotificarNuevaRecogida(recogida.Recogida.Ciudad, true, recogida.Recogida.Direccion);
            }

            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();

                long documento = 0;
                long.TryParse(recogida.Recogida.NumeroDocumento, out documento);
                var mensaje = new ParametrosSignalR
                {
                    IdSolicitud = recogida.Recogida.IdAsignacion,
                    Documento = documento,
                    IdLocalidad = id.ToString(),
                    IdAplicacion = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), "1" /*recogida.IdAplicacion.ToString()*/),
                    Mensaje = "se asigno la recogida :" + id.ToString() + "\n Mensajero :" + recogida.Recogida.NumeroDocumento
                };

                NotificarAdministradores(mensaje);
                NotificaNuevaAppsRecogidas(mensaje);
            }
            catch
            {
            }
            return id;
        }

        /// <summary>
        /// metodo para asignar la recogida (esporádica o fija)
        /// </summary>
        /// <param name="rGAsignarRecogidaDC"></param>
        internal void EjecutarRecogida(RecogidaRequest recogida)
        {
            FabricaServicios.ServicioRecogidas.EjecutarRecogida(recogida.Recogida, recogida.IdSistema, recogida.TipoNovedad, recogida.Parametros, recogida.IdCiudad);
        }


        #region IVR

        public string ObtenerNombreyDireccionCliente([FromUri]string telefono)
        {
            return FabricaServicios.ServicioRecogidas.ObtenerNombreyDireccionCliente(telefono);
        }

        #endregion
        internal void NotificarAdministradores(ParametrosSignalR notificacion)
        {

            Task taskArray;

            taskArray = new Task(() =>
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
                context.Clients.All.enviarNotificacionAdministradorRecogida(notificacion);
            });
            taskArray.Start();

        }
        internal void NotificaNuevaAppsRecogidas(ParametrosSignalR mensaje)
        {

            List<PADispositivoMovil> dispositivos = PushGeneral.ServicioParametros.ObtenerDispositivosMovilesIdentificacionEmpleado(mensaje.Documento);
            dispositivos.ForEach(d =>
            {
                Task.Factory.StartNew(() =>
                {
                    string msg = "{\"title\":\"Nueva recogida\", \"message\": \"Existe una nueva recogida. \" }";
                    PushGeneral.Instancia.EnviarNotificacionAndroidPAM(d.TokenDispositivo.Trim(), msg);
                });

            });
        }
        internal void NotificaAsignacionAppsRecogidas(ParametrosSignalR mensaje)
        {
            List<PADispositivoMovil> dispositivos = PushGeneral.ServicioParametros.ObtenerDispositivosMovilesEmpleadosCiudad(mensaje.IdLocalidad);
            dispositivos.ForEach(d =>
            {
                Task.Factory.StartNew(() =>
                {
                    string msg = "{\"title\":\"Nueva Asignacion\", \"message\": \"Tienes asignada una nueva recogida. \" }";
                    PushGeneral.Instancia.EnviarNotificacionAndroidPAM(d.TokenDispositivo.Trim(), msg);
                });

            });
        }
        internal void NotificaCancelacionAppsRecogidas(ParametrosSignalR mensaje)
        {
            List<PADispositivoMovil> dispositivos = PushGeneral.ServicioParametros.ObtenerDispositivosMovilesEmpleadosCiudad(mensaje.IdLocalidad);
            dispositivos.ForEach(d =>
            {
                Task.Factory.StartNew(() =>
                {
                    string msg = "{\"title\":\"Cancelacion recogida\", \"message\": \"Te cancelaron una recogida. \" }";
                    PushGeneral.Instancia.EnviarNotificacionAndroidPAM(d.TokenDispositivo.Trim(), msg);
                });

            });
        }



    }
}