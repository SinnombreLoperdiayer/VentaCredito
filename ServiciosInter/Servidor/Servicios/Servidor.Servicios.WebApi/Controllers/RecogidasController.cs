using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.ModelosRequest.recogidas;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Recogidas")]
    public class RecogidasController : ApiController
    {
        #region Obtener      

        /// <summary>
        /// Consulta las cantidad de recogidas fijas pendientes de asignar mensajero
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerCantidadFijasPorAsignar/{idCentroServicio}")]
        public long ObtenerCantidadFijasPorAsignar(string idCentroServicio)
        {
            return ApiRecogidas.Instancia.ObtenerCantidadFijasPorAsignar(idCentroServicio);
        }

        /// <summary>
        /// Obtiene las recogidas que no se han asignado
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <param name="idCol"></param>
        /// <param name="idClienteCredito"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRecogidasPorAsignar")]
        [SeguridadWebApi]
        public List<RGRecogidasDC> ObtenerRecogidasPorAsignar([FromUri]string idCiudad, [FromUri] string idCol, [FromUri]int? idClienteCredito, [FromUri] long? idCentroServicio, [FromUri]int numeroPagina, [FromUri]int tamanioPagina)
        {
            return ApiRecogidas.Instancia.ObtenerRecogidasPorAsignar(idCiudad, idCol, idClienteCredito, idCentroServicio, numeroPagina, tamanioPagina);
        }


        /// <summary>
        /// Obtener Motivos Estado Solicitud Recogida X Actor
        /// </summary>
        /// <param name="idActor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMotivoEstadoSolRecogidaXActor/{idActor}")]
        [SeguridadWebApi]
        public List<RGMotivoEstadoSolRecogidaDC> ObtenerMotivoEstadoSolRecogidaXActor([FromUri]long idActor)
        {
            return ApiRecogidas.Instancia.ObtenerMotivoEstadoSolRecogidaXActor(idActor);
        }

        /// <summary>
        /// Gestionar Con Motivo estado Solicitud Recogida
        /// </summary>
        /// <param name="solicitudConMotivo"></param>
        [HttpPost]
        [Route("CancelarConMotivoSolRecogida")]
        [SeguridadWebApi]
        public void CancelarConMotivoSolRecogida([FromBody]SolicitudConMotivoRequest solicitudConMotivo)
        {
            ApiRecogidas.Instancia.CancelarConMotivoSolRecogida(solicitudConMotivo.Solicitud, solicitudConMotivo.IdMotivo, solicitudConMotivo.IdActor);
        }


        /// <summary>
        /// obtiene la lista de clientes credito
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerClientesCredito")]
        [SeguridadWebApi]
        public List<RGRecogidasDC> ObtenerClientesCredito([FromUri]string idCol)
        {
            return ApiRecogidas.Instancia.ObtenerClientesCredito(idCol);
        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerEmpleadosParaAsignarRecogidas/{idLocalidad}")]
        [SeguridadWebApi]
        public List<RGEmpleadoDC> ObtenerEmpleadosParaAsignarRecogidas(string idLocalidad)
        {
            return ApiRecogidas.Instancia.ObtenerEmpleadosParaAsignarRecogidas(idLocalidad);
        }


        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona sin importar que esten activos en la empresa
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerEmpleadosParaReAsignarRecogidas/{idLocalidad}")]
        [SeguridadWebApi]
        public List<RGEmpleadoDC> ObtenerEmpleadosParaReAsignarRecogidas(string idLocalidad)
        {
            return ApiRecogidas.Instancia.ObtenerEmpleadosParaReAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// Obtiene todos los datos de un empleado por su cedula
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerDatosDeEmpleadoPorCedula")]
        [SeguridadWebApi]
        public RGEmpleadoDC ObtenerDatosDeEmpleadoPorCedula([FromUri]string idEmpleado)
        {
            return ApiRecogidas.Instancia.ObtenerDatosDeEmpleadoPorCedula(idEmpleado);
        }

        /// <summary>
        /// obtiene todos los centros de serivcio que pertenezcan a determinado col y a determinada ciudad
        /// </summary>
        /// <param name="idCol"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerCentrosDeServicio")]
        [SeguridadWebApi]

        public List<RGRecogidasDC> ObtenerCentrosDeServicio([FromUri]string idCol, [FromUri]string idCiudad)
        {
            return ApiRecogidas.Instancia.ObtenerCentrosDeServicio(idCol, idCiudad);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="datosUbicacion"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ObtenerHorarioRecogida")]
        [SeguridadWebApi]
        public string ObtenerHorarioRecogida(RGRecogidasDC datosUbicacion)
        {
            return ApiRecogidas.Instancia.ObtenerHorarioRecogida(datosUbicacion);
        }

        /// <summary>
        /// Obtiene las recogidas programadas a cierto empleado
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("ObtenerRecogidasPorEmpleado")]
        [SeguridadWebApi]
        public List<RGRecogidasDC> ObtenerRecogidasPorEmpleado([FromUri]string idEmpleado)
        {
            return ApiRecogidas.Instancia.ObtenerRecogidasPorEmpleado(idEmpleado);
        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerUltimaSolicitud")]
        [SeguridadWebApi]
        public RGRecogidasDC ObtenerUltimaSolicitud([FromUri]string numeroDocumento)
        {
            return ApiRecogidas.Instancia.ObtenerUltimaSolicitud(numeroDocumento);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="datosUbicacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerEstadisticas")]
        [SeguridadWebApi]

        public RGEstadisticas ObtenerEstadisticas([FromUri]DateTime fechaInicial, [FromUri]Nullable<DateTime> fechaFinal, [FromUri]string idCiudad, [FromUri]string idTerritorial, [FromUri]string idRegional)
        {
            return ApiRecogidas.Instancia.ObtenerEstadisticas(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="datosUbicacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMensajerosActivosPorAplicacion")]
        [SeguridadWebApi]

        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerMensajerosActivosAplicacion([FromUri]DateTime fechaInicial, [FromUri]Nullable<DateTime> fechaFinal, [FromUri]string idCiudad, [FromUri]string idTerritorial, [FromUri]string idRegional)
        {
            return ApiRecogidas.Instancia.ObtenerMensajerosActivosPorAplicacion(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="datosUbicacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerPosicionesMensajerosActivos")]
        [SeguridadWebApi]
        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerPosicionesMensajerosActivos([FromUri]DateTime fechaInicial, [FromUri]Nullable<DateTime> fechaFinal, [FromUri]string idCiudad, [FromUri]string idTerritorial, [FromUri]string idRegional)
        {
            return ApiRecogidas.Instancia.ObtenerPosicionesMensajerosActivos(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="datosUbicacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerDetalleMensajeroPorId")]
        [SeguridadWebApi]

        public RGDetalleMensajeroBalance ObtenerDetalleMensajeroPorId([FromUri]DateTime fechaInicial, [FromUri]Nullable<DateTime> fechaFinal, [FromUri]string idMensajero)
        {
            return ApiRecogidas.Instancia.ObtenerDetalleMensajeroPorId(fechaInicial, fechaFinal, idMensajero);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="datosUbicacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRecogidasClientePeaton")]
        [SeguridadWebApi]
        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            return ApiRecogidas.Instancia.ObtenerMisRecogidasClientePeaton(idUsuario);
        }

        #endregion

        #region Insertar

        /// <summary>
        /// Inserta la programacion de recogidas fijas a un mensajero
        /// </summary>
        [HttpPost]
        [Route("InsertarProgramacionRecogidasFijas")]
        [SeguridadWebApi]
        public void InsertarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            ApiRecogidas.Instancia.InsertarProgramacionRecogidasFijas(programacion);
        }

        /// <summary>
        /// Consulta las territoriales de los centros de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTerritoriales/{idCentroServicio}")]
        public List<RGTerritorialDC> ObtenerTerritoriales(string idCentroServicio)
        {
            return ApiRecogidas.Instancia.ObtenerTerritoriales(idCentroServicio);
        }

        /// <summary>
        /// Consulta agencias de las territoriales de los centros de servicios
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerAgencias")]
        public List<RGAgenciaDC> ObtenerAgencias()
        {
            return ApiRecogidas.Instancia.ObtenerAgencias();
        }


        /// <summary>
        /// Obtienes los registros a mostrar en el tablero de administracion de  solicitud de recogidas y generar los conteos
        /// </summary>
        /// <param name="IdCentroservicio"></param>
        /// <param name="FechaConteo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConsultaDetConteosAdminRecogidas/{IdCentroservicio}/{FechaConteo}/{documento}/{municipio}")]
        public RGDetalleyConteoRecogidasDC ConsultaDetConteosAdminRecogidas(String IdCentroservicio, DateTime FechaConteo, long documento, string municipio)
        {
            return ApiRecogidas.Instancia.ConsultaDetConteosAdminRecogidas(IdCentroservicio, FechaConteo, documento, municipio);
        }
        /// <summary>
        /// Obtiene los mensajeros de un col para forzar una recogida
        /// </summary>
        /// <param name="idCol"></param>        
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMensajerosParaForzarRecogida/{Ubicaciones}")]
        public List<RGDispositivoMensajeroDC> ObtenerMensajerosForzarRecogida([FromUri]string ubicaciones)
        {
            return ApiRecogidas.Instancia.ObtenerMensajerosForzarRecogida(ubicaciones);
        }

        /// <summary>
        /// Obtine las posiciones de dispositivos cercanos a la coordenada cercana
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMensajerosCercanos/{latitud}/{longitud}")]
        public List<RGDispositivoMensajeroDC> ObtenerMensajerosCercanos([FromUri]decimal latitud, [FromUri]decimal longitud)
        {
            return ApiRecogidas.Instancia.ObtenerMensajerosCercanos(latitud, longitud);
        }



        [HttpGet]
        [Route("ObtenerDetalleSolRecogida/{idSolicitudRecogida}/{claseSolicitud}")]
        public RGDetalleSolicitudRecogidaDC ObtenerDetalleSolRecogida([FromUri]long idSolicitudRecogida, [FromUri]RGEnumClaseSolicitud claseSolicitud)
        {
            return ApiRecogidas.Instancia.ObtenerDetalleSolRecogida(idSolicitudRecogida, claseSolicitud);
        }

        /// <summary>
        /// Inserta una gestion de telemercadeo
        /// </summary>
        /// <param name="telemercadeo"></param>
        [HttpPost]
        [Route("InsertarTelemercadeo")]
        [SeguridadWebApi]
        public void InsertarTelemercadeo(RGTelemercadeo telemercadeo)
        {
            ApiRecogidas.Instancia.InsertarTelemercadeo(telemercadeo);
        }

        /// <summary>
        /// Obtiene la informacion de los mensajeros de la localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMensajerosLocalidad/{idLocalidad}")]
        public List<RGMensajeroLocalidadDC> ObtenerMensajerosLocalidad(string idLocalidad)
        {
            return ApiRecogidas.Instancia.ObtenerMensajerosLocalidad(idLocalidad);
        }

        /// <summary>
        /// registra el forzado de un recogida esporadica
        /// </summary>
        /// <param name="solicitud"></param>
        [HttpPost]
        [Route("AsignarRecogida")]
        [SeguridadWebApi]
        public void AsignarRecogida(RGAsignarRecogidaDC solicitud)
        {
            ApiRecogidas.Instancia.AsignarRecogida(solicitud);
        }

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertarRecogidaEsporadica")]
        [SeguridadWebApi]
        public long InsertarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            return ApiRecogidas.Instancia.InsertarRecogidaEsporadica(recogida);
        }

        /// <summary>
        /// Inserta las recogidas esporadicas IVR
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertarRecogidaEsporadicaIVR")]
        [SeguridadWebApi]
        public long InsertarRecogidaEsporadicaIVR(RecogidaIVRRequest recogida)
        {
            return ApiRecogidas.Instancia.InsertarRecogidaEsporadicaIVR(recogida);
        }

        /// <summary>
        /// Modificar detalle de la solicitud Recogida Esporadica
        /// </summary>
        /// <param name="recogida"></param>
        [HttpPost]
        [Route("ModificarRecogidaEsporadica")]
        [SeguridadWebApi]
        public void ModificarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            ApiRecogidas.Instancia.ModificarRecogidaEsporadica(recogida);
        }

        #endregion

        #region Editar
        /// <summary>
        /// Edita las recogidas asignadas a un empleado con el fin de asignarselas a un nuevo empleado
        /// </summary>
        /// <param name="programacion"></param>
        [HttpPost]
        [Route("EditarProgramacionRecogidasFijas")]
        [SeguridadWebApi]
        public void EditarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            ApiRecogidas.Instancia.EditarProgramacionRecogidasFijas(programacion);
        }

        [HttpPost]
        [Route("ModificarCoordenadasRecogidaEsporadica")]
        [SeguridadWebApi]
        public void ModificarCoordenadasRecogidaEsporadica(RGRecogidaEsporadicaDC recogida)
        {
            ApiRecogidas.Instancia.ModificarCoordenadasRecogidaEsporadica(recogida);
        }
        #endregion

        #region Eliminar

        #endregion

        #region Recogidas Controller App

        /// <summary>
        /// Metodo para obtener las recogidas disponibles 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerRecogidasDisponibles/{idLocalidad}")]
        public List<RGRecogidasDC> ObtenerRecogidasDisponibles([FromUri] string idLocalidad)
        {
            return ApiRecogidas.Instancia.ObtenerRecogidasDisponibles(idLocalidad);
        }

        /// <summary>
        /// Metodo para obtener las recogidas reservadas para el mensajero
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerRecogidasReservadasMensajero/{numIdentificacion}")]
        public List<RGRecogidasDC> ObtenerRecogidasReservadasMensajero(string numIdentificacion)
        {
            return ApiRecogidas.Instancia.ObtenerRecogidasReservadasMensajero(numIdentificacion);
        }

        /// <summary>
        /// Metodo para obtener las recogidas efectivas por mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRecogidasEfectivasMensajero/{numIdentificacion}")]
        public List<RGRecogidasDC> ObtenerRecogidasEfectivasMensajero(string numIdentificacion)
        {
            return ApiRecogidas.Instancia.ObtenerRecogidasEfectivasMensajero(numIdentificacion);
        }

        /// <summary>
        /// Metodo para finalizar recogida (esporádica o fija)
        /// </summary>
        /// <param name="asignarRecogida"></param>
        [HttpPost]
        [SeguridadWebApi]
        [Route("EjecutarRecogida")]
        public void EjecutarRecogida([FromBody] RecogidaRequest recogida)
        {
            ApiRecogidas.Instancia.EjecutarRecogida(recogida);
        }
        #endregion


        #region API

        [HttpGet]
        [Route("ObtenerNombreyDireccionCliente")]
        // [SeguridadWebApi]
        public string ObtenerNombreyDireccionCliente([FromUri]string telefono)
        {
            return ApiRecogidas.Instancia.ObtenerNombreyDireccionCliente(telefono);
        }

        /*
        /// <summary>
        /// consulta direcciones historico recogidas por documento
        /// </summary>
        /// <param name="SolicitudRecogidaPeaton"></param>
        [HttpPost]
        [Route("ObtenerDireccionesPeaton")]
        [SeguridadWebApi]
        public List<SolicitudRecogidaRequest> ObtenerDireccionesPeaton([FromBody]SolicitudRecogidaPeaton Peaton)
        {
            return ApiRecogidas.Instancia.ObtenerDireccionesPeaton(Peaton);
        }
        */
        #endregion

        #region Notificaciones
        /// <summary>
        /// Metodo para finalizar recogida (esporádica o fija)
        /// </summary>
        /// <param name="asignarRecogida"></param>
        [HttpPost]
        [SeguridadWebApi]
        [Route("NotificarAdministradores")]
        public void NotificarAdministradores([FromBody] ParametrosSignalR notificacion)
        {
            ApiRecogidas.Instancia.NotificarAdministradores(notificacion);
        }
        #endregion
    }
}