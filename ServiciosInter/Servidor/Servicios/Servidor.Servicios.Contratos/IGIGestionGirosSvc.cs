using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    ///Contratos WCF de Solicitudes de Giros
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IGIGestionGirosSvc
    {
        #region Solicitudes

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIMotivoSolicitudDC> ObtenerMotivosSolicitudes(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActuzalizarMotivoSolicitudes(GIMotivoSolicitudDC motivoSolicitud);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GITipoSolicitudDC> ObtenerTiposSolicitudes();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC ObtenerGiro(long idGiro, string digVerif);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GISolicitudGiroDC> ObtenerSolicitudesPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                             int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                             string idRacol, string idAgencia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GISolicitudGiroDC> ObtenerSolicitudesActivas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                          int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                          long idRegional);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC AdicionarNvaSolicitud(GISolicitudGiroDC nvaSol);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC ObtenerDetalleSolicitud(long idSolicitud);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoAdjunto(long idArchivo);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GISolicitudGiroDC> ObtenerSolicitudesAnteriores(string idGiro);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC ActualizarSolicitud(GISolicitudGiroDC solicitudAtendida);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ObtenerConsecutivoSolicitud();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PURegionalAdministrativa> ObtenerRegionalAdministrativa();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEEstadoUsuario> ObtenerTiposEstado();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerUltimoEstadoGiro(long idGiro, long idRacol);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarCambioEstadoGiro(long idAdmisionGiro, string estadoGiro);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC CrearSolicitudAnulacionComprobante(GISolicitudGiroDC nvaSolicitud);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidarComprobantePago(long idSuministro, long idAgencia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUPropietarioGuia ValidarSuministro(long idSuministro, SUEnumSuministro tipoSuministro, long idPropietario);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUPropietarioGuia ValidarSuministroNumeroSolicitud(long idSolicitud, long idPropietario);

        /// <summary>
        /// Obtiene el detalle de una solicitud.
        /// </summary>
        /// <param name="idSolicitud">id de la solicitud a consultar.</param>
        /// <returns>el detalle de una solicitud</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GISolicitudGiroDC ObtenerDetalleSolicitudAprobada(long idSolicitud, long idRacol);

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoIdentificacion> ObtenerTiposIdentificacionReclamaGiros();

        /// <summary>
        /// Obtiene la Informacion de Carga inicial
        /// </summary>
        /// <returns>info inicial de cargue</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIInfoCargaInicialSolicitudDC ObtenerInfoCargaInicialSolicitud();

        #endregion Solicitudes

        #region Explorador de Giros

        /// <summary>
        /// Obtiene los datos del explorador de giros
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registros por página</param>
        /// <returns>Colección de giros</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<GIAdmisionGirosDC> ObtenerGirosExplorador(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha);

        /// <summary>
        /// Obtiene los tipos de giros
        /// </summary>
        /// <returns>Colección con los tipos de giros</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<GITipoGiroDC> ObtenerTiposGiros();

        /// <summary>
        /// Obtiene los valores adicionales de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección valores adicionales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAValorAdicional> ObtenerValoresAdicionalesGiro(long idGiro);

        /// <summary>
        /// Obtiene los impuestos de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección impuestos giros</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAImpuestoDelServicio> ObtenerImpuestosGiros(long idGiro);

        /// <summary>
        /// Obtiene la información del pago
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Objeto pago</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PGPagosGirosDC ObtenerInformacionPago(long idAdmisionGiro);

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivosGiros(long numeroGiro);

        /// <summary>
        /// Obtiene la informacíón dependiendo del tipo de giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <param name="tipoGiro">Tipo de giro</param>
        /// <returns>Objeto datos convenio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ObtenerInformacionTipoGiro(long idAdmisionGiro, string tipoGiro);

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro);

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado);

        /// <summary>
        /// Obtiene la informacion de los intentos a transmitir de un giro
        /// </summary>
        /// <param name="idAdminGiro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro);

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion);

        /// <summary>
        /// Obtiene la informacion del almacen
        /// </summary>
        /// <param name="idOperacion">Numero del giro</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion);

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<GIEstadosGirosDC> ObtenerEstadosGiro(long idAdminGiro);

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoAdjuntoSolicitud(long idArchivo);

        /// <summary>
        /// Obtiene la informacion de Telemercadeo de
        /// un giro especifico
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns>la info del telemercadeo de un giro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro);

         /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion);

        #endregion Explorador de Giros

        #region Pagos Manuales

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicioOrigen, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgenciaPorTrasmitir(long idCentroServicioDestino);

        /// <summary>
        /// Consultar informacion del giro adicionales - impuestos- intentos transmitir
        /// </summary>
        /// <param name="idGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ConsultarGiroTransmisionGiro(long idGiro);

        /// <summary>
        ///  Insertar los intentos de transmision de un giro y
        ///  Actualizar la tabla giros indicando que el giro ya fue transmitido
        /// </summary>
        /// <param name="intestosTransmision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarIntentosTransmisionGiro(GIIntentosTransmisionGiroDC intentosTransmision);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long InsertarIntentosTransmisionFax(GIIntentosTransmisionGiroDC intentosTransmision);

        /// <summary>
        /// consulta los giros activos para realizar el descarge manual de pagos
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<GIAdmisionGirosDC> ConsultarGirosADescargar(long? idCentroServicioDestino, long? idGiro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// consulta los giros activos para realizar el descarge manual de pagos
        /// por el id del giro y que correspondan al RACOL que esta logeado
        /// en el cliente
        /// </summary>
        /// <param name="lstCentroSrv">lista de centros de servicios de racol</param>
        /// <param name="idGiro">id del giro</param>
        /// <param name="indicePagina">indice de pagina</param>
        /// <param name="registrosPorPagina">registros por pagina</param>
        /// <returns>la info del giro encontrado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ConsultarGiroADescargarPorRacol(ObservableCollection<PUCentroServiciosDC> lstCentroSrv, long? idGiro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void PagarGiro(PGPagosGirosDC pagosGiros);

        /// <summary>
        /// Obtiene las planillas de trasmision para una agencia
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<GIIntentosTransmisionGiroDC> ObtenerPlanillasTrasmisionAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idAgencia);

        #endregion Pagos Manuales

        #region Giros Convenio

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerClientesContratosGiros();

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// y Cupo de Dispersion Aprobado
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerTodosClientesContratosGiros();

        /// <summary>
        /// Adiciona, edita o elimina una de las condiciones para el servicio de giros para un cliente
        /// </summary>
        /// <param name="cuentaExterna">Objeto cuenta externa</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdministrarClienteCondicionGiro(CLContratosDC contrato);

        #endregion Giros Convenio

        #region Configuracion Giros

        /// <summary>
        /// consulta los diferentes clasificaciones que existen para un agencia por el servicio de giros
        /// superhabitarias
        /// deficitarias
        /// compensadas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUClasificacionPorIngresosDC> ConsultarClasificacionPorIngreso();

        /// <summary>
        /// Consultar los tipos de motivo  para la inactivacion de un giro
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ConsultarTiposMotivos();

        /// <summary>
        /// Guarda la nueva informacion de la configuracion de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarConfiguracionGiro(PUCentroServiciosDC centroServicio);

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio);

        #endregion Configuracion Giros

        #region Telemercadeo

        /// <summary>
        /// Retorna los resultados de la gestion de telemercadeo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIResultadoGestionTelemercadeoDC> ObtenerResultadoGestionTelemercadeo();

        /// <summary>
        /// Obtiene los giros que cumplen con los tiempos para estar en telemercadeo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ObtenerGirosTelemercadeo(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idRacol);

        /// <summary>
        /// Obtiene la informacion del giro
        /// </summary>
        /// <param name="giro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIAdmisionGirosDC ObtenerInformacionGiroTelemercadeo(GIAdmisionGirosDC giro);

        /// <summary>
        /// Obtiene el historico de telemercadeo de un giro
        /// </summary>
        /// <param name="idGiro"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<GITelemercadeoGiroDC> ObtenerHistoricoTelemercadeoGiro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idGiro);

        /// <summary>
        /// Guardar telemercadeo giro
        /// </summary>
        /// <param name="telemercadeo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarTelemercadeoGiro(GITelemercadeoGiroDC telemercadeo);

        /// <summary>
        /// Retorna la informacion de telemercadeo
        /// </summary>
        /// <param name="idTelemercadeoGiro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GITelemercadeoGiroDC ObtenerDetalleTelemercadeoGiro(long idTelemercadeoGiro);

        #endregion Telemercadeo

        #region Centro Servicios

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUCentroServiciosDC> ObtenerAgencias();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol);

        /// <summary>
        /// Metodo para Obtener los Centros de servicios de un racol
        /// Que tengan giros por pagar
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicioRacol(long idRacol);

        #endregion Centro Servicios

        /// <summary>
        /// Obtiene la informacíón básica de un giro
        /// </summary>
        /// <param name="informacionGiro">Información del giro</param>
        /// <returns>Explorador de giros.</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GIExploradorGirosWebDC ObtenerDatosGiros(GIExploradorGirosWebDC informacionGiro);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIMotivosInactivacion> ObtenerMotivosGiros();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUArchivosPersonas> ObtenerTodasPersonasInternas();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo);
    }
}