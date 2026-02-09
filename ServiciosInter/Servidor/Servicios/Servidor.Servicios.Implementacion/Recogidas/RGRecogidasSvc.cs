using CO.Servidor.Recogidas;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CO.Servidor.Servicios.Implementacion.Recogidas
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RGRecogidasSvc : IRGRecogidasSvc
    {
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
            return RGRecogidas.Instancia.ObtenerRecogidasPorAsignar(idCiudad, idCol, idClienteCredito, idCentroServicio, numeroPagina, tamanioPagina);
        }

        /// <summary>
        /// obtiene la lista de clientes credito
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerClientesCredito(string idCol)
        {
            return RGRecogidas.Instancia.ObtenerClientesCredito(idCol);
        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaAsignarRecogidas(string idLocalidad)
        {
            return RGRecogidas.Instancia.ObtenerEmpleadosParaAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// Consulta las cantidad de recogidas fijas pendientes de asignar mensajero
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public long ObtenerCantidadFijasPorAsignar(string idCentroServicio)
        {
            return RGRecogidas.Instancia.ObtenerCantidadFijasPorAsignar(idCentroServicio);
        }

        /// <summary>
        /// Obtiene todos los datos de un empleado por su cedula
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public RGEmpleadoDC ObtenerDatosDeEmpleadoPorCedula(string idEmpleado)
        {
            return RGRecogidas.Instancia.ObtenerDatosDeEmpleadoPorCedula(idEmpleado);
        }

        /// <summary>
        ///  Obtener Motivos Estado Solicitud Recogida X Actor
        /// </summary>
        /// <param name="idActor"></param>
        /// <returns></returns>
        public List<RGMotivoEstadoSolRecogidaDC> ObtenerMotivoEstadoSolRecogidaXActor(long idActor)
        {
            return RGRecogidas.Instancia.ObtenerMotivoEstadoSolRecogidaXActor(idActor);
        }

        /// <summary>
        /// Gestionar Con Motivo estado Solicitud Recogida
        /// </summary>
        /// <param name="solicitud"></param>
        /// <param name="idMotivo"></param>
        /// <param name="idActor"></param>
        public void CancelarConMotivoSolRecogida(RGAsignarRecogidaDC solicitud, int idMotivo, int idActor)
        {
            RGRecogidas.Instancia.CancelarConMotivoSolRecogida(solicitud, idMotivo, idActor);
        }

        /// <summary>
        /// obtiene todos los centros de serivcio que pertenezcan a determinado col y a determinada ciudad
        /// </summary>
        /// <param name="idCol"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerCentrosDeServicio(string idCol, string idCiudad)
        {
            return RGRecogidas.Instancia.ObtenerCentrosDeServicio(idCol, idCiudad);
        }

        /// <summary>
        /// Inserta la programacion de recogidas fijas a un mensajero
        /// </summary>
        public void InsertarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            RGRecogidas.Instancia.InsertarProgramacionRecogidasFijas(programacion);
        }

        /// <summary>
        /// Consulta las territoriales de los centros de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<RGTerritorialDC> ObtenerTerritoriales(string idCentroServicio)
        {
            return RGRecogidas.Instancia.ObtenerTerritoriales(idCentroServicio);
        }

        /// <summary>
        /// Consulta agencias de las territoriales de los centros de servicios
        /// </summary>
        /// <returns></returns>
        public List<RGAgenciaDC> ObtenerAgencias()
        {
            return RGRecogidas.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona sin importar que esten activos en la empresa
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaReAsignarRecogidas(string idLocalidad)
        {
            return RGRecogidas.Instancia.ObtenerEmpleadosParaReAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// Consulta el detalle de una solicitud de recogida basada en la clase de solicitud
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="claseSolicitud"></param>
        /// <returns></returns>
        public RGDetalleSolicitudRecogidaDC ObtenerDetalleSolrecogida(long idSolicitudRecogida, RGEnumClaseSolicitud claseSolicitud)
        {
            return RGRecogidas.Instancia.ObtenerDetalleSolRecogida(idSolicitudRecogida, claseSolicitud);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ObtenerHorarioRecogida(RGRecogidasDC datosUbicacion)
        {
            return RGRecogidas.Instancia.ObtenerHorarioRecogida(datosUbicacion);
        }

        public RGEstadisticas ObtenerEstadisticas(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return RGRecogidas.Instancia.ObtenerEstadisticas(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerMensajerosActivosPorAplicacion(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return RGRecogidas.Instancia.ObtenerMensajerosActivosAplicacion(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerPosicionesMensajerosActivos(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return RGRecogidas.Instancia.ObtenerPosicionesMensajerosActivos(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public RGDetalleMensajeroBalance ObtenerDetalleMensajeroPorId(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idMensajero)
        {
            return RGRecogidas.Instancia.ObtenerDetalleMensajeroPorId(fechaInicial, fechaFinal, idMensajero);
        }

        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            return RGRecogidas.Instancia.ObtenerMisRecogidasClientePeaton(idUsuario);
        }

        /// <summary>
        /// Obtiene las recogidas programadas a cierto empleado
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasPorEmpleado(string idEmpleado)
        {
            return RGRecogidas.Instancia.ObtenerRecogidasPorEmpleado(idEmpleado);
        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        public RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento)
        {
            return RGRecogidas.Instancia.ObtenerUltimaSolicitud(numeroDocumento);
        }

        /// <summary>
        /// Edita las recogidas asignadas a un empleado con el fin de asignarselas a un nuevo empleado
        /// </summary>
        /// <param name="programacion"></param>
        public void EditarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            RGRecogidas.Instancia.EditarProgramacionRecogidasFijas(programacion);
        }



        /// <summary>
        /// registra el forzado de un recogida esporadica
        /// </summary>
        /// <param name="solicitud"></param>
        public void AsignarRecogida(RGAsignarRecogidaDC solicitud)
        {
            RGRecogidas.Instancia.AsignarRecogida(solicitud);
        }

        /// <summary>
        /// Obtienes los registros a mostrar en el tablero de administracion de  solicitud de recogidas y generar los conteos
        /// </summary>
        /// <param name="idCentroservicio"></param>
        /// <param name="fechaConteo"></param>
        /// <returns></returns>
        public RGDetalleyConteoRecogidasDC ConsultaDetConteosAdminRecogidas(string idCentroservicio, DateTime fechaConteo, long documento, string municipio)
        {
            return RGRecogidas.Instancia.ConsultaDetConteosRecogidas(idCentroservicio, fechaConteo, documento, municipio);
        }

        /// <summary>
        /// Obtine las posiciones de dispositivos cercanos a la coordenada cercana
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        public List<RGDispositivoMensajeroDC> ObtenerMensajerosCercanos(decimal latitud, decimal longitud)
        {
            return RGRecogidas.Instancia.ObtenerMensajerosCercanos(latitud, longitud);
        }

        public List<RGDispositivoMensajeroDC> ObtenerMensajerosForzarRecogida(string ubicaciones)
        {
            return RGRecogidas.Instancia.ObtenerMensajerosForzarRecogida(ubicaciones);
        }

        /// <summary>
        /// Obtiene la informacion de los mensajeros de la localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<RGMensajeroLocalidadDC> ObtenerMensajerosLocalidad(string idLocalidad)
        {
            return RGRecogidas.Instancia.ObtenerMensajerosLocalidad(idLocalidad);
        }

        /// <summary>
        /// inserta un cambio de estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        public void InsertarEstadoSolRecogidaTraza(RGAsignarRecogidaDC solicitud, EnumEstadoSolicitudRecogida nuevoEstado)
        {
            RGRecogidas.Instancia.InsertarEstadoSolRecogidaTraza(solicitud, nuevoEstado);
        }

        #region Recogida Controller App
        /// <summary>
        /// Metodo para obtener las recogidas disponibles por mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasDisponibles(string idLocalidad)
        {
            return RGRecogidas.Instancia.ObtenerRecogidasDisponibles(idLocalidad);
        }
        /// <summary>
        /// Metodo para obtener las recogidas reservadas por mensajero
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasReservadasMensajero(string numIdentificacion)
        {
            return RGRecogidas.Instancia.ObtenerRecogidasReservadasMensajero(numIdentificacion);
        }

        /// <summary>
        /// metodo para obtener las recogidas efectivas del mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasEfectivasMensajero(string numIdentificacion)
        {
            return RGRecogidas.Instancia.ObtenerRecogidasEfectivasMensajero(numIdentificacion);
        }

        #endregion

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            return RGRecogidas.Instancia.InsertarRecogidaEsporadica(recogida);
        }
        /// <summary>
        /// Metodo para finalizar la recogida (esporadica o fija)
        /// </summary>
        /// <param name="recogida"></param>
        public void EjecutarRecogida(RGAsignarRecogidaDC recogida, int idSistema, int tipoNovedad, Dictionary<string, object> parametros, string idCiudad)
        {
            RGRecogidas.Instancia.EjecutarRecogida(recogida, idSistema, tipoNovedad, parametros, idCiudad);
        }

        #region IVR

        public string ObtenerNombreyDireccionCliente(string telefono)
        {
            return RGRecogidas.Instancia.ObtenerNombreyDireccionCliente(telefono);
        }

        /// <summary>
        /// Modificar detaller solicitud Recogida Esporadica
        /// </summary>
        /// <param name="recogida"></param>
        public void ModificarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            RGRecogidas.Instancia.ModificarRecogidaEsporadica(recogida);
        }

        public void ModificarCoordenadasRecogidaEsporadica(RGRecogidaEsporadicaDC recogida)
        {
             RGRecogidas.Instancia.ModificarCoordenadasRecogidaEsporadica(recogida);
        }

        /// <summary>
        /// Inserta una gestion de telemercadeo
        /// </summary>
        /// <param name="telemercadeo"></param>
        public void InsertarTelemercadeo(RGTelemercadeo telemercadeo)
        {
            RGRecogidas.Instancia.InsertarTelemercadeo(telemercadeo);
        }

        #endregion


    }
}
