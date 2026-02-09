using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IOUOperacionUrbanaSvc
    {
        #region Mensajeros

        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarMensajero(OUMensajeroDC mensajero);

        /// <summary>
        /// Obtiene la lista de los tipos de mensajero
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero();

        /// <summary>
        /// Obtiene los estados para los mensajeros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero();

        /// <summary>
        /// Consulta si existe el mensajero
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista);

        /// <summary>
        /// Método para obtener los mensajeros por localidad
        /// </summary>
        /// <param name="Localidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<POMensajero> ObtenerMensajerosLocalidad(string Localidad);


        #endregion Mensajeros

        #region centro de acopio

        /// <summary>
        /// Obtiene el total de los envios pendientes asignados por planilla de venta al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerTotalEnviosPendientes(long idMensajero);

        /// <summary>
        /// Obtiene la informacion del mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC ObtenerInfoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long centroLogistico);

        /// <summary>
        /// Obtiene los estados de los empaques para mensajeria y carga
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUEstadosEmpaqueDC> ObtenerEstadosEmpaque();

        /// <summary>
        /// Obtiene los mensajeros de un centro logistico en una lista paginada
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long centroLogistico);


        /// <summary>
        /// Obtiene todos los mensajeros de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUMensajeroDC> ObtenerMensajerosCol(long idCol);

        /// <summary>
        /// Obtiene los mensajeros de un centro logistico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long puntoServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC GuardarIngreso(OUGuiaIngresadaDC guiaIngresada);

        /// <summary>
        /// retorna el total de los envios planillados por mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int TotalEnviosPlanillados(long idMensajero);

        /// <summary>
        /// Retorna la guia consultada a partir del numero de guia
        /// </summary>
        /// <param name="guiaIngresada"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC ConsultaGuia(OUGuiaIngresadaDC guiaIngresada);

        /// <summary>
        /// Obtiene las envios pendientes por ingresar
        ///
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUGuiasPendientesDC> ObtenerEnviosPendientes(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idMensajero);

        /// <summary>
        /// Obtiene las planillas por centro de servicios
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUPlanillaVentaDC> ObtenerPlanillasPorCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios);

        /// <summary>
        /// Consulta si un punto de servicio tiene almenos una planilla de recoleccion en punto (planilla ventas)  abierta
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarPlanillasAbiertasPorPuntoVenta(long idPuntoServicio);

        /// <summary>
        /// Adiciona una guia a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarGuiaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla);

        /// <summary>
        /// Adiciona una guia suelta a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarGuiaSueltaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla);

        /// <summary>
        /// Elimina una guia de un consolidado en la planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUPlanillaVentaGuiasDC EliminarGuiaConsolidadoPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla);

        /// <summary>
        /// Elimina una guia o un rotulo de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUPlanillaVentaGuiasDC EliminarGuiaPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla);

        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasTotal(OUPlanillaVentaDC planilla, long idCentroServicios);

        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla parcial
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasParcial(long idPlanilla, long idCentroServicios);

        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUImpresionPlanillaVentasDC ObtenerImpresionManifiestoPlanillaVentas(long idPlanilla, long idCentroServicios);

        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUImpresionPlanillaVentasDC> ObtenerImpresionPlanillaVentasSinCerrar(long idPlanilla, long idCentroServicios);

        /// <summary>
        /// Obtiene una lista de las asignaciones de tulas y precintos por punto de servicio,  y por estado
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUAsignacionDC> ObtenerAsignacionTulaPrecintoPuntoServicio(long idPuntoServicio, string estadoAsignacion);

        /// <summary>
        /// Obtiene las guias por punto de servicios sin planillar
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUPlanillaVentaGuiasDC> ObtenerGuiasPorPuntoDeServicios(long idCentroServicios);

        /// <summary>
        /// Guarda la planilla venta y planilla venta guias
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiasPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GuardarPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> guiasPlanilla);

        /// <summary>
        /// Obtiene las guias asignadas a una planilla y a una asignacion tula
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="idAsignacionTula">Id de la asignacion tula</param>
        /// <returns>Lista con las guias asignadas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaDC> ObtenerGuiasPorPlanillaAsignacionTula(long idPlanillaVentas, long idAsignacionTula);

        /// <summary>
        /// Obtiene todas las guias sueltas planilladas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaDC> ObtenerGuiasSueltasPlanilladas(long idPlanilla);

        /// <summary>
        /// Despacha la falla para las guias sin planillar
        /// </summary>
        /// <param name="idCentroServicios"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EnviaFallaConGuiasNoPlanilladas(long idCentroServicios, string nombreCentroServicios, string direccionCentroServicios);

        #endregion centro de acopio

        #region Planilla de asignacion

        /// <summary>
        /// Obtiene los mensajeros del racol
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroPorRegional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idRacol);

        /// <summary>
        /// Retorna las planillas de asignacion del centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha);

        /// <summary>
        /// Retorna los mensajeros asociados al col, filtrados por tipo de mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUMensajeroDC> ObtenerMensejorPorColYTipoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, int idTipoMensajero);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUPlanillaAsignacionDC GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla);

        /// <summary>
        /// Retorna los envios de la planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUGuiaIngresadaDC> ObtenerEnviosPlanillaAsignacion(long idPlanilla);

        /// <summary>
        /// Obtiene las guias de determinado mensajero en determinado dia
        /// </summary>
        /// <param name="idmensajero"></param>
        /// <param name="fechaPlanilla"></param>
        /// <returns>las guias de determinado mensajero con determinada fecha</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUGuiaIngresadaDC> ObtenerGuiasPlanilladasPorDiaYMensajero(long idMensajero, DateTime fechaPlanilla);

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaTotalEnviosPlanillados(long idPlanilla, int totalGuias);

        /// <summary>
        /// Elimina un envio de la planilla de asignacion
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarEnvioPlanillaAsignacion(OUPlanillaAsignacionDC planilla, long idAdmisionMensajeria);



        /// <summary>
        /// Elimina un envio de la planilla de asignacion
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarEnvioPlanillaCentroAcopio(long numeroGuia);


        /// <summary>
        /// Valida que el envío esté asignado a una planilla
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia);

        /// <summary>
        /// Reasigna un envio a otra planilla
        /// </summary>
        /// <param name="idPlanilla">id de la planilla del envio</param>
        /// <param name="idPlanillaNueva">id de la nueva planilla a reasignar</param>
        /// <param name="idAdmisionMensajeria">id del envio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ReasignarEnvioPlanilla(long idPlanilla, long idPlanillaNueva, long idAdmisionMensajeria, long idAgencia);

        /// <summary>
        /// Realiza la verificacion del envio seleccionado
        /// </summary>
        /// <param name="planillaAsignacion"></param>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaEnvioVerificadoPlanillaAsignacion(long planillaAsignacion, long numGuia);

        /// <summary>
        /// Cerrar la planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarPlanillaASignacion(long idPlanilla);

        /// <summary>
        /// Abre la planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AbrirPlanillaAsignacion(long idPlanilla);

        /// <summary>
        /// Verifica el soat y la revision tecnomecanica del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>
        /// true = si el soat y la tecnomecanica estan vigentes
        /// false = si el soat y la tecnomecanica estan vencidos
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificaMensajeroSoatTecnoMecanica(long idMensajero);

        /// <summary>
        /// Asigna el mensajero a la planilla de asignación
        /// </summary>
        /// <param name="planilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla);

        /// <summary>
        /// Obtiene el total de los envios pendientes del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerEnviosPendientesMensajero(long idMensajero);

        /// <summary>
        /// Retorna los estados de la planilla
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUEstadosPlanillaAsignacionDC> ObtenerEstadosPlanillaAsignacion();

        /// <summary>
        /// Retorna los mensajeros del centro logistico
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajerosCOL(long idCentroLogistico);

        /// <summary>
        /// Obtene los datos de los mensajeros de una agencia a partir de un punto de servicio.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosAgenciaDesdePuntoServicio(long idPuntoServicio);
        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia);
        /// <summary>
        /// Modifica una planilla de asignacion de envios
        /// </summary>
        /// <param name="planilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ModificarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla);

        #endregion Planilla de asignacion

        #region Solicitud Recogidas

        /// <summary>
        /// Obtiene los estados de la solicitud de recogida
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUEstadosSolicitudRecogidaDC> ObtenerEstadosRecogida();

        /// <summary>
        /// Obtiene las recogidas de la agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idAgencia"></param>
        /// <param name="incluyeFechaAsignacion"></param>
        /// <param name="incluyeFechaRecogida"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OURecogidasDC> ObtenerRecogidas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idAgencia, bool incluyeFechaAsignacion, bool incluyeFechaRecogida);

        /// <summary>
        /// Guarda la solicitud de recogida del punto de servicios
        /// </summary>
        /// <param name="recogida"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardaSolicitudRecogidaPuntoSvc(OURecogidasDC recogida);

        /// <summary>
        /// Guarda la solicitud de la recogida por cliente convenio
        /// </summary>
        /// <param name="recogida"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarSolicitudClienteConvenio(OURecogidasDC recogida);

        /// <summary>
        /// Guarda la solicitud de recogida del cliente peaton
        /// </summary>
        /// <param name="recogida"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GuardaSolicitudClientePeaton(OURecogidasDC recogida);

        /// <summary>
        /// Actualiza datos básicos de la solicitud de recogida del cliente peaton
        /// </summary>
        /// <param name="recogida"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaSolicitudClientePeaton(OURecogidasDC recogida);

        /// <summary>
        /// Consulta la solicitud de recogida para un cliente peaton especifico
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OURecogidasDC ObtenerInformacionClientePeaton(OURecogidasDC infoRecogida);

        #endregion Solicitud Recogidas

        #region Programacion de recogidas

        /// <summary>
        /// Retorna los motivos de la Reprogramacion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUMotivosReprogramacionDC> ObtenerMotivosReprogramacion();

        /// <summary>
        /// Retorna la recogida por punto de servicio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OURecogidasDC ObtenerRecogidaPuntoServicio(long idSolicitud);

        /// <summary>
        /// Retorna la recogida del cliente convenio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OURecogidasDC ObtenerRecogidaConvenio(long idSolicitud);

        /// <summary>
        /// Retorna el historico de la programacion de la recogida
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUProgramacionSolicitudRecogidaDC> ObtenerProgramacionRecogidas(long idSolicitud);

        /// <summary>
        /// Obtiene las planillas de recogidas creadas para el tipo de mensajero, zona y fecha de recogidas
        /// </summary>
        /// <param name="idZona">id de la zona</param>
        /// <param name="idTipoMensajero">id del tipo de mensajero</param>
        /// <param name="fechaRecogida">fecha de recogida</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidaZonaTipoMenFecha(string idZona, int idTipoMensajero, DateTime fechaRecogida, long idCol);

        /// <summary>
        /// Retorna la recogida peaton
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OURecogidasDC ObtenerRecogidaPeaton(long idSolicitud);

        /// <summary>
        /// Actualiza la solicitud de la recogida y la planilla
        /// </summary>
        /// <param name="programacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion);

        /// <summary>
        /// Agrega una programacion y una planilla a una solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AgregarProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion);

        /// <summary>
        /// Actualiza la georeferenciacion de una recogida
        /// </summary>
        /// <param name="longitud"></param>
        /// <param name="latitud"></param>
        /// <param name="idRecogida"></param>

        void ActualizarGeoreferenciacionRecogida(string longitud, string latitud, long idRecogida);

        /// <summary>
        /// Obtiene los mensajero por centro logistico y tipo
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUNombresMensajeroDC> ObtenerMensajerosPorTipo(long idCentroLogistico, int idTipoMensajero);

        /// <summary>
        /// Obtiene las planillas de recogidas por centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCol"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidas(Dictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCol, bool incluyeFecha);

        /// <summary>
        /// Obtiene la programacion de la recogida esporadica sin planillar
        /// </summary>
        /// <param name="idZona"></param>
        /// <param name="idTipoMensajero"></param>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OURecogidasDC> ObtenerProgramacionRecogidasSinPlanillar(string idZona, short idTipoMensajero, long idCol, DateTime fechaRecogidas);

        /// <summary>
        /// Actualiza el reporte de la recogida al mensajero
        /// </summary>
        /// <param name="programacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaReporteMensajero(OUProgramacionSolicitudRecogidaDC programacion);

        /// <summary>
        /// Guarda la planilla de recogidas con las recogidas seleccionadas
        /// </summary>
        /// <param name="programacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardaPlanillaRecogidas(OUProgramacionSolicitudRecogidaDC programacion);

        /// <summary>
        /// Obtiene la planilla de programacion de recogidas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUProgramacionSolicitudRecogidaDC ObtenerPlanillaRecogida(long idPlanilla);

        #endregion Programacion de recogidas

        #region Descargue Recogidas

        /// <summary>
        /// Retorna los motivos de descargue de recogidas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUMotivoDescargueRecogidasDC> ObtenerMotivosDescargueRecogidas();

        /// <summary>
        /// Retorna los motivos de descargue de recogidas firltrado por idmotivo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUMotivoDescargueRecogidasDC ObtenerMotivosDescargueRecogidasIdMotivo(int idMotivo);

        /// <summary>
        /// Obtiene las recogidas de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OURecogidasDC> ObtenerRecogidasPlanilla(long idPlanilla);

        /// <summary>
        /// Guarda el descargue de una recogida
        /// </summary>
        /// <param name="AdminPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarDescargueRecogida(OUProgramacionSolicitudRecogidaDC AdminPlanilla);

        #endregion Descargue Recogidas

        #region Apertura Recogidas

        /// <summary>
        /// Obtiene los motivos de apertura de una recogida
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUMotivoAperturaDC> ObtenerMotivosAperturaRecogida();

        /// <summary>
        /// Abre una recogida
        /// </summary>
        /// <param name="recogida"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AbrirRecogida(OUAperturaRecogidaDC recogida);

        #endregion Apertura Recogidas

        /// <summary>
        /// retorna el valor del parametro
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerValorParametro(string idParametro);

        /// <summary>
        /// Retorna la lista de parametros
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
        GenericoConsultasFramework<OUParametrosDC> ObtenerParametrosOperacionUrbana(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Edita el parametro de operacion urbana
        /// </summary>
        /// <param name="parametro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarParametroOperacionUrbana(OUParametrosDC parametro);

        #region Asignacion de tulas y precintos

        /// <summary>
        /// Método para obtener los tipos de asignación posibles
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUTipoAsignacionDC> ObtenerTiposAsignacion();


        /// <summary>
        /// Método para obtener las tulas y precintos sin utilizar generadas desde una racol
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUAsignacionDC> ObtenerAsignacionCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);


        /// <summary>
        /// Método para asignar una tula y un precinto a un centro de servicio
        /// </summary>
        /// <param name="asignacionTula"></param>
        /// <returns></returns
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUAsignacionDC AdicionarAsignacionCentroServicio(OUAsignacionDC asignacion);


        /// <summary>
        /// Método para eliminar una asignacion de tulas o contenedores
        /// </summary>
        /// <param name="asignacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarAsignacionTulaContenedor(OUAsignacionDC asignacion);

        #endregion


        #region Novedades de ingreso

        /// <summary>
        /// Método para obtener las novedades de ingreso
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUNovedadIngresoDC> ObtenerNovedadesIngreso();

        #endregion


        #region Ingreso a centro de acopio

        /// <summary>
        /// Método para obtener las asignaciones
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUAsignacionDC> ObtenerAsignaciones(long controlTrans, long noPrecinto, string noConsolidado, OUMensajeroDC mensajero, long idCentroServicio);

        /// <summary>
        /// Método para ingresar una guía suelta a centro de acopio 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUPlanillaVentaGuiasDC IngresarGuiaSuelta(OUPlanillaVentaGuiasDC guia, List<OUNovedadIngresoDC> listaNovedades);

        #endregion

        /// <summary>
        /// Aprovisiona guía catalogada como "fantasma", es decir una numeración que debería ser automática
        /// </summary>
        /// <param name="numGUia"></param>
        /// <param name="idCs"></param>
        bool AprovisionGuiaFantasma(long numGUia, long idCs);

        /// <summary>
        /// Retorna el número de auditoria
        /// </summary>
        /// <param name="idCs"></param>
        /// <returns>Retorna el número de auditoria</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CrearAuditoriaAsignacionMensajero(long idCs, long idMensajero);

        /// <summary>
        /// inserta en la tabla PlanillaAsignacionAuditGuia_OPU
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long idMensajero, DateTime fecha);

        /// <summary>
        /// actualiza el campo es sobrante en la tabla PlanillaAsignacionAuditGuia_OPU
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="numeroGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long numeroGuia);

        /// <summary>
        /// obtiene las auditorias por mensajero en determinado rango de fecha
        /// </summary>
        /// <param name="IdAuditoria"></param>
        /// <param name="fechaIni"></param>
        /// <param name="fechaFin"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaDC> ObtenerAuditoriasPorMensajero(long IdAuditoria, DateTime fechaIni, DateTime fechaFin);

        /// <summary>
        /// obtiene las guias de determinada auditoria
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaDC> ObtenerGuiasPorAuditoria(long idAuditoria);

        /// <summary>
        /// obtiene el detallado de las guias de la auditoria
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaDC> ObtenerDetalleGuiasAuditadas(long idAuditoria);

        /// <summary>
        /// Inserta la relacion entre un dispositivo movil y una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        /// <param name="idDispositivoMovil"></param>
        void RegistrarSolicitudRecogidaMovil(long idRecogida, long idDispositivoMovil);
        /// <summary>
        /// retorna el token del dispositivo movil con el que se hizo una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        PADispositivoMovil ObtenerdispositivoMovilClienteRecogida(long idRecogida);



        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y ciudad de recogida
        /// </summary>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerRecogidasPeatonPendientesPorProgramarDia(string idLocalidad);

        /// <summary>
        /// /// Obtiene todas las solicitudes de recogida disponibles por localidad.
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerRecogidasDisponiblesPeatonDia(string idLocalidad);


        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programas
        /// </summary>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerTodasRecogidasPeatonPendientesPorProgramar();

        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida
        /// </summary>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerRecogidasPeatonPendientesDia();

        /// <summary>
        /// Guarda las notificaciones enviadas por cada recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        void GuardarNotificacionRecogida(long idSolicitudRecogida);

        /// <summary>
        /// Obtiene los datos del usuario que está solicitando 
        /// la recogida si ya se ha registrado antes.
        /// </summary>
        /// <param name="tipoid"></param>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        PAPersonaInternaDC ObtenerInfoUsuarioRecogida(string tipoid, string identificacion);

        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerRecogidasMensajerosDia(long idMensajero);

        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerRecogidasClienteMovilDia(string tokenDispositivo);

        /// <summary>
        /// Selecciona todas las recogidas vencidas que fueron asignadas a los usuarioMensajero (usuarios PAM ) en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        List<OURecogidasDC> ObtenerRecogidasVencidasMensajerosPAMDia();
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<COTipoNovedadGuiaDC> ObtenerTiposNovedadGuia(COEnumTipoNovedad tipoNovedad);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario);

        #region Obtener mensajeros pam por centro de servicio
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUMensajeroPamDC> ObtenerMensajerosPamPorCentroServicio(long IdCentroServicio);
        #endregion

        #region Obtener lista guias mensajeros pam por centro de servicio
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIReclameEnOficinaDC> ObtenerGuiasDelPamPorCentroServicio(long IdCentroServicio, string usuario);
        #endregion

        #region ingresa guia pam
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIReclameEnOficinaDC IngresoGuiaPam(LIReclameEnOficinaDC GuiasPam);
        #endregion

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CrearPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> LstGuias);

        /// <summary>
        /// Obtiene los mensajeros de una punto especifico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia);

        /// <summary>
        /// Metodo para consultar la ultima informacion de usuario externo
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OURecogidasDC ObtenerInformacionRecogidaUsuarioExterno(string nomUsuario);
        /// <summary>
        /// metodo para obtener motivo y fecha intento de entrega por numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMotivoGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIMotivoEvidenciaGuiaDC ObtenerFechaIntentoYMotivoGuia(long numeroGuia, long idPlanilla);
        /// <summary>
        /// Metodo que consulta las guias planilladas por auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor);

        /// <summary>
        /// Metodo para obtener las guias planilladas para el mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero);

        /// <summary>
        /// Metodo para obtener las guias planillas al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZona(long idMensajero);

        /// <summary>
        /// Metodo para obtener las guias entregadas planillas al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasMensajero(long idMensajero);

        /// <summary>
        /// Metodo para obtener las guias en devolucion del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionMensajero(long idMensajero);

        /// <summary>
        /// Metodo para obtener las guias en zona del auditor 
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaAppDC> ObtenerGuiasEnZonaAuditor(long idAuditor);

        /// <summary>
        /// Metodo para obtener las guias entregadas por auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasAuditor(long idAuditor);

        /// <summary>
        /// Metodo para obtener las guias en devolucion del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionAuditor(long idAuditor);


        /// <summary>
        /// Obtener la guia en planilla para descargue 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaDescargue(long numeroGuia, long idMensajero);

        /// <summary>
        /// metodo para obtener la guia planillada al auditor para descargar con controller app
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaAuditorDescargue(long numeroGuia, long idMensajero);

        /// <summary>
        /// Metodo para obtener la informacion del usuario (mensajero/auditor)
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUMensajeroDC ObtenerInformacionUsuarioControllerApp(string numIdentificacion);

        /// <summary>
        /// Obtener la informacion de un mensajero por medio de su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Datos mensajero</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero);
    }
}