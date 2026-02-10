using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ISUSuministrosSvc
    {
        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio);

        /// <summary>
        /// Obtiene los suministros de un grupo
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosAsignadosGrupo(string idGrupo);

        /// <summary>
        /// Obtiene todos los suministros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerTodosSuministros();

        /// <summary>
        /// Retorna las categorias de los suministros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUCategoriaSuministro> ObtenerCategoriasSuministros();

        /// <summary>
        /// Retorna la lista de los suministros confugurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<SUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Realiza la administracion del suministro
        /// </summary>
        /// <param name="suministro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdministrarSuministro(SUSuministro suministro);

        /// <summary>
        /// Obtener los suministros activos y que sean para preimprimir
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosPreImpresion();

        /// <summary>
        ///
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosSucursal(int idSucursal);

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro);

        #region Aprovisionar suministros de un Centro de servicio

        /// <summary>
        /// Obtener suministros de grupo de un centro de servicio
        /// </summary>
        /// <param name="idGrupo">CSV -PUA -racol</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosGrupoCentroServicio(string idGrupo, long idCentroServicio);

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en el centro de servicio
        /// </summary>
        /// <param name="filtro">codigo erp(novasoft) y descripcion del suministro</param>
        /// <param name="idGrupo">id del grupo</param>
        /// <param name="idCentroServicio">id del centro de servicio</param>
        /// <returns>Lista de suministro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idCentroServicio);

        #endregion Aprovisionar suministros de un Centro de servicio

        #region ResolucionSuministro

        /// <summary>
        /// Retorna las resoluciones de suministros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<SUNumeradorPrefijo> ObtenerResolucionesSuministro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Administra la resulucion del suministro
        /// </summary>
        /// <param name="numerador"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdministrarResolucion(SUNumeradorPrefijo numerador);

        /// <summary>
        /// Obtiene los suministros q aplican resolucion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosResolucion();

        #endregion ResolucionSuministro

        #region Grupo suministros

        /// <summary>
        /// Obtiene los grupos de suministros configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Administra el grupo de suministros
        /// </summary>
        /// <param name="grupo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdministrarGrupoSuministros(SUGrupoSuministrosDC grupo);

        /// <summary>
        /// Obtener suministros de grupo
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosGrupo(string idGrupo, long idMensajero);

        /// <summary>
        /// Obtiene los suministros que no estan incluidos en ningun grupo, ni en en el grupo seleccionado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idMensajero);

        /// <summary>
        /// Obtiene los mensajeros y conductores configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente);

        #endregion Grupo suministros

        #region Suministros Mensajeros

        /// <summary>
        /// Administra la informacion del mensajero
        /// </summary>
        /// <param name="grupoMensajero"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdministrarSuministroMensajero(SUGrupoSuministrosDC grupoMensajero);

        #endregion Suministros Mensajeros

        #region Aprovisionamiento de suministros

        /// <summary>
        /// Obtiene las ultimas remisiones realizadas
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<SURemisionSuministroDC> ObtenerUltimasRemisiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosAsignadosMensajero(long idMensajero);

        /// <summary>
        /// Obtiene los mensajeros por agencia para la asignacion de los suministros
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia);

        /// <summary>
        /// Valida la asignacion del suministro
        /// </summary>
        /// <param name="suministro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidaSuministroAsignacion(SUSuministro suministro);

        /// <summary>
        /// Genera la remision de suministros para el mensajero
        /// </summary>
        /// <param name="remision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdminRemisionMensajero(SURemisionSuministroDC remision);

        #endregion Aprovisionamiento de suministros

        #region Canal de venta

        /// <summary>
        /// Obtiene los suministros asignados al canal de venta
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosAsignadosCanalVenta(long idCentroServicios);

        /// <summary>
        /// Genera la remision de los suministros para el canal de ventas
        /// </summary>
        /// <param name="remision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdminRemisionCanalVenta(SURemisionSuministroDC remision);

        #endregion Canal de venta

        #region Clientes

        /// <summary>
        /// Obtiene los clientes activos con una sucursal activa por localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerClientesConSucursalActiva(string idLocalidad);

        /// <summary>
        /// Obtiene las sucursales activas del cliente seleccionado
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idCliente, string idLocalidad);

        /// <summary>
        /// Obtiene los suministros asignados a la sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosAsignadoSucursal(int idSucursal);

        /// <summary>
        /// Genera la remision de suministros para el mensajero
        /// </summary>
        /// <param name="remision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdminRemisionCliente(SURemisionSuministroDC remision);

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns>lista de contratos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLContratosDC> ObtenerContratosActivosClientes(int idCliente);

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato
        /// </summary>
        /// <param name="idContrato">id del contrato</param>
        /// <returns>lista de Sucursales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato);

        /// <summary>
        /// Obtiene la lista de los contratos de las
        /// sucursales Activas
        /// </summary>
        /// <returns>lista de Contratos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLContratosDC> ObtenerContratosActivosDeSucursales();

        /// <summary>
        /// obtiene el contrato de un cliente en una
        /// ciudad
        /// </summary>
        /// <param name="idCliente">id del cliente</param>
        /// <param name="idCiudad">id de la ciudad</param>
        /// <returns>lista de contratos del cliente en esa ciudad</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLContratosDC> ObtenerContratosClienteCiudad(int idCliente, string idCiudad);

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato y ciudad
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>lista de Sucursales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad);

        #endregion Clientes

        #region desasignacion de suministros

        /// <summary>
        /// Obtiene los grupos de suministros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUGrupoSuministrosDC> ObtenerGruposSuministros();

        /// <summary>
        /// Realiza la desasignacion de suministros de un mensajero
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DesasignarSuministrosRemision(SURemisionSuministroDC remision);

        /// <summary>tener
        /// Retorna los suministros que esten en el rango de fecha seleccionado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SURemisionSuministroDC> ObtenerSuministrosRemisionXRangoFecha(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, string idGrupo);

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en la sucursal
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosSucursalNoIncluidosEnGrupo(string idGrupo, int idSucursal);

        #endregion desasignacion de suministros

        #region PreImpresion de Suministros

        /// <summary>
        /// Obtener las provisiones de un racol si estas tienen rangos consecutivos
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta</param>
        /// <param name="fechaFinal">fecha final de la consulta</param>
        /// <param name="idRacol">Id del RACOL</param>
        /// <param name="idSuministro">id del suministro a consultar</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">Tamaño de la pagina</param>
        /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosPorRACOL(DateTime? fechaInicial, DateTime? fechaFinal, long idRacol, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha);


        /// <summary>
        /// Obtener las provisiones de un racol si estas tienen rangos consecutivos por ciudad destino
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta</param>
        /// <param name="fechaFinal">fecha final de la consulta</param>
        /// <param name="idRacol">Id del RACOL</param>
        /// <param name="idSuministro">id del suministro a consultar</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">Tamaño de la pagina</param>
        /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias por ciudad destino</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha);


        /// <summary>
        /// Obtener las provisiones de un Centro de servicio si estas tienen rangos consecutivos
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta</param>
        /// <param name="fechaFinal">fecha final de la consulta</param>
        /// <param name="idCentroServicio">Id Centro Servicio</param>
        /// <param name="idSuministro">id del suministro a consultar</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">Tamaño de la pagina</param>
        /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUImpresionSuministrosDC ObtenerProvisionesSuministrosPorCentroServicio(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize);

        /// <summary>
        /// Obtiene las provisiones de suministros en los rangos ingresados por parametros
        /// en un rango de fechas
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
        /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
        /// <param name="rangoInicial">rango inicial</param>
        /// <param name="rangoFinal">rango final</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">tamaño de la paginacion</param>
        /// <returns>Lista con la informacion para impresión los suministro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUImpresionSuministrosDC ObtenerProvisionSuministroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango);

        /// <summary>
        /// Obtiene informacion de las provisiones de un suministro por el numero de remision
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
        /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
        /// <param name="usuario">Usuario que realizo la remision</param>
        /// <param name="remisionInicial">numero de la remision inicial</param>
        /// <param name="remisionFinal">numero de la remision final</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">tamaño de la paginacion</param>
        /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision);

        /// <summary>
        /// Obtiene informacion de las provisiones de un suministro por el numero de remision
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
        /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
        /// <param name="usuario">Usuario que realizo la remision</param>
        /// <param name="remisionInicial">numero de la remision inicial</param>
        /// <param name="remisionFinal">numero de la remision final</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">tamaño de la paginacion</param>
        /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SURemisionSuministroDC> ObtenerRemisionesGuiasInternas(SUFiltroSuministroPorRemisionDC filtroPorRemision);

        /// <summary>
        /// Obtiene las provisiones de suministros realizados por un usuario
        /// en un rango de fechas
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
        /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
        /// <param name="rangoInicial">rango inicial</param>
        /// <param name="rangoFinal">rango final</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">tamaño de la paginacion</param>
        /// <returns>Lista con la informacion para impresión los suministro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUImpresionSuministrosDC ObtenerProvisionSuministroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario);

        #endregion PreImpresion de Suministros

        #region Modificacion de suministros

        /// <summary>
        /// Administra la remision del suministro
        /// </summary>
        /// <param name="remisionDestino"></param>
        /// <param name="remisionOrigen"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdministrarModificacionSuministro(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen);

        #endregion Modificacion de suministros

        #region Proceso Suministro

        /// <summary>
        /// Obtiene los grupos de suministros configurados en una gestion
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<SUSuministrosProcesoDC> ObtenerProcesosSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long codProceso);

        /// <summary>
        /// agrega o modifica un suministro de una gestion
        /// </summary>
        /// <param name="sumSuc"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarSuministroProceso(List<SUSuministrosProcesoDC> sumPro);

        /// <summary>
        /// Consulta los parametros de suministros
        /// </summary>
        /// <param name="IdParametro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerParametrosSuministro(string idParametro);

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados a una gestion
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosProcesoNoIncluidosEnGrupo(string idGrupo, long codProceso, Dictionary<string, string> filtro);

        /// <summary>
        /// Obtiene los suministros de un proceso
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministrosProcesoDC> ObtenerSuministrosProceso(long codProceso);

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUSuministro> ObtenerSuministrosAsignadosProceso(long idProceso);

        /// <summary>
        /// Genera la remision de suministros para un proceso
        /// </summary>
        /// <param name="remision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdminRemisionProceso(SURemisionSuministroDC remision);

        #endregion Proceso Suministro

        /// <summary>
        /// Obtiene los Correos a notificar la alerta del fallo en
        /// la sincronización a Novasoft de la salida ó
        /// traslado de suministros asignados desde Controller
        /// </summary>
        /// <returns>Lista de Correos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<SUCorreoNotificacionesSumDC> ObtenerCorreosNotificacionesSuministro();

        /// <summary>
        /// Gestiona los mail a la lista de notificaciones
        /// de sincronizacion de Novasoft para Adicionar ó Borrar
        /// </summary>
        /// <param name="email">Correo a Adicionar</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GestionarCorreoNotificacionSuministro(ObservableCollection<SUCorreoNotificacionesSumDC> correosGestionar);

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUPropietarioGuia ObtenerPropietarioSuministro(long numeroSuministro, SUEnumSuministro idSuministro, long idPropietario);

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUPropietarioGuia ObtenerPropietarioGuia(long numeroSuministro);

        /// <summary>
        /// Retorna el propietario de un suministro sin validar 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUPropietarioGuia ObtenerPropietarioSuministroSinValidar(long numeroSuministro, SUEnumSuministro idSuministro);

                /// <summary>
        /// Genera la remision de los suministros para el canal de ventas
        /// </summary>
        /// <param name="remision"></param>
        ///    [OperationContract]
        [FaultContract(typeof(ControllerException))]
        [OperationContract]
        SURemisionSuministroDC GenerarRangoGuiaManualOffline(long idCentroServicio, int cantidad);

        /// <summary>
        /// Obtiene los datos del responsable del suministro
        /// </summary>
        /// <param name="remision"></param>

        [FaultContract(typeof(ControllerException))]
        [OperationContract]
        SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia);

        #region Consolidados
        /// <summary>
        /// Indica si un consolidado dado está activo o inactivo
        /// </summary>
        /// <returns></returns>
        /// <param name="codigo">Código del consolidado</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerEstadoActivoConsolidado(string codigo);

        /// <summary>
        /// Retorna los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUTipoConsolidadoDC> ObtenerTiposConsolidado();

        /// <summary>
        /// Retorna los tamaños de la tula
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUTamanoTulaDC> ObtenerTamanosTula();

        /// <summary>
        /// Retorna los motivos de cambios de un contenedor
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SUMotivoCambioDC> ObtenerMotivosCambioContenedor();

        /// <summary>
        /// Registra un nuevo contenedor en la base de datos
        /// </summary>
        /// <param name="contenedor"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RegistrarNuevoContenedor(SUConsolidadoDC contenedor);

        /// <summary>
        /// Registra una modificación de un contendor
        /// </summary>
        /// <param name="consolidado"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RegistrarModificacionContenedor(SUModificacionConsolidadoDC consolidado);

        /// <summary>
        /// Registra una modificación de un contendor
        /// </summary>
        /// <param name="consolidado"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<SUConsolidadoDC> ObtenerListaConsolidados(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        #endregion
    }
}