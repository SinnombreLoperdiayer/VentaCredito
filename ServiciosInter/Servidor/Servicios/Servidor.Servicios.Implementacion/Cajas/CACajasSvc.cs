using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.Cajas;
using CO.Servidor.Cajas.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Servicios.Implementacion.Cajas
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CACajasSvc : ICACajasSvc
    {
        /// <summary>
        /// Adiciona el movimientor caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, PAEnumConsecutivos? idConsecutivoComprobante)
        {
            return CAAdministradorCajas.Instancia.AdicionarMovimientoCaja(movimientoCaja, idConsecutivoComprobante);
        }

        /// <summary>
        /// Obtiene el centro servicio.
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return CAAdministradorCajas.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtener las cajas para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id punto centro servicio.</param>
        /// <returns>Las cajas que se pueden Cerrar</returns>
        public List<CACierreCajaDC> ObtenerCajasParaCerrar(long idPuntoCentroServicio, short idFormaPago, string operador)
        {
            return CAAdministradorCajas.Instancia.ObtenerCajasParaCerrar(idPuntoCentroServicio, idFormaPago, operador);
        }

        /// <summary>
        /// Cierra la caja y Totaliza Procesos.
        /// </summary>
        /// <param name="idApertura">Es el id de la apertura.</param>
        /// <param name="idPunto">Es el id del punto.</param>
        public void CerrarCaja(long idCodigoUsuario, long idApertura, long idPunto, int idCaja)
        {
            CAAdministradorCajas.Instancia.CerrarCaja(idCodigoUsuario, idApertura, idPunto, idCaja);
        }

        /// <summary>
        /// Obtiene el resumen por concepto de caja de un punto o centro de
        /// servicio para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id del punto ó centro de servicio.</param>
        /// <returns>la lista de los conceptos agrupados y totalizados para cerrar el punto</returns>
        public List<CAResumeCierrePuntoDC> ObtenerResumenCierrePunto(long idPuntoCentroServicio, long idCierrePuntoAsociado)
        {
            return CAAdministradorCajas.Instancia.ObtenerResumenCierrePunto(idPuntoCentroServicio, idCierrePuntoAsociado);
        }

        /// <summary>
        /// Obtiene el ultimo cierre del punto.
        /// </summary>
        /// <returns></returns>
        public CACierreCentroServicioDC ObtenerUltimoCierrePunto(long idPuntoCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerUltimoCierrePunto(idPuntoCentroServicio);
        }

        /// <summary>
        /// Obtienes the cierres punto.
        /// </summary>
        /// <param name="filtro">Es el filtro.</param>
        /// <param name="campoOrdenamiento">El campo de ordenamiento.</param>
        /// <param name="indicePagina">El indice pagina.</param>
        /// <param name="registrosPorPagina">Los registros por pagina.</param>
        /// <param name="ordenamientoAscendente">Sie s verdadero <c>true</c> es ascendente el ordenamiento.</param>
        /// <param name="totalRegistros">El total de Registros.</param>
        /// <param name="idPuntoCentroServicio">Es el Id del Punto de Servicio.</param>
        /// <returns>La lista de los 10 ultimos Cierres del punto</returns>
        public List<CACierreCentroServicioDC> ObtenerCierresPunto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idPuntoCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerCierresPunto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                   ordenamientoAscendente, out totalRegistros, idPuntoCentroServicio);
        }

        /// <summary>
        /// Cierra la Caja del Punto ó Centro de Servicio y
        /// actualiza el Id de Cierre de Punto en las Cajas respectivas
        /// como reportado.
        /// </summary>
        /// <param name="cajasPuntoCentroServicio">Informacion de Cierre de punto + lista de cajas del punto</param>
        public long CerrarCajaPuntoCentroServcio(CACierreCentroServicioDC cierrePuntoCentroServicio)
        {
            return CAAdministradorCajas.Instancia.CerrarCajaPuntoCentroServcio(cierrePuntoCentroServicio);
        }

        /// <summary>
        /// Obtiene las Cajas Cerradas y Pendientes
        /// para Cerrar de un punto o centro de Servicio
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="idFormaPago"></param>
        /// <param name="operador"></param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerCajasAReportarCajeroPrincipal(long idCentroSrv, short idFormaPago, string operador)
        {
            return CAAdministradorCajas.Instancia.ObtenerCajasAReportarCajeroPrincipal(idCentroSrv, idFormaPago, operador);
        }

        /// <summary>
        /// Obtiene el Valor a Enviar a la Empresa
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        public decimal ObtenerValorEmpresa(long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerValorEmpresa(idCentroServicio);
        }

        /// <summary>
        /// Inserta el dinero envio empresa.
        /// </summary>
        /// <param name="EnvioDineroAgencia">El envio dinero agencia.</param>
        public void InsertarDineroEnvioEmpresa(CARecoleccionDineroPuntoDC EnvioDineroAgencia)
        {
            CAAdministradorCajas.Instancia.InsertarDineroEnvioEmpresa(EnvioDineroAgencia);
        }

        /// <summary>
        /// Adiciona el dinero recibido por la Empresa
        /// de una agencio o punto.
        /// </summary>
        /// <param name="dineroRecibido">The dinero recibido.</param>
        public void AdicionarDineroAgencia(CARecoleccionDineroPuntoDC dineroRecibido)
        {
            CAAdministradorCajas.Instancia.AdicionarDineroAgencia(dineroRecibido);
        }

        /// <summary>
        /// Obtiene los mensajeros de apoyo para el envio de dinero.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicio(long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerMensajerosPuntoCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los suministros de un punto.
        /// </summary>
        /// <param name="centroServicio">The centro servicio.</param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosPunto(PUCentroServiciosDC centroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerSuministrosPunto(centroServicio);
        }

        /// <summary>
        /// Obtiene el dinero puntos reportados.
        /// </summary>
        /// <param name="idCentroServicio">es el id centro servicio.</param>
        /// <returns>la lista de los puntos con el dinero reportado</returns>
        public List<CARecoleccionDineroPuntoDC> ObtenerDineroReportadoPuntos(IDictionary<string, string> filtro, long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerDineroReportadoPuntos(filtro, idCentroServicio);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Obtener los Tipos Observ punto.
        /// </summary>
        /// <returns></returns>
        public List<CATipoObsPuntoAgenciaDC> ObtenerTiposObservPunto()
        {
            return CAAdministradorCajas.Instancia.ObtenerTiposObservPunto();
        }

        /// <summary>
        /// Adiciona las Transacciones de un Mensajero.
        /// </summary>
        /// <param name="registroMensajero">Clase Cuenta Mensajero.</param>
        public void AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero)
        {
            CAAdministradorCajas.Instancia.AdicionarTransaccMensajero(registroMensajero);
        }

        /// <summary>
        /// Obtiene los mensajeros de apoyo para el envio de dinero.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicioPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                                int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                                long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerMensajerosPuntoCentroServicioPag(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                          ordenamientoAscendente, out totalRegistros, idCentroServicio);
        }

        /// <summary>
        /// Adicionar Reporte del Mensajero.
        /// </summary>
        /// <param name="reportMensajero">Clase report mensajero.</param>
        public void AdicionarReporteMensajero(CAReporteMensajeroCajaDC reportMensajero)
        {
            CAAdministradorCajas.Instancia.AdicionarReporteMensajero(reportMensajero);
        }

        /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<CAReporteMensajeroCajaDC> ObtenerReportesMensajeros(long idMensajero)
        {
            return CAAdministradorCajas.Instancia.ObtenerReportesMensajeros(idMensajero);
        }

        /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero por comprobante para imprimir
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="numComprobante"></param>
        /// <returns></returns>
        public CADatosImpCompMensajeroDC ObtenerReportesMensajerosImprimir(long idMensajero, string numComprobante)
        {
            return CAAdministradorCajas.Instancia.ObtenerReportesMensajerosImprimir(idMensajero, numComprobante);
        }

        /// <summary>
        /// Recibir el dinero del mensajero.
        /// </summary>
        /// <param name="transaccionMensajero">Lista de Transacciones para el registro del dinero.</param>
        public CADatosImpCompMensajeroDC RecibirDineroMensajero(List<CARecibirDineroMensajeroDC> transaccionMensajero)
        {
            return CAAdministradorCajas.Instancia.RecibirDineroMensajero(transaccionMensajero);
        }

        /// <summary>
        /// Obtiene las entregas del mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <returns>Lista de Guias entregadas de alcobro  por mensajero</returns>
        public List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante)
        {
            return CAAdministradorCajas.Instancia.ObtenerEnviosPendMensajero(idMensajero, idComprobante);
        }

        /// <summary>
        /// Estados de la Cta mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <param name="fechaConsulta">The fecha consulta.</param>
        /// <returns></returns>
        public List<CACuentaMensajeroDC> ObtenerEstadoCtaMensajero(long idMensajero, DateTime fechaConsulta)
        {
            return CAAdministradorCajas.Instancia.ObtenerEstadoCtaMensajero(idMensajero, fechaConsulta);
        }

        /// <summary>
        /// Obtiene los conceptos de Caja por especificacion de
        /// visibilidad para mensajero - punto/Agencia - Racol.
        /// </summary>
        /// <param name="filtroCampoVisible">The filtro campo visible.</param>
        /// <returns>Lista de Conceptos de Caja por el filtro de Columna</returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCajaPorCategoria(int idCategoria)
        {
            return CAAdministradorCajas.Instancia.ObtenerConceptosCajaPorCategoria(idCategoria);
        }

        /// <summary>
        /// Actualiza la Observacion de trans.
        /// </summary>
        /// <param name="CuentaMensajero">The cuenta mensajero.</param>
        public void ActualizarObservacionEstadoCta(CACuentaMensajeroDC cuentaMensajero)
        {
            CAAdministradorCajas.Instancia.ActualizarObservacionEstadoCta(cuentaMensajero);
        }

        /// <summary>
        /// Obtiene los Prepagos vendidos por un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Prepagos vendidos</returns>
        public List<CAPinPrepagoDC> ObtenerPrepagosCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerPrepagosCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                ordenamientoAscendente, out totalRegistros, idCentroServicio);
        }

        /// <summary>
        /// Obtiene el detalle del pin prepago.
        /// </summary>
        /// <param name="pinPrepago">The pin prepago.</param>
        /// <returns>Lista de las compras realizadas con un pin prepago</returns>
        public List<CAPinPrepagoDtllCompraDC> ObtenerDtllCompraPinPrepago(long pinPrepago)
        {
            return CAAdministradorCajas.Instancia.ObtenerDtllCompraPinPrepago(pinPrepago);
        }

        /// <summary>
        /// Adiciona la venta de un Pin Prepago.
        /// </summary>
        /// <param name="ventaPinPrepago">The venta pin prepago.</param>
        public void AdicionarPinPrepago(CAPinPrepagoDC pinPrepago)
        {
            CAAdministradorCajas.Instancia.AdicionarPinPrepago(pinPrepago);
        }

        /// <summary>
        /// Venta del Pin prepago.
        /// </summary>
        /// <param name="ventaPinPrepago">The venta pin prepago.</param>
        public void VenderPinPrepago(CAVenderPinPrepagoDC ventaPinPrepago)
        {
            CAAdministradorCajas.Instancia.VenderPinPrepago(ventaPinPrepago);
        }

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return GCAdministradorCajas.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene a la persona de la lista restrictiva por tipo de restriccion.
        /// </summary>
        /// <param name="documento">The documento.</param>
        /// <param name="tipoLista">The tipo lista.</param>
        /// <returns></returns>
        public bool ObtenerPersonaListaRestrictiva(string documento)
        {
            return CAAdministradorCajas.Instancia.ObtenerPersonaListaRestrictiva(documento);
        }

        /// <summary>
        /// Obtiene el valor del parametro configurado en la tabla de parametrso caja.
        /// </summary>
        /// <param name="idParametro">The id parametro.</param>
        /// <returns></returns>
        public string ObtenerParametroCajas(string idParametro)
        {
            return CAAdministradorCajas.Instancia.ObtenerParametroCajas(idParametro);
        }

        /// <summary>
        /// Validar el saldo del Prepago para descontar del
        /// saldo.
        /// </summary>
        /// <param name="idPinPrepago">el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">es el valor de la compra con el pin prepago.</param>
        /// <returns></returns>
        public void ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago)
        {
            CAAdministradorCajas.Instancia.ValidarSaldoPrepago(pinPrepago, valorCompraPinPrepago);
        }

        /// <summary>
        /// Obtener la info de la caja para cierre.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <param name="idFormaPago">The id forma pago.</param>
        /// <param name="operador">The operador.</param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerInfoCierreCaja(long idUsuario, short idFormaPago, string operador, long idCentroServicio, int idCaja)
        {
            return CAAdministradorCajas.Instancia.ObtenerInfoCierreCaja(idUsuario, idFormaPago, operador, idCentroServicio, idCaja);
        }

        /// <summary>
        /// Obtener Cierres del Cajero.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerCierresCajero(long idCodigoUsuario, long idCentroServicio, DateTime fechaCierre, int indicePagina, int tamanoPagina)
        {
            return CAAdministradorCajas.Instancia.ObtenerCierresCajero(idCodigoUsuario, idCentroServicio, fechaCierre, indicePagina, tamanoPagina);
        }

        /// <summary>
        /// Obtiene a los Cajeros Auxiliares de un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol)
        {
            return CAAdministradorCajas.Instancia.ObtenerCajeroCentroServicio(idCentroServicio, idRol);
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de Comprobante de Caja ingreso
        /// </summary>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoComprobateCajaIngreso()
        {
            return CAAdministradorCajas.Instancia.ObtenerConsecutivoComprobateCajaIngreso();
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de Comprobante de Caja egreso
        /// </summary>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoComprobateCajaEgreso()
        {
            return CAAdministradorCajas.Instancia.ObtenerConsecutivoComprobateCajaEgreso();
        }

        /// <summary>
        /// Obtener los datos de la empresa (casa matriz según parámetros del Framework
        /// </summary>
        /// <returns>Información de la empresa</returns>
        public AREmpresaDC ObtenerDatosEmpresa()
        {
            return CAAdministradorCajas.Instancia.ObtenerDatosEmpresa();
        }

        /// <summary>
        /// Actualiza el cierre de caja como Caja Reportada al cajero Ppal
        /// </summary>
        /// <param name="idCierreCajaAux">The id cierre caja aux.</param>
        public void ReportarCajaACajeroPrincipal(CARegistroTransacCajaDC movimientoCaja, long idCierreCaja)
        {
            CAAdministradorCajas.Instancia.ReportarCajaACajeroPrincipal(movimientoCaja, idCierreCaja);
        }

        /// <summary>
        /// Cierre automatico de Cajas Auxiliares ejecuta por el
        /// Cajero ppal para cerrar la Caja del Punto o centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <param name="idUsuario">The id usuario.</param>
        public void CerrarCajasAutomaticamentePorCentroSvc(CARegistroTransacCajaDC movimientoCaja)
        {
            CAAdministradorCajas.Instancia.CerrarCajasAutomaticamentePorCentroSvc(movimientoCaja);
        }

        /// <summary>
        /// Obtiene la Ultima apertura activa del usuario por Centro de
        /// servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCaja"></param>
        /// <param name="idCodUsuario"></param>
        /// <returns>el id long de la ultima apertura</returns>
        public long ObtenerUltimaAperturaActiva(long idCentroServicio, int idCaja, long idCodUsuario)
        {
            return CAAdministradorCajas.Instancia.ObtenerUltimaAperturaActiva(idCentroServicio, idCaja, idCodUsuario);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return CAAdministradorCajas.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Obtiene los consecutivos de ingreso y egreso para el formato
        /// de translado de dinero entre cajas ppal y Auxiliar
        /// </summary>
        /// <returns></returns>
        public PAConsecutivoIngresoEgresoDC ObtenerConsecutivoIngresoEgreso()
        {
            return CAAdministradorCajas.Instancia.ObtenerConsecutivoIngresoEgreso();
        }

        /// <summary>
        /// Metodo que realiza la transaccion de
        /// obtener la dupla y realiza la transaccion
        /// entre Caja Ppal y Caja Auxiliar
        /// </summary>
        /// <param name="infoTransaccion"></param>
        public void TransladarDineroEntreCajas(CAOperaRacolBancoEmpresaDC infoTransaccion)
        {
            CAAdministradorCajas.Instancia.TransladarDineroEntreCajas(infoTransaccion);
        }

        /// <summary>
        /// Obtener información complementaria para el cierre de una caja
        /// </summary>
        /// <param name="idCierre">Identificacdor del cierre de caja</param>
        /// <returns>Información complementaria del cierre de caja</returns>
        public CAInfoComplementariaCierreCajaDC ObtenerInfoComplementariaCierreCaja(long idCierreCaja)
        {
            return CAAdministradorCajas.Instancia.ObtenerInfoComplementariaCierreCaja(idCierreCaja);
        }

        public CAConsolidadoCierreDC ObtenerResumenCierreCajaPrincipal(long idCentroServicios, long idAperturaCaja, long idCierrePuntoAsociado)
        {
            return CAAdministradorCajas.Instancia.ObtenerResumenCierreCajaPrincipal(idCentroServicios, idAperturaCaja, idCierrePuntoAsociado);
        }

        /// <summary>
        /// Obtiene la Info de la Pantalla de
        /// RegistroEnvioAgencia
        /// </summary>
        /// <param name="idCentroServicio">idcentroServicio</param>
        /// <returns>info de registroEnvio</returns>
        public CARegistrarEnvioAgenciaDC ObtenerInfoRegistroEnvioAgencia(long idCentroServicio)
        {
            return CAAdministradorCajas.Instancia.ObtenerInfoRegistroEnvioAgencia(idCentroServicio);
        }

        /// <summary>
        /// Procedimientos para el descuento y generacion de la informacion
        /// para el formato de descuento de nomina
        /// </summary>
        /// <param name="registroMensajero">datos del mensajero a descontar</param>
        /// <returns>info de impresion formato</returns>
        public CADatosImpCompMensajeroDC DescuentoNominaMensajero(CACuentaMensajeroDC registroMensajero)
        {
            return CAAdministradorCajas.Instancia.DescuentoNominaMensajero(registroMensajero);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns>lista de Clientes</returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            return CAAdministradorCajas.Instancia.ObtenerClientes();
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro">Numero del giro a consultar</param>
        /// <returns>informacion del giro</returns>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            return CAAdministradorCajas.Instancia.ConsultarGiroXNumGiro(idGiro);
        }
        
    }
}