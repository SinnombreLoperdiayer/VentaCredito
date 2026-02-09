using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ICACajasSvc
    {
        /// <summary>
        /// Adiciona el movimientor caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, PAEnumConsecutivos? idConsecutivoComprobante);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CACierreCajaDC> ObtenerCajasParaCerrar(long idPuntoCentroServicio, short idFormaPago, string operador);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarCaja(long idCodigoUsuario, long idApertura, long idPunto, int idCaja);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAResumeCierrePuntoDC> ObtenerResumenCierrePunto(long idPuntoCentroServicio, long idCierrePuntoAsociado);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CACierreCentroServicioDC ObtenerUltimoCierrePunto(long idPuntoCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CACierreCentroServicioDC> ObtenerCierresPunto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idPuntoCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CerrarCajaPuntoCentroServcio(CACierreCentroServicioDC cierrePuntoCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        decimal ObtenerValorEmpresa(long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarDineroEnvioEmpresa(CARecoleccionDineroPuntoDC EnvioDineroAgencia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarDineroAgencia(CARecoleccionDineroPuntoDC dineroRecibido);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicio(long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<SUSuministro> ObtenerSuministrosPunto(PUCentroServiciosDC centroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CARecoleccionDineroPuntoDC> ObtenerDineroReportadoPuntos(IDictionary<string, string> filtro, long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CATipoObsPuntoAgenciaDC> ObtenerTiposObservPunto();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicioPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                                int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                                long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarReporteMensajero(CAReporteMensajeroCajaDC reportMensajero);

         /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAReporteMensajeroCajaDC> ObtenerReportesMensajeros(long idMensajero);

         /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero por comprobante para imprimir
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="numComprobante"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CADatosImpCompMensajeroDC ObtenerReportesMensajerosImprimir(long idMensajero, string numComprobante);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CADatosImpCompMensajeroDC RecibirDineroMensajero(List<CARecibirDineroMensajeroDC> transaccionMensajero);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CACuentaMensajeroDC> ObtenerEstadoCtaMensajero(long idMensajero, DateTime fechaConsulta);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAConceptoCajaDC> ObtenerConceptosCajaPorCategoria(int idCategoria);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarObservacionEstadoCta(CACuentaMensajeroDC cuentaMensajero);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAPinPrepagoDC> ObtenerPrepagosCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idCentroServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAPinPrepagoDtllCompraDC> ObtenerDtllCompraPinPrepago(long pinPrepago);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarPinPrepago(CAPinPrepagoDC pinPrepago);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void VenderPinPrepago(CAVenderPinPrepagoDC ventaPinPrepago);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PABanco> ObtenerTodosBancos();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ObtenerPersonaListaRestrictiva(string documento);

        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        string ObtenerParametroCajas(string idParametro);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CACierreCajaDC> ObtenerInfoCierreCaja(long idUsuario, short idFormaPago, string operador, long idCentroServicio, int idCaja);

        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        List<CACierreCajaDC> ObtenerCierresCajero(long idCodigoUsuario, long IdCentroServicio, DateTime fechaCierre, int indicePagina, int tamanoPagina);

        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol);

        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        long ObtenerConsecutivoComprobateCajaIngreso();

        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        long ObtenerConsecutivoComprobateCajaEgreso();

        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        List<CACierreCajaDC> ObtenerCajasAReportarCajeroPrincipal(long idCentroSrv, short idFormaPago, string operador);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        AREmpresaDC ObtenerDatosEmpresa();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ReportarCajaACajeroPrincipal(CARegistroTransacCajaDC movimientoCaja, long idCierreCaja);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarCajasAutomaticamentePorCentroSvc(CARegistroTransacCajaDC movimientoCaja);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ObtenerUltimaAperturaActiva(long idCentroServicio, int idCaja, long idCodUsuario);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PAConsecutivoIngresoEgresoDC ObtenerConsecutivoIngresoEgreso();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void TransladarDineroEntreCajas(CAOperaRacolBancoEmpresaDC infoTransaccion);

        /// <summary>
        /// Obtener información complementaria para el cierre de una caja
        /// </summary>
        /// <param name="idCierre">Identificacdor del cierre de caja</param>
        /// <returns>Información complementaria del cierre de caja</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAInfoComplementariaCierreCajaDC ObtenerInfoComplementariaCierreCaja(long idCierreCaja);

        /// <summary>
        /// Obtener el resumen para el cierre de de caja principal
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <param name="idAperturaCaja">Identificador de la apertura de caja</param>
        /// <param name="idCierrePuntoAsociado">Identificador del cierre de centro de servicios, para el caso de consulta de un cierre ya hecho</param>
        /// <returns>Resumen del cierre o transacciones de una apertura</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CAConsolidadoCierreDC ObtenerResumenCierreCajaPrincipal(long idCentroServicios, long idAperturaCaja, long idCierrePuntoAsociado);

        /// <summary>
        /// Obtiene la Info de la Pantalla de
        /// RegistroEnvioAgencia
        /// </summary>
        /// <param name="idCentroServicio">idcentroServicio</param>
        /// <returns>info de registroEnvio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CARegistrarEnvioAgenciaDC ObtenerInfoRegistroEnvioAgencia(long idCentroServicio);

        /// <summary>
        /// Procedimientos para el descuento y generacion de la informacion
        /// para el formato de descuento de nomina
        /// </summary>
        /// <param name="registroMensajero">datos del mensajero a descontar</param>
        /// <returns>info de impresion formato</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CADatosImpCompMensajeroDC DescuentoNominaMensajero(CACuentaMensajeroDC registroMensajero);

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns>lista de Clientes</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CLClientesDC> ObtenerClientes();

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro">Numero del giro a consultar</param>
        /// <returns>informacion del giro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro);        
    }
}