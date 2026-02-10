using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    public interface IRGRecogidasSvc
    {

        /// <summary>
        /// Consulta las cantidad de recogidas fijas pendientes de asignar mensajero
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        long ObtenerCantidadFijasPorAsignar(string idCentroServicio);

        /// <summary>
        /// Obtiene las recogidas que no se han asignado
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <param name="idCol"></param>
        /// <param name="idClienteCredito"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGRecogidasDC> ObtenerRecogidasPorAsignar(string idCiudad, string idCol, int? idClienteCredito, long? idCentroServicio, int numeroPagina, int tamanioPagina);
        /// <summary>
        /// obtiene la lista de clientes credito
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGRecogidasDC> ObtenerClientesCredito(string idCol);

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGEmpleadoDC> ObtenerEmpleadosParaAsignarRecogidas(string idLocalidad);
        /// <summary>
        /// Obtiene todos los datos de un empleado por su cedula
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RGEmpleadoDC ObtenerDatosDeEmpleadoPorCedula(string idEmpleado);

        /// <summary>
        ///  Obtener Motivos Estado Solicitud Recogida X Actor
        /// </summary>
        /// <param name="idActor"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGMotivoEstadoSolRecogidaDC> ObtenerMotivoEstadoSolRecogidaXActor(long idActor);

        /// <summary>
        /// Gestionar Con Motivo estado Solicitud Recogida
        /// </summary>
        /// <param name="solicitud"></param>
        /// <param name="idMotivo"></param>
        /// <param name="idActor"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CancelarConMotivoSolRecogida(RGAsignarRecogidaDC solicitud, int idMotivo, int idActor);
        /// <summary>
        /// obtiene todos los centros de serivcio que pertenezcan a determinado col y a determinada ciudad
        /// </summary>
        /// <param name="idCol"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGRecogidasDC> ObtenerCentrosDeServicio(string idCol, string idCiudad);

        /// <summary>
        /// Consulta las territoriales de los centros de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGTerritorialDC> ObtenerTerritoriales(string idCentroServicio);

        /// <summary>
        /// Consulta agencias de las territoriales de los centros de servicios
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGAgenciaDC> ObtenerAgencias();

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona sin importar que esten activos en la empresa
        /// </summary>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RGEmpleadoDC> ObtenerEmpleadosParaReAsignarRecogidas(string idLocalidad);

        /// <summary>
        /// Inserta la programacion de recogidas fijas a un mensajero
        /// </summary>
        void InsertarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion);

        /// <summary>
        /// Inserta una gestion de telemercadeo
        /// </summary>
        /// <param name="telemercadeo"></param>
        [operationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarTelemercadeo(RGTelemercadeo telemercadeo);

        /// <summary>
        /// registra el forzado de un recogida esporadica
        /// </summary>
        /// <param name="solicitud"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AsignarRecogida(RGAsignarRecogidaDC solicitud);

        /// <summary>
        /// Consulta el detalle de una solicitud de recogida basada en la clase de solicitud
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="claseSolicitud"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RGDetalleSolicitudRecogidaDC ObtenerDetalleSolrecogida(long idSolicitudRecogida, RGEnumClaseSolicitud claseSolicitud);

        /// <summary>
        /// Obtienes los registros a mostrar en el tablero de administracion de  solicitud de recogidas y generar los conteos
        /// </summary>
        /// <param name="idCentroservicio"></param>
        /// <param name="fechaConteo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        RGDetalleyConteoRecogidasDC ConsultaDetConteosAdminRecogidas(string idCentroservicio, DateTime fechaConteo, long documento, string municipio);



        /// <summary>
        /// Obtine las posiciones de dispositivos cercanos a la coordenada cercana
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        List<RGDispositivoMensajeroDC> ObtenerMensajerosCercanos(decimal latitud, decimal longitud);

        /// <summary>
        /// Obtiene la informacion de los mensajeros de la localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        List<RGMensajeroLocalidadDC> ObtenerMensajerosLocalidad(string idLocalidad);

        #region Recogidas Controller App
        /// <summary>
        /// Metodo para obtener recogidas disponibles 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        List<RGRecogidasDC> ObtenerRecogidasDisponibles(string idLocalidad);

        /// <summary>
        /// Metodo para obtener las recogidas reservadas por mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        List<RGRecogidasDC> ObtenerRecogidasReservadasMensajero(string numIdentificacion);
        #endregion

        /// <summary>
        /// Obtiene las recogidas programadas a cierto empleado
        /// </summary>
        /// <returns></returns>
        List<RGRecogidasDC> ObtenerRecogidasPorEmpleado(string idEmpleado);

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        long InsertarRecogidaEsporadica(RGRecogidasDC recogida);

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento);

        /// <summary>
        /// Metodo para finalizar la recogida (esporadica o fija)
        /// </summary>
        /// <param name="recogida"></param>
        void EjecutarRecogida(RGAsignarRecogidaDC recogida, int idSistema, int tipoNovedad, Dictionary<string, object> parametros, string idCiudad);

        /// <summary>
        /// metodo para obtener las recogidas efectivas del mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        List<RGRecogidasDC> ObtenerRecogidasEfectivasMensajero(string numIdentificacion);

        void ModificarCoordenadasRecogidaEsporadica(RGRecogidaEsporadicaDC recogida);
    }

}
