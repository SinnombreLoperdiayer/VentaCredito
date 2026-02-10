using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IONOperacionNacionalSvc
    {
        /// <summary>
        /// Obtiene  los manifiestos de la operacion nacional
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCentroServiciosManifiesta">Centro de servicios que manifiesta</param>
        /// <returns>Lista con los vehiculos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONManifiestoOperacionNacional> ObtenerManifiestosOperacionNacional(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta, bool incluyeFecha);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_ConNovRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_Disponibles(long idCentroServicios, int idRuta, DateTime fechaInicial, DateTime fechaFinal);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONNovedadEstacionRutaDC> Obtener_NovedadesEstacionRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONManifiestoOperacionNacional Obtener_ManifiestoxId(long IdManifiesto);

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de orgen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad);

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta);


        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol);

        /// <summary>
        /// Obtiene una lista de los vehiculos activos que pertenecen a un racol y que tienen ingreso a un col
        /// </summary>
        /// <param name="idRacol">Id del racol al que pertenece el vehiculo</param>
        /// <param name="idCentroServicios">Id del centro de servicios al que ingreso el vehiculo</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POVehiculo> ObtenerVehiculosIngresoCentroServicioXRacol(long idRacol, long idCentroServicios, bool esCol);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarNovedadEstacionRutaPorGuia(List<ONNovedadEstacionRutaxGuiaDC> lstNovedadesEstacioRuta);

        /// <summary>
        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo);

        /// <summary>
        /// Inserta, actualiza un manifiesto nacional
        /// </summary>
        /// <param name="manifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONManifiestoOperacionNacional ActualizarManifiestoOperacionNacional(ONManifiestoOperacionNacional manifiesto);

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta);

        /// <summary>
        /// Obtiene las estaciones-ruta de un Manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long idManifiesto);


        /// <summary>
        /// Obtiene los consolidados de un manifiesto de la operacion nacional
        /// </summary>
        /// <param name="idManifiestoOperacionNacional">Identificador del manifiesto de la operacion nacional</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<ONConsolidado> ObtenerConsolidadosManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idManifiestoOperacionNacional, string idLocalidad);

        /// <summary>
        /// Obtiene todos los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONTipoConsolidado> ObtenerTipoConsolidado();


        /// <summary>
        /// Obtiene todos los tipos Novedades de Estacion-Ruta
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONTipoNovedadEstacionRutaDC> ObtenerTiposNovedadEstacionRuta();


        /// <summary>
        /// Obtiene todos los detalles de los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONTipoConsolidadoDetalle> ObtenerTipoConsolidadoDetalle(int idTipoConsolidado);

        /// <summary>
        /// Inserta un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONConsolidado AdicionarConsolidado(ONConsolidado consolidado, long idCentroServicios);


        /// <summary>
        /// Inserta una Novedad de Estacion-Ruta
        /// </summary>
        /// <param name="Lista de Novedades"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarNovedadEstacionRuta(List<ONNovedadEstacionRutaDC> lstNovedadesEstacioRuta);


        /// <summary>
        /// Modifica un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarConsolidado(ONConsolidado consolidado, long idCentroServicios);

        /// <summary>
        /// Inserta un envio al consolidado
        /// </summary>
        /// <param name="consolidadoDetalle"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONConsolidadoDetalle AdicionarConsolidadoDetalle(ONConsolidadoDetalle consolidadoDetalle);

        /// <summary>
        /// Obtiene las guias de un consolidado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONConsolidadoDetalle> ObtenerGuiasConsolidado(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idConsolidado);

        /// <summary>
        /// Obtiene todas las guias manifestadas, incluye guias sueltas y guias consolidadas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONManifiestoGuia> ObtenerTodasGuiasManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional);

        /// <summary>
        /// Inserta una guia suelta a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONManifiestoGuia AdicionarGuiaSuelta(ONManifiestoGuia guia);

        /// Obtiene los consolidados y las guias sueltas para la impresion del manifiesto por ruta
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONImpresionManifiestoDC> ObtenerImpresionManifiestoRuta(long idManifiesto, List<string> ciudadesAManifestar);

        /// <summary>
        /// Obtiene todas las guias sueltas de un manifiesto nacional
        /// </summary>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONManifiestoGuia> ObtenerGuiasSueltas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idManifiestoOpeNacional);

        /// <summary>
        /// Obtiene todos los motivos de eliminacion de una guia
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONTipoMotivoElimGuiaMani> ObtenerTodosMotivosEliminacionGuia();

        /// <summary>
        /// Elimina una guia suelta de un manifiesto o una guia de un consolidado de un manifiesto
        /// </summary>
        /// <param name="motivo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarGuiaManifiesto(ONMotivoElimGuiaMani motivo);

        /// <summary>
        /// Valida el vehiculo seleccionado para poder hacer el ingreso a col
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidacionVehiculoIngreso(ONIngresoOperativoDC ingreso);

        /// <summary>
        /// Obtiene los envios consolidados de los manifiestos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONManifiestoOperacionNacional> ObtenerEnviosConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Cierra un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarManifiesto(long idManifiesto, DateTime fechaSalida);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ReportarSatrack(INDatosEnvioSatrack infoSatrack);

        /// <summary>
        /// Reabre un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AbrirManifiesto(ONManifiestoOperacionNacional manifiesto);

        /// <summary>
        /// Obtiene los consolidados de los manifiestos abiertos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidado> ObtenerConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Obtiene el ingreso o salida del transportador por los parametros de entrada
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="idRuta"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="identificadorConductor"></param>
        /// <param name="nombreConductor"></param>
        /// <param name="incluyeFecha"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<ONIngresoSalidaTransportadorDC> ObtenerIngresoSalidaTransportador(ONFiltroTransportadorDC filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha);

        /// <summary>
        /// Obtiene las rutas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad);

        /// <summary>
        /// Obtiene los estados del empaque para el parametro de peso especificado
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaqueParametroPeso(string idParametro);

        /// <summary>
        /// Obtiene todos los conductores Activos
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POConductores> ObtenerTodosConductores();

        /// <summary>
        /// Guarda el ingreso a la agencia del vehiculo del operativo
        /// </summary>
        /// <param name="ingresoOperativo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GuardarIngresoAgenciaRuta(ONIngresoOperativoDC ingresoOperativo);

        /// <summary>
        /// Guarda la novedad del consolidado
        /// </summary>
        /// <param name="idConsolidado">id del consolidado </param>
        /// <param name="descripcion">Descripcion de la novedad</param>
        /// <param name="numeroPrecintoIngreso">numero del precinto de ingreso</param>
        /// <param name="numeroTulaContenedor">número de tula o contendor de ingreso</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarNovedadConsolidado(ONConsolidado consolidado, string descripcion);

        /// <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONIngresoOperativoDC GuardaIngresoEnvioAgencia(ONIngresoOperativoDC ingreso);

        /// <summary>
        /// Obtiene la informacion de ingreso y salida del transportador por ID
        /// </summary>
        /// <param name="idIngrsoSalidaTransportado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONIngresoSalidaTransportadorDC ObtenerIngresoSalidaTransportadorPorId(long idIngrsoSalidaTransportado);

        /// <summary>
        /// Retorna los envios del consolidado seleccionado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidadoDetalle> ObtenerEnviosConsolidado(long idConsolidado);

        /// <summary>
        /// Obtiene todas las novedades operativas
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONNovedadOperativoDC> ObtenerNovedadOperativo();

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado o no
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta);

        /// <summary>
        /// Ingreso en la tabla de ingreso salida transportador
        /// </summary>
        /// <param name="ingresoSalidaTrans"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void IngresarIngresosSalidasTrasnportador(ONIngresoSalidaTransportadorDC ingresoSalidaTrans);

        /// <summary>
        /// Consultar la última ruta de un vehículo que ha sido manifestado,
        /// el vehículo se consulta por su placa
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONRutaConductorDC ObtenerUltimaRutaConduPlaca(string placa);

        /// <summary>
        /// Obtiene los manifiestos sin descargar de un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONManifiestoOperacionNacional> ObtenerManifiestosVehiculo(int idVehiculo);

        /// <summary>
        /// Obtiene los envios sueltos del manifiesto
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <param name="idIngresoOperativo">id del ingreso a la agencia del vehiculo</param>
        /// <param name="estaDescargada">bit para saber si esta descargado o no el manifiesto</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize">tamaño de la pagina</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidadoDetalle> ObtenerEnviosSueltosManifiestoVehiculo(int idVehiculo, int idRuta, long idIngresoOperativo, int pageIndex, int pageSize);

        /// <summary>
        /// Metodo para Obtener las
        /// Regionales Administrativas
        /// </summary>
        /// <returns>lista de RACOL</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PURegionalAdministrativa> ObtenerRegionalAdministrativa();

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol);

        /// <summary>
        /// Obtiene el total de los envios manifestados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerTotalEnviosManifestadosVehiculoLocalidad(int idVehiculo, string idLocalidadDestino);

        /// <summary>
        /// Obtiene el total de los envios sobrantes
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerTotalEnviosSobrantesVehiculoLocalidad(long idOperativo, string idLocalidadDestino);

        /// <summary>
        /// Obtiene el total de los envios descargados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerTotalEnviosDescargadosVehiculoLocalidad(long idOperativo, string idLocalidadDestino);

        /// <summary>
        /// Guarda y valida los envios transito
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        /// <param name="ingreso"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardaIngresoEnviosTransito(ONIngresoOperativoDC ingreso);

        /// <summary>
        /// Obtener los consolidados a partir de la guia interna
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONConsolidado ObtenerConsolidadoPorIdGuia(long idGuiaInterna);

        /// <summary>
        /// Cierra el ingreso del operativo para el vehiculo
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarIngresoOperativoAgencia(ONIngresoOperativoDC ingreso);

        /// <summary>
        /// Consulta todos los envios de un consolidado
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idIngresoOperativo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidadoDetalle> ObtenerTodosEnviosSueltosVehiculo(int idVehiculo, int idRuta);

        /// <summary>
        /// Consulta todos los envios consolidados del manifiesto
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidadoDetalle> ObtenerTodosEnviosConsolidadoManifiesto(int idVehiculo, int idRuta);

        // <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONIngresoOperativoCiudadDC GuardaIngresoEnvioOperativoCiudad(ONIngresoOperativoCiudadDC ingreso);

        /// <summary>
        /// Guarda  el ingreso al operativo por ciudad
        /// </summary>
        /// <param name="ingreso"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GuardarOperativoAgenciaCiudad(ONIngresoOperativoCiudadDC ingreso);

        /// <summary>
        /// Obtiene los manifietos donde la ciudad origen, destino o estacion de ruta sea la ciudad
        /// donde se esta haciendo el descargue
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONManifiestoOperacionNacional> ObtenerManifiestosXLocalidad(IDictionary<string, string> filtro, string idLocalidad, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Consulta el detalle del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONIngresoOperativoDC ObtenerDetalleManifiesto(ONIngresoOperativoDC ingreso);

        /// <summary>
        /// Obtiene los envios de un manifiesto por numero de manifesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidado> ObtenerEnviosXManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Genera el reporte del manifiesto de carga
        /// </summary>
        /// <param name="idManifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONReporteManifiestoCarga GenerarReporteManifiestoCarga(long idManifiesto, long IdCentroServiciosManifiesta);

        /// <summary>
        /// Retorna o asigan los consolidados del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidado> ObtenerDetalleConsolidadosManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Consulta las estaciones de una ruta y la cantidad de envíos manifestados en un manifiesto específico x cada estación
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONCantEnviosManXEstacionDC> ConsultarCantEnviosManXEstacion(int idRuta, long idManifiesto);

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONEnviosDescargueRutaDC> ObtenerEnviosConsolidadoXManifiesto(long idManifiesto);

        /// <summary>
        /// Obtiene los envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONEnviosDescargueRutaDC> ObtenerEnviosSueltosXManifiesto(long idManifiesto);

        /// <summary>
        /// Obtiene los totales de manifiesto, total sobrantes, total faltantes
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="idOperativo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONCierreDescargueManifiestoDC ObtenerTotalCierreManifiesto(long idManifiesto, long idOperativo);

        /// <summary>
        /// Cierra el ingreso del operativo para la ciudad
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void TerminaDescargueOperativo(long idIngresoOperativo);

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estación
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion);

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion);

        /// <summary>
        /// obtiene una lista de guias en el centro de acopio, que debieron haber sido agregadas al manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerEnviosPendientesPorManifestar(ONManifiestoOperacionNacional manifiesto);
        /// <summary>
        /// para obtener parametros guias en novedades de ruta cuando se requiera asignar novedad a guias especificas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="guiaxNovIndv"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONParametrosGuiaInvNovedadRuta> ObtenerParametrosGuiaIndvPorNovedadRuta(long numeroGuia, ONEnumGuiaIndvNovedadRuta guiaxNovIndv, string tipoUbicacion);

        /// <summary>
        /// Obtiene Novedades de Transporte de la guia seleccionada
        /// </summary>
        /// <param name="numeroGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia);

        // Se revisa si el Casillero es AEREO (Ciudades de la Costa en la Tabla TrayectoCasilleroServicio_OPN)
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarTrayectoServicioAereo(string idLocalidadOrigen, string idLocalidadDestino, int idServicio);

        #region Ingreso centro de acopio nacional

        /// <summary>
        /// Método para ingresar los consolidados en el ingreso a centro de acopio nacional
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONConsolidado> IngresarManifiestoConsolidado(long controlTrans, long noPrecinto, string noConsolidado, long idCentroServicio);

        /// <summary>
        ///  Método para ingresar una guía suelta a centro de acopio nacional
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONEnviosDescargueRutaDC IngresarGuiaSuelta(ONEnviosDescargueRutaDC guia, List<OUNovedadIngresoDC> listaNovedades);

        #endregion Ingreso centro de acopio nacional

        #region Descargue Consolidados

        /// <summary>
        /// Obtiene los Consolidados de Guias para el descarge Urbano - Regional - Nacional
        /// </summary>
        /// <param name="ingresoConsolidado">info a consultar</param>
        /// <returns>info de las guias descargadas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONDescargueConsolidadosUrbRegNalDC ObtenerIngresoConsolidado(ONDescargueConsolidadosUrbRegNalDC ingresoConsolidado);

        /// <summary>
        /// Obtiene las novedades delos ingresos de la Guia
        /// </summary>
        /// <returns>lista de Novedades de Ingresos Guia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONNovedadesEnvioDC> ObtenerNovedadesIngresosGuia();

        /// <summary>
        ///Obtiene las novedades de Descargue Consolidado
        /// </summary>
        /// <returns>lista de Novedades descargue Consolidados</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueConsolidado();

        /// <summary>
        /// Obtiene la info inicial de la pantalla de Descargue Consolidados
        /// </summary>
        /// <returns>Listas de info Inicial pantalla</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ONDescargueConsolidadosInfoInicialDC ObtenerInfoInicialDescargueConsolidados();

        /// <summary>
        /// Adiciona el Proceso de Descargue Consolidado
        /// </summary>
        /// <param name="nuevoDescargue"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool AdicionarProcesoDescargueConsolidado(ONDescargueConsolidadosUrbRegNalDC nuevoDescargue);

        #endregion Descargue Consolidados

        #region TrayectoCasillero

        /// <summary>
        /// Método para obtener los rangos de peso de los casilleros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONRangoPesoCasilleroDC> ObtenerRangosPesoCasillero();


        /// <summary>
        /// Método para obtener los trayectos de un origen 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONTrayectoCasilleroPesoDC> ObtenerTrayectosCasilleroDestino(PALocalidadDC localidadOrigen);


        /// <summary>
        /// Método para guardar los cambios de  los trayectos casillero
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ONTrayectoCasilleroPesoDC> GuardarTrayectoCasillero(List<ONTrayectoCasilleroPesoDC> ListaTrayectosCasillero);



        #endregion


        #region PruebaCarga


        /// <summary>
        /// Prueba de carga para el grafo de rutas
        /// </summary>
        /// <param name="IdCiudadDestino"></param>
        /// <param name="nombreCiudadDestino"></param>
        /// <param name="idCiudadManifiesta"></param>
        /// <param name="nombreciudadManifiesta"></param>
        /// <param name="idRuta"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Dictionary<string, string> CalcularRutaOptimaOmitiendoCiudadesPruebaCargaGrafo(string IdCiudadDestino, string nombreCiudadDestino, string idCiudadManifiesta, string nombreciudadManifiesta, int idRuta);

        #endregion


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ONHorarioRutaDC> ConsultarHorariosRuta(int idRuta);
    }
}