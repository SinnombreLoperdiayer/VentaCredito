using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Comun.Util;
using CO.Servidor.Clientes;

namespace CO.Servidor.Servicios.Implementacion.ControlCuentas
{
    /// <summary>
    /// Clase para los servicios de administración del Control de Cuentas
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CCControlCuentasSvc : ICCControlCuentasSvc
    {
        /// <summary>
        /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerMensajeriaUltimoEstado(idNumeroGuia);
        }


        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        public ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerMensajeriaFormaPago(idAdmision);
        }

        /// <summary>
        /// Calcula el precio de un guia para cambio de destino
        /// </summary>
        public decimal ReLiquidacion(ADGuia guia)
        {
            return CCAdministradorControlCuentas.Instancia.ReLiquidacion(guia);
        }

        /// <summary>
        /// Calcula el precio de un guia para cambio de destino
        /// </summary>
        public CCBolsaNovedadesReliquidacionDC ReLiquidacionBolsaNovedades(ADGuia guia)
        {
            return CCAdministradorControlCuentas.Instancia.ReLiquidacionBolsaNovedades(guia);
        }

        /// <summary>
        /// Recalcula la prima de un factura
        /// </summary>
        public decimal ReLiquidacionPrima(ADGuia guia)
        {
            return CCAdministradorControlCuentas.Instancia.ReLiquidacionPrima(guia);
        }

        /// <summary>
        /// Obtener el empleado en NovaSoft
        /// </summary>
        /// <param name="identificacion"></param>
        public CCEmpleadoNovaSoftDC ObtenerEmpleadoNovaSoft(string identificacion)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerEmpleadoNovaSoft(identificacion);
        }

        /// <summary>
        /// Crear novedad cambio de destino
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void CrearNovedadCambioDestino(CCNovedadCambioDestinoDC novedadGuia)
        {
            CCAdministradorControlCuentas.Instancia.CrearNovedadCambioDestino(novedadGuia);
        }

        /// <summary>
        /// Crear novedad cambio de peso
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void CrearNovedadCambioPeso(CCNovedadCambioPesoDC novedadGuia)
        {
            CCAdministradorControlCuentas.Instancia.CrearNovedadCambioPeso(novedadGuia);
        }

        /// <summary>
        /// crear novedad cambio forma de pago
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void CrearNovedadFormaPago(CCNovedadCambioFormaPagoDC novedadGuia)
        {
            CCAdministradorControlCuentas.Instancia.CrearNovedadFormaPago(novedadGuia);
        }

        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void CrearNovedadCambioRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia)
        {
            CCAdministradorControlCuentas.Instancia.ActualizarRemitenteDestinatarioGuia(novedadGuia);
        }

        /// <summary>
        /// Crea novedad de cambio de tipo de servicio en una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void CrearNovedadCambioTipoServicio(CCNovedadCambioTipoServicio novedadGuia)
        {
            CCAdministradorControlCuentas.Instancia.CrearNovedadCambioTipoServicio(novedadGuia);
        }

        /// <summary>
        /// Crea novedad de cambio de valor total de una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void CrearNovedadCambioValorTotal(CCNovedadCambioValorTotal novedadGuia)
        {
            CCAdministradorControlCuentas.Instancia.CrearNovedadCambioValorTotal(novedadGuia);
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerNombreMensajeroAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerClientesContratosXAgencia(idAgencia);
        }

        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerCLientesContratosXAgenciaDependientes(idAgencia);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerPuntosAgenciasDependientes(idCentroServicio);
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias()
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerAgencias();
        }


        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <returns>Colección de guías</returns>
        public List<ADGuia> ObtenerGuiasAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerGuiasAgencia(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia y un cliente
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <param name="cliente">Objeto Cliente</param>
        /// <returns>Colección guías de mensajería</returns>
        public List<ADGuia> ObtenerGuiasClienteCredito(PUCentroServiciosDC agencia, CLClientesDC cliente, DateTime fechaInicial, DateTime fechaFinal, int idSucursal)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerGuiasClienteCredito(agencia, cliente, fechaInicial, fechaFinal, idSucursal);
        }

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia y un mensajero
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <param name="cliente">Objeto Cliente</param>
        /// <returns>Colección guías de mensajería</returns>
        public List<ADGuia> ObtenerGuiasMensajero(PUCentroServiciosDC agencia, OUNombresMensajeroDC mensajero, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerGuiasMensajero(agencia, mensajero, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Adiciona un registro al almacen de control de cuentas
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        public CCAlmacenDC AdicionarAlmacenControlCuentas(CCAlmacenDC almacen)
        {
            return CCAdministradorControlCuentas.Instancia.AdicionarAlmacenControlCuentas(almacen);
        }

        /// <summary>
        /// Adiciona al almacén de control de cuentas una guía anulada
        /// </summary>
        /// <param name="almacen"></param>
        /// <returns></returns>
        public CCAlmacenDC AdicionarAlmacenGuiaAnulada(CCAlmacenDC almacen)
        {
            return CCAdministradorControlCuentas.Instancia.AdicionarAlmacenGuiaAnulada(almacen);
        }

        /// <summary>
        /// Adiciona un registro al almacen de control de cuentas sin archivar, es decir que no adicionar lote, posición ni caja
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        public CCAlmacenDC AdicionarAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen)
        {
            return CCAdministradorControlCuentas.Instancia.AdicionarAlmacenControlCuentasSinArchivar(almacen);
        }

        /// <summary>
        /// Adicionar varios registros de almacén de control de cuentas sin archivar.
        /// </summary>
        /// <param name="almacen"></param>
        /// <param name="operaciones"></param>
        public void AdicionarVariosAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen, List<long> operaciones)
        {
            CCAdministradorControlCuentas.Instancia.AdicionarVariosAlmacenControlCuentasSinArchivar(almacen, operaciones);
        }

        /// <summary>
        /// Obtiene los giros de una agencia
        /// </summary>
        /// <returns>Colección giros</returns>
        public List<GIAdmisionGirosDC> ObtenerGirosAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerGirosAgencia(agencia, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene las admisiones y pagos de una agencia
        /// </summary>
        /// <param name="agencia">Objeto agencia</param>
        /// <returns>Colección admisiones pagos</returns>
        public List<PGPagoAdmisionGiroDC> ObtenerAdmisionPagoGiroAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerAdmisionPagoGiroAgencia(agencia, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene los gastos de caja
        /// </summary>
        /// <param name="agencia">Objeto agencia</param>
        /// <returns>Colección de gastos</returns>
        public List<CAMovimientoCajaDC> ObtenerGastosCaja(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerGastosCaja(agencia, fechaDesde, fechaHasta);
        }

        /// <summary>
        /// Obtiene los movimientos de caja para un centro de servicio dado en un rago de fechas que difieren de ventas de mensajería, 
        /// pago de al cobros, venta de giros, pago de giros y ventas de pines prepago.
        /// </summary>
        ///<param name="agencia"></param>
        ///<param name="fechaFinal"></param>
        ///<param name="fechaInicial"></param>
        public List<CAMovimientoCajaDC> ObtenerOtrosMovimientosCaja(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerOtrosMovimientosCaja(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Retorna las ventas de pin prepago realizads por la gencia en el rango de fechas dado
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<CAMovimientoCajaDC> ObtenerVentasPinPrepago(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerVentasPinPrepago(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Retorna las operaciones de caja que tengan el concepto de pago "Al Cobro" realizadas por la agencia en el rango de fechas dado
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<CAMovimientoCajaDC> ObtenerRecaudosAlCobro(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerRecaudosAlCobro(agencia, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
        {
            return CCAdministradorControlCuentas.Instancia.ObtenerMotivosAnulacion();
        }

        /// <summary>
        /// Anula una guía de mensajería
        /// </summary>
        /// <param name="anulacion">Objeto</param>
        public CCResultadoAnulacionGuia AnularGuia(CCAnulacionGuiaMensajeriaDC anulacion)
        {
            return CCAdministradorControlCuentas.Instancia.AnularGuia(anulacion);
        }

        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        public long AdicionarAdmisionAnulada(CCAnulacionGuiaMensajeriaDC guia)
        {
            return CCAdministradorControlCuentas.Instancia.AdicionarAdmisionAnulada(guia);
        }

        /// <summary>
        /// Ejecuta toda la lógica para cambiar una factura de forma de pago al cobro a crédito
        /// </summary>
        /// <param name="cambioFPAlCobroCredito">Datos del cambio</param>
        public void CambiarFPAlCobroACredito(CCNovedadFPAlCobroCreditoDC cambioFPAlCobroCredito)
        {
            CCControlCuentas.Instancia.CambiarFPAlCobroACredito(cambioFPAlCobroCredito);
        }

        public decimal RegistrarEncabezadoCargueArchivoAjuste(CCEncabezadoArchivoAjusteGuiaDC encabezadoArchivo)
        {
            return CCControlCuentas.Instancia.RegistrarEncabezadoCargueArchivoAjuste(encabezadoArchivo);
        }

        public void RegistarDetalleCargueArchivoAjuste(CCDetalladoArchivoAjusteGuiaDC detalladoArchivo)
        {
            CCControlCuentas.Instancia.RegistarDetalleCargueArchivoAjuste(detalladoArchivo);
        }

        public void ActualizarEncabezadoCargueArchivoAjuste(long IdArchivo, short idEstado)
        {
            CCControlCuentas.Instancia.ActualizarEncabezadoCargueArchivoAjuste(IdArchivo, idEstado);
        }

        public List<CCEncabezadoArchivoAjusteGuiaDC> ConsultarUltimosRegistrosCargueArchivoUsuario(string usuario)
        {
            return CCControlCuentas.Instancia.ConsultarUltimosRegistrosCargueArchivoUsuario(usuario);
        }

        public List<long> ConsultarIdsArchivoNoProcesados(long idArchivo)
        {
            return CCControlCuentas.Instancia.ConsultarIdsArchivoNoProcesados(idArchivo);
        }

        public void ProcesarRegistroArchivo(long idRegistro, string usuario)
        {
            CCControlCuentas.Instancia.ProcesarRegistroArchivo(idRegistro, usuario);
        }

        #region Control de cuentas / Novedades de Inter Logis App 

        /// <summary>
        /// Metodo para obtener cantidades de guias por auditar
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
        public CCRespuestaAuditoriaDC ObtenerCantidadGuiasPorAuditar(long idCentroLogistico)
        {
            return CCNovedadesApp.Instancia.ObtenerCantidadGuiasPorAuditar(idCentroLogistico);
        }

        /// <summary>
        /// Metodo para obtener la informacion de la guia para auditar su liquidación
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public CCRespuestaAuditoriaDC ObtenerGuiaAuditoriaLiquidacion(long NumeroGuia)
        {
            return CCNovedadesApp.Instancia.ObtenerGuiaAuditoriaLiquidacion(NumeroGuia);
        }

        /// <summary>
        /// Valida si el cupo de un clinte se ha excedido
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        /// <returns></returns>
        public bool validarCupoClienteCredito(int idCliente, decimal valorTransaccion)
        {
            bool avisoPorcentajeMinimoAviso = CCNovedadesApp.Instancia.validarCupoClienteCredito(idCliente, valorTransaccion);

            return avisoPorcentajeMinimoAviso;
        }

        /// <summary>
        /// Metodo para insertar novedades de control de liquidacion de auditoria de pesos, realizando comunicado 
        /// </summary>
        /// <param name="guia"></param>
        public CCRespuestaAuditoriaDC InsertarNovedadAuditoriaPorPeso(CCGuiaDC guia, ADGuiaUltEstadoDC guiaSinAuditar, CCEmpleadoNovaSoftDC empleadoNovasoft, bool guardatraza)
        {
            CCRespuestaAuditoriaDC respuesta = CCNovedadesApp.Instancia.InsertarNovedadAuditoriaPorPeso(guia, guiaSinAuditar, empleadoNovasoft, guardatraza);
            return respuesta;
        }

        /// <summary>
        /// Metodo para insertar novedades contorl de liquidacion 
        /// </summary>
        /// <param name="guia"></param>
        public CCRespuestaAuditoriaDC InsertarNovedadControlLiquidacion(CCGuiaDC guia)
        {
            return CCNovedadesApp.Instancia.InsertarNovedadControlLiquidacion(guia);
        }
        /// <summary>
        /// metodo para insertar traza de una novedad gestionada presentada en una guia desde financiera
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="guia"></param>
        public void InsertarTrazaNovedadControlLiquidacion(int pk, CCGuiaDC guia)
        {
            CCNovedadesApp.Instancia.InsertarTrazaNovedadControlLiquidacion(pk, guia);

        }
        /// <summary>
        /// 
        /// </summary>
        public void EnviarComunicadoCServicio(string destinatario, string asunto, string nombreAdjuntos, string remitente = null, 
                                               string displayRemitente = null, string passwordRemitente = null)
        {
            CorreoElectronico.Instancia.Enviar(destinatario, asunto, nombreAdjuntos, remitente, displayRemitente, passwordRemitente);
        }
        /// <summary>
        /// Metodo para consultar lista de guias de novedades control liquidacion 
        /// </summary>
        /// <returns></returns>
        public List<CCGuiaIdAuditoria> ConsultarGuiasNovedadesControlLiquidacion(int indicePagina, int registrosPorPagina)
        {
            return CCNovedadesApp.Instancia.ConsultarGuiasNovedadesControlLiquidacion(indicePagina, registrosPorPagina);
        }
        /// <summary>
        /// Metodo para obtener una lista con los tipos de novedades de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<short> ObtenerTipoNovedadesGuia(long numeroGuia, int IdEstadoNovedad)
        {
            return CCNovedadesApp.Instancia.ObtenerTipoNovedadesGuia(numeroGuia, IdEstadoNovedad);
        }
        /// <summary>
        /// Metodo para obtener peso volumetrico de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PesoVolGuiaDC ObtenerPesoVolumetricoGuia(long numeroGuia)
        {
            var guia = CCNovedadesApp.Instancia.ObtenerPesoVolumetricoGuia(numeroGuia);
            return guia;
        }
        /// <summary>
        /// obtiene imagenes por numero de guia
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesNovedadGuia(long numeroGuia)
        {
            return CCNovedadesApp.Instancia.ObtenerImagenesNovedadGuia(numeroGuia);
        }
        #endregion
    }
}