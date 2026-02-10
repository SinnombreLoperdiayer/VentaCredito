using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.ExploradorGiros;
using CO.Servidor.GestionGiro.ClienteConvenio;
using CO.Servidor.PagosManuales;
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
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Solicitudes.Giros;
using CO.Servidor.Telemercadeo.Giros;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Servicios.Implementacion.GestionGiros
{
    /// <summary>
    ///Implementacion de Solicitudes Giros
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GIGestionGirosSvc : IGIGestionGirosSvc
    {
        #region Motivos

        /// <summary>
        /// Obtiene los Motivos de las solicitudes
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<GIMotivoSolicitudDC> ObtenerMotivosSolicitudes(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerMotivosSol(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona - Modifica - Elimina el Motivo de la Solicitud Rafael Ramirez 05-01-2011
        /// </summary>
        /// <param name="motivoSolicitud"></param>
        public void ActuzalizarMotivoSolicitudes(GIMotivoSolicitudDC motivoSolicitud)
        {
            GIAdministradorSolicitudes.Instancia.ActuzalizarMotivoSolicitudes(motivoSolicitud);
        }

        /// <summary>
        /// Obtiene los Tipos Solicitudes y Motivos
        /// </summary>
        /// <returns></returns>
        public List<GITipoSolicitudDC> ObtenerTiposSolicitudes()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerTiposSolicitudes();
        }

        #endregion Motivos

        #region Solicitudes

        /// <summary>
        /// Obtiene las Solicitudes por Agencia y Giro
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <param name="idGiro"></param>
        /// <param name="dgVerif"></param>
        /// <returns></returns>
        public GISolicitudGiroDC ObtenerGiro(long idGiro, string digVerif)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerGiro(idGiro, digVerif);
        }

        /// <summary>
        /// Obtiene las Solicitudes por Agencia
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <param name="idGiro"></param>
        /// <param name="dgVerif"></param>
        /// <returns></returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina,
                                                                          bool ordenamientoAscendente, out int totalRegistros, string idRacol, string idAgencia)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerSolicitudesPorAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                        ordenamientoAscendente, out totalRegistros, idRacol, idAgencia);
        }

        /// <summary>
        /// Obtiene Todas las Solicitudes Activas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesActivas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina,
                                                                         bool ordenamientoAscendente, out int totalRegistros, long idRegional)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerSolicitudesActivas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                        ordenamientoAscendente, out totalRegistros, idRegional);
        }

        /// <summary>
        /// Metodo para Obtener el detalle de la
        /// Solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitud(long idSolicitud)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerDetalleSolicitud(idSolicitud);
        }

        /// <summary>
        /// Obtiene el detalle de una solicitud.
        /// </summary>
        /// <param name="idSolicitud">id de la solicitud a consultar.</param>
        /// <returns>el detalle de una solicitud</returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitudAprobada(long idSolicitud, long idRacol)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerDetalleSolicitudAprobada(idSolicitud, idRacol);
        }

        /// <summary>
        /// Metodo que Adiciona una nueva Solicitud
        /// dependiendo del tipo de Motivo que tenga
        /// </summary>
        /// <param name="nvaSol"></param>
        public GISolicitudGiroDC AdicionarNvaSolicitud(GISolicitudGiroDC nvaSol)
        {
            return GIAdministradorSolicitudes.Instancia.AdicionarNvaSolicitud(nvaSol);
        }

        /// <summary>
        /// Metodo poara obtener el archivo
        /// Adjunto
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoAdjunto(long idArchivo)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerArchivoAdjunto(idArchivo);
        }

        /// <summary>
        /// Metodo de Consulta de las
        /// Solicitudes de un giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns></returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesAnteriores(string idGiro)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerSolicitudesAnteriores(idGiro);
        }

        /// <summary>
        /// Metodo de actualizacion e
        /// insercion de registro de solicitudes atendidas
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        public GISolicitudGiroDC ActualizarSolicitud(GISolicitudGiroDC solicitudAtendida)
        {
            return GIAdministradorSolicitudes.Instancia.ActualizarSolicitud(solicitudAtendida);
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de una solicitud
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoSolicitud()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerConsecutivoSolicitud();
        }

        /// <summary>
        /// Metodo para Obtener las
        /// Regionales Administrativas
        /// </summary>
        /// <returns></returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Metodo para Obtener los Centros de servicios de un racol
        /// Que tengan giros por pagar
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioRacol(long idRacol)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerCentrosServicioRacol(idRacol);
        }

        /// <summary>
        /// Obtiene posibles estados
        /// </summary>
        /// <returns></returns>
        public List<SEEstadoUsuario> ObtenerTiposEstado()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerTiposEstado();
        }

        /// <summary>
        /// Metodo para consultar el ultimo estado del giro
        /// por el numero del giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns>el estado del giro</returns>
        public string ObtenerUltimoEstadoGiro(long idGiro, long idRacol)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerUltimoEstadoGiro(idGiro, idRacol);
        }

        /// <summary>
        /// Inserta el Cambio del estado del giro
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        /// <param name="contexto"></param>
        public void InsertarCambioEstadoGiro(long idAdmisionGiro, string estadoGiro)
        {
            GIAdministradorSolicitudes.Instancia.InsertarCambioEstadoGiro(idAdmisionGiro, estadoGiro);
        }

        /// <summary>
        /// Se llenan los Datos Pendientes, Obtiene el
        /// consecutivo de la Solicitud y retorna
        /// Rafram
        /// </summary>
        /// <param name="nvaSolicitud"></param>
        /// <returns>la Info de la Solicitud para Imprimir Formato</returns>
        public GISolicitudGiroDC CrearSolicitudAnulacionComprobante(GISolicitudGiroDC nvaSolicitud)
        {
            return GIAdministradorSolicitudes.Instancia.CrearSolicitudAnulacionComprobante(nvaSolicitud);
        }

        /// <summary>
        /// Valida que el comprobante de pago no haya sido usado
        /// aplica solo para pagos Manuales
        /// </summary>
        public void ValidarComprobantePago(long idSuministro, long idAgencia)
        {
            GIAdministradorSolicitudes.Instancia.ValidarComprobantePago(idSuministro, idAgencia);
        }

        /// <summary>
        /// Valida el suministro
        /// </summary>
        /// <param name="idSuministro">suministro</param>
        /// <param name="tipoSuministro">tipo de Suministro</param>
        /// <returns>info del suministro</returns>
        public SUPropietarioGuia ValidarSuministro(long idSuministro, SUEnumSuministro tipoSuministro, long idPropietario)
        {
            return GIAdministradorSolicitudes.Instancia.ValidarSuministro(idSuministro, tipoSuministro, idPropietario);
        }

        /// <summary>
        /// Valida el Suministro del numero de la Solicitud
        /// </summary>
        /// <param name="idSolicitud">suministro</param>
        public SUPropietarioGuia ValidarSuministroNumeroSolicitud(long idSolicitud, long idPropietario)
        {
            return GIAdministradorSolicitudes.Instancia.ValidarSuministro(idSolicitud, SUEnumSuministro.SOLICITUD_MODIFICACION_GIROS_POSTALES, idPropietario);
        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ObtenerTiposIdentificacionReclamaGiros()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerTiposIdentificacionReclamaGiros();
        }

        /// <summary>
        /// Obtiene la Informacion de Carga inicial
        /// </summary>
        /// <returns>info inicial de cargue</returns>
        public GIInfoCargaInicialSolicitudDC ObtenerInfoCargaInicialSolicitud()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerInfoCargaInicialSolicitud();
        }

        #endregion Solicitudes

        #region Explorador de Giros

        /// <summary>
        /// Obtiene los datos del explorador de giros
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registros por página</param>
        /// <returns>Colección de giros</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<GIAdmisionGirosDC> ObtenerGirosExplorador(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<GIAdmisionGirosDC>()
            {
                Lista = GIAdministradorExploradorGiros.Instancia.ObtenerGirosExplorador(filtro, indicePagina, registrosPorPagina, incluyeFecha, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene los tipos de giros
        /// </summary>
        /// <returns>Colección con los tipos de giros</returns>
        public IEnumerable<GITipoGiroDC> ObtenerTiposGiros()
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerTiposGiros();
        }

        /// <summary>
        /// Obtiene los valores adicionales de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValoresAdicionalesGiro(long idGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerValoresAdicionalesGiro(idGiro);
        }

        /// <summary>
        /// Obtiene los impuestos de un giro
        /// </summary>
        /// <param name="idGiro">Identificador Giro</param>
        /// <returns>Colección impuestos giros</returns>
        public IEnumerable<TAImpuestoDelServicio> ObtenerImpuestosGiros(long idGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerImpuestosGiros(idGiro);
        }

        /// <summary>
        /// Obtiene la información del pago
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Objeto pago</returns>
        public PGPagosGirosDC ObtenerInformacionPago(long idAdmisionGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerInformacionPago(idAdmisionGiro);
        }


        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="numeroGiro">NUmero de giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivosGiros(long numeroGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerArchivosGiros(numeroGiro);
        }

        /// <summary>
        /// Obtiene la informacíón dependiendo del tipo de giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <param name="tipoGiro">Tipo de giro</param>
        /// <returns>Objeto datos convenio</returns>
        public GIAdmisionGirosDC ObtenerInformacionTipoGiro(long idAdmisionGiro, string tipoGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerInformacionTipoGiro(idAdmisionGiro, tipoGiro);
        }

        /// <summary>
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        public IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerSolicitudesGiros(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerArchivoSolicitud(idSolicitud, archivoSeleccionado);
        }

        /// <summary>
        /// Obtiene la informacion de los intentos a transmitir de un giro
        /// </summary>
        /// <param name="idAdminGiro"></param>
        /// <returns></returns>
        public List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerIntentosTransmitir(idAdminGiro);
        }

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        public FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion)
        {
            return GIAdministradorExploradorGiros.Instancia.ConsultarFacturaPorNumeroOperacion(numeroOperacion);
        }

        /// <summary>
        /// Obtiene la informacion del almacen
        /// </summary>
        /// <param name="idOperacion">Numero del giro</param>
        /// <returns></returns>
        public List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerAlmacenControlCuentas(idOperacion);
        }

        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        public IList<GIEstadosGirosDC> ObtenerEstadosGiro(long idAdminGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerEstadosGiro(idAdminGiro);
        }

        /// <summary>
        /// Obtiene el archivo adjunto Solicitud.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        public string ObtenerArchivoAdjuntoSolicitud(long idArchivo)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerArchivoAdjuntoSolicitud(idArchivo);
        }

        /// <summary>
        /// Obtiene la informacion de Telemercadeo de
        /// un giro especifico
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns>la info del telemercadeo de un giro</returns>
        public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerTelemercadeoDeGiro(idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene reporte para la uiaf de giros
        /// </summary>
        /// <param name="fechaGeneracion">Fecha para generar el reporte</param>
        /// <returns>Lista con los registros a reportar</returns>
        public List<string> ObtenerReporteUIAFGiros(DateTime fechaGeneracion)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerReporteUIAFGiros(fechaGeneracion);
        }

        #endregion Explorador de Giros

        #region Pagos Manuales

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IEnumerable<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicioOrigen, int indicePagina, int registrosPorPagina)
        {
            return GIPagosManuales.Instancia.ConsultarGirosPeatonPeatonPorAgencia(idCentroServicioOrigen, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgenciaPorTrasmitir(long idCentroServicioDestino)
        {
            return GIPagosManuales.Instancia.ConsultarGirosPeatonPeatonPorAgenciaPorTrasmitir(idCentroServicioDestino);
        }

        /// <summary>
        /// Consultar informacion del giro adicionales - impuestos- intentos transmitir
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroTransmisionGiro(long idGiro)
        {
            return GIPagosManuales.Instancia.ConsultarGiroTransmisionGiro(idGiro);
        }

        /// <summary>
        ///  Insertar los intentos de transmision de un giro y
        ///  Actualizar la tabla giros indicando que el giro ya fue transmitido
        /// </summary>
        /// <param name="intestosTransmision"></param>
        public void InsertarIntentosTransmisionGiro(GIIntentosTransmisionGiroDC intentosTransmision)
        {
            GIPagosManuales.Instancia.InsertarIntentosTransmisionGiro(intentosTransmision);
        }

        /// <summary>
        /// Transmision de giros via fax
        /// </summary>
        /// <param name="intentosTransmision"></param>
        public long InsertarIntentosTransmisionFax(GIIntentosTransmisionGiroDC intentosTransmision)
        {
            return GIPagosManuales.Instancia.InsertarIntentosTransmisionFax(intentosTransmision);
        }

        /// <summary>
        /// consulta los giros activos para realizar el descarge manual de pagos
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IEnumerable<GIAdmisionGirosDC> ConsultarGirosADescargar(long? idCentroServicioDestino, long? idGiro, int indicePagina, int registrosPorPagina)
        {
            return GIPagosManuales.Instancia.ConsultarGirosADescargar(idCentroServicioDestino, idGiro, indicePagina, registrosPorPagina);
        }

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
        public List<GIAdmisionGirosDC> ConsultarGiroADescargarPorRacol(ObservableCollection<PUCentroServiciosDC> lstCentroSrv, long? idGiro, int indicePagina, int registrosPorPagina)
        {
            return GIPagosManuales.Instancia.ConsultarGiroADescargarPorRacol(lstCentroSrv, idGiro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene las planillas de trasmision para una agencia
        /// </summary>
        /// <returns></returns>
        public GenericoConsultasFramework<GIIntentosTransmisionGiroDC> ObtenerPlanillasTrasmisionAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idAgencia)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<GIIntentosTransmisionGiroDC>()
            {
                Lista = GIPagosManuales.Instancia.ObtenerPlanillasTrasmisionAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idAgencia),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        public void PagarGiro(PGPagosGirosDC pagosGiros)
        {
            GIPagosManuales.Instancia.PagarGiro(pagosGiros);
        }

        #endregion Pagos Manuales

        #region Giros Convenio

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosGiros()
        {
            return GIAdministradorClienteConvenio.Instancia.ObtenerClientesContratosGiros();
        }

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// y Cupo de Dispersion Aprobado
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerTodosClientesContratosGiros()
        {
            return GIAdministradorClienteConvenio.Instancia.ObtenerTodosClientesContratosGiros();
        }

        /// <summary>
        /// Adiciona, edita o elimina una de las condiciones para el servicio de giros para un cliente
        /// </summary>
        /// <param name="cuentaExterna">Objeto cuenta externa</param>
        public void AdministrarClienteCondicionGiro(CLContratosDC contrato)
        {
            GIAdministradorClienteConvenio.Instancia.AdministrarClienteCondicionGiro(contrato);
        }

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
        public List<PUClasificacionPorIngresosDC> ConsultarClasificacionPorIngreso()
        {
            return GIAdministradorSolicitudes.Instancia.ConsultarClasificacionPorIngreso();
        }

        /// <summary>
        /// Consultar los tipos de motivo  para la inactivacion de un giro
        /// </summary>
        /// <returns></returns>
        public List<string> ConsultarTiposMotivos()
        {
            return GIAdministradorSolicitudes.Instancia.ConsultarTiposMotivos();
        }

        /// <summary>
        /// Guarda la nueva informacion de la configuracion de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        public void GuardarConfiguracionGiro(PUCentroServiciosDC centroServicio)
        {
            GIAdministradorSolicitudes.Instancia.GuardarConfiguracionGiro(centroServicio);
        }

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerObservacionCentroServicio(idCentroServicio);
        }

        #endregion Configuracion Giros

        #region TelemercadeoGiros

        /// <summary>
        /// Retorna los resultados de la gestion de telemercadeo
        /// </summary>
        /// <returns></returns>
        public List<GIResultadoGestionTelemercadeoDC> ObtenerResultadoGestionTelemercadeo()
        {
            return GIAdministradorTelemercadeoGiros.Instancia.ObtenerResultadoGestionTelemercadeo();
        }

        /// <summary>
        /// Obtiene los giros que cumplen con los tiempos para estar en telemercadeo
        /// </summary>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ObtenerGirosTelemercadeo(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idRacol)
        {
            return GIAdministradorTelemercadeoGiros.Instancia.ObtenerGirosTelemercadeo(filtro, indicePagina, registrosPorPagina, idRacol);
        }

        /// <summary>
        /// Obtiene la informacion del giro
        /// </summary>
        /// <param name="giro"></param>
        public GIAdmisionGirosDC ObtenerInformacionGiroTelemercadeo(GIAdmisionGirosDC giro)
        {
            return GIAdministradorTelemercadeoGiros.Instancia.ObtenerInformacionGiroTelemercadeo(giro);
        }

        /// <summary>
        /// Obtiene el historico de telemercadeo de un giro
        /// </summary>
        /// <param name="idGiro"></param>
        public GenericoConsultasFramework<GITelemercadeoGiroDC> ObtenerHistoricoTelemercadeoGiro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idGiro)
        {
            int totalRegistros;
            return new GenericoConsultasFramework<GITelemercadeoGiroDC>()
            {
                Lista = GIAdministradorTelemercadeoGiros.Instancia.ObtenerHistoricoTelemercadeoGiro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idGiro),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Guardar telemercadeo giro
        /// </summary>
        /// <param name="telemercadeo"></param>
        public void GuardarTelemercadeoGiro(GITelemercadeoGiroDC telemercadeo)
        {
            GIAdministradorTelemercadeoGiros.Instancia.GuardarTelemercadeoGiro(telemercadeo);
        }

        /// <summary>
        /// Retorna la informacion de telemercadeo
        /// </summary>
        /// <param name="idTelemercadeoGiro"></param>
        /// <returns></returns>
        public GITelemercadeoGiroDC ObtenerDetalleTelemercadeoGiro(long idTelemercadeoGiro)
        {
            return GIAdministradorTelemercadeoGiros.Instancia.ObtenerDetalleTelemercadeoGiro(idTelemercadeoGiro);
        }

        #endregion TelemercadeoGiros

        #region Centro Servicio

        /// <summary>
        /// Obtiene Todas las agencias a
        /// nivel nacional que pueden pagar Giros
        /// </summary>
        /// <returns>lista de agencias</returns>
        public IList<PUCentroServiciosDC> ObtenerAgencias()
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return GIAdministradorSolicitudes.Instancia.ObtenerCentrosServicios(idRacol);
        }

        #endregion Centro Servicio


        /// <summary>
        /// Obtiene la informacíón basica del Giro acorde a un numero de giro y un numero de identificacion bien sea de un Remitente o de un Destinatario
        /// </summary>
        /// <param name="numeroGiro">Número del giro</param>
        /// <param name="IdRemitente">Nro. de identificacion del Remitente</param>
        /// <param name="IdDestinatario">Nro. de identificacion del Destinatario</param>
        /// <returns>Objeto datos convenio</returns>
        public GIExploradorGirosWebDC ObtenerDatosGiros(GIExploradorGirosWebDC informacionGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerDatosGiros(informacionGiro);
        }
        /// <summary>
        /// obtiene los motivos de bloqueo
        /// </summary>
        /// <returns></returns>
        public List<GIMotivosInactivacion> ObtenerMotivosGiros()
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerMotivosGiros();
        }
        /// <summary>
        /// obtiene todas los usuarios internas
        /// </summary>
        /// <returns></returns>
        public List<PUArchivosPersonas> ObtenerTodasPersonasInternas()
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerTodasPersonasInternas();
        }
        /// <summary>
        /// agrega el motivo de bloqueo
        /// </summary>
        /// <param name="GIMotivo"></param>
        public void AgregarMotivoBloqueo(GIMotivosBloqueoDesbloqueo GIMotivo)
        {
            GIAdministradorExploradorGiros.Instancia.AgregarMotivoBloqueo(GIMotivo);
        }

    }
}