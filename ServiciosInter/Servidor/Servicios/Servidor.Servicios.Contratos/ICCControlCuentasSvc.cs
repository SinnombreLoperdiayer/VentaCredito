using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
   public interface ICCControlCuentasSvc
   {
      /// <summary>
      /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
      /// </summary>
      /// <param name="idAdmisionMensajeria"></param>
      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia);

        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision);

        /// <summary>
        /// Calcula el precio de un guia para cambio de destino
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        decimal ReLiquidacion(ADGuia guia);

        /// <summary>
        /// Recalcula la prima de un factura
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        decimal ReLiquidacionPrima(ADGuia guia);

        /// <summary>
        /// Obtener el empleado en NovaSoft
        /// </summary>
        /// <param name="identificacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCEmpleadoNovaSoftDC ObtenerEmpleadoNovaSoft(string identificacion);

        /// <summary>
        /// Crear novedad cambio de destino
        /// </summary>
        /// <param name="novedadGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearNovedadCambioDestino(CCNovedadCambioDestinoDC novedadGuia);

        /// <summary>
        /// Crear novedad cambio de peso
        /// </summary>
        /// <param name="novedadGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearNovedadCambioPeso(CCNovedadCambioPesoDC novedadGuia);

        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearNovedadCambioRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia);

        /// <summary>
        /// crear novedad cambio forma de pago
        /// </summary>
        /// <param name="novedadGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearNovedadFormaPago(CCNovedadCambioFormaPagoDC novedadGuia);

        /// <summary>
        /// Crea novedad de cambio de tipo de servicio en una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearNovedadCambioTipoServicio(CCNovedadCambioTipoServicio novedadGuia);

        /// <summary>
        /// Crea novedad de cambio de valor total de una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearNovedadCambioValorTotal(CCNovedadCambioValorTotal novedadGuia);

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia);

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia);

        /// <summary>
        /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia);

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio);

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <returns>Colección de guías</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerContext))]
        List<PUCentroServiciosDC> ObtenerAgencias();


        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia y un cliente
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <param name="cliente">Objeto Cliente</param>
        /// <returns>Colección guías de mensajería</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasClienteCredito(PUCentroServiciosDC agencia, CLClientesDC cliente, DateTime fechaInicial, DateTime fechaFinal, int idSucursal);

        /// <summary>
        /// Obtiene las guías de mensajería a partir de una agencia y un mensajero
        /// </summary>
        /// <param name="agencia">Objeto Agencia</param>
        /// <param name="cliente">Objeto Cliente</param>
        /// <returns>Colección guías de mensajería</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasMensajero(PUCentroServiciosDC agencia, OUNombresMensajeroDC mensajero, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Adiciona un registro al almacen de control de cuentas
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCAlmacenDC AdicionarAlmacenControlCuentas(CCAlmacenDC almacen);

        /// <summary>
        /// Adiciona al almacén de control de cuentas una guía anulada
        /// </summary>
        /// <param name="almacen"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCAlmacenDC AdicionarAlmacenGuiaAnulada(CCAlmacenDC almacen);

        /// <summary>
        /// Adiciona un registro al almacen de control de cuentas sin archivar, es decir que no adicionar lote, posición ni caja
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCAlmacenDC AdicionarAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarVariosAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen, List<long> operaciones);

        /// <summary>
        /// Obtiene los giros de una agencia
        /// </summary>
        /// <returns>Colección giros</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<GIAdmisionGirosDC> ObtenerGirosAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta);

        /// <summary>
        /// Obtiene las admisiones y pagos de una agencia
        /// </summary>
        /// <param name="agencia">Objeto agencia</param>
        /// <returns>Colección admisiones pagos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PGPagoAdmisionGiroDC> ObtenerAdmisionPagoGiroAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta);

        /// <summary>
        /// Obtiene los gastos de caja
        /// </summary>
        /// <param name="agencia">Objeto agencia</param>
        /// <returns>Colección de gastos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAMovimientoCajaDC> ObtenerGastosCaja(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta);

        /// <summary>
        /// Obtiene los movimientos de caja para un centro de servicio dado en un rago de fechas que difieren de ventas de mensajería, 
        /// pago de al cobros, venta de giros, pago de giros y ventas de pines prepago.
        /// </summary>
        ///<param name="agencia"></param>
        ///<param name="fechaFinal"></param>
        ///<param name="fechaInicial"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAMovimientoCajaDC> ObtenerOtrosMovimientosCaja(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Retorna las ventas de pin prepago realizads por la gencia en el rango de fechas dado
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAMovimientoCajaDC> ObtenerVentasPinPrepago(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Retorna las operaciones de caja que tengan el concepto de pago "Al Cobro" realizadas por la agencia en el rango de fechas dado
        /// </summary>
        /// <param name="agencia"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CAMovimientoCajaDC> ObtenerRecaudosAlCobro(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion();

        /// <summary>
        /// Anula una guía de mensajería
        /// </summary>
        /// <param name="anulacion">Objeto</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCResultadoAnulacionGuia AnularGuia(CCAnulacionGuiaMensajeriaDC anulacion);

        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarAdmisionAnulada(CCAnulacionGuiaMensajeriaDC guia);

        /// <summary>
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio);

        /// <summary>
        /// Ejecuta toda la lógica para cambiar una factura de forma de pago al cobro a crédito
        /// </summary>
        /// <param name="cambioFPAlCobroCredito">Datos del cambio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CambiarFPAlCobroACredito(CCNovedadFPAlCobroCreditoDC cambioFPAlCobroCredito);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        decimal RegistrarEncabezadoCargueArchivoAjuste(CCEncabezadoArchivoAjusteGuiaDC encabezadoArchivo);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RegistarDetalleCargueArchivoAjuste(CCDetalladoArchivoAjusteGuiaDC detalladoArchivo);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarEncabezadoCargueArchivoAjuste(long IdArchivo, short idEstado);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CCEncabezadoArchivoAjusteGuiaDC> ConsultarUltimosRegistrosCargueArchivoUsuario(string usuario);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<long> ConsultarIdsArchivoNoProcesados(long idArchivo);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ProcesarRegistroArchivo(long idRegistro, string usuario);

        #region Control de cuentas / Novedades Inter Logis App

        /// <summary>
        /// Metodo para obtener la informacion de la guia para auditar liquidación
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCRespuestaAuditoriaDC ObtenerGuiaAuditoriaLiquidacion(long NumeroGuia);

        /// <summary>
        /// Metodo para insertar novedad control de liquidacion 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCRespuestaAuditoriaDC InsertarNovedadControlLiquidacion(CCGuiaDC guia);

        /// <summary>
        /// Metodo para consultar lista de guias de novedades control liquidacion 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<CCGuiaIdAuditoria> ConsultarGuiasNovedadesControlLiquidacion(int indicePagina, int registrosPorPagina);
        /// <summary>
        /// Metodo para obtener una lista con los tipos de novedades de una guia  
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<short> ObtenerTipoNovedadesGuia(long numeroGuia, int IdEstadoNovedad);
        /// <summary>
        /// metodo para insertar traza de una novedad presentada en una guia desde financiera
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarTrazaNovedadControlLiquidacion(int pk, CCGuiaDC guia);
        /// <summary>
        /// obtiene peso volumetrico por guia
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        //[FaultContract(typeof(ControllerException))]
        PesoVolGuiaDC ObtenerPesoVolumetricoGuia(long numeroGuia);
        /// <summary>
        /// obtiene imagenes por numero de guia
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ObtenerImagenesNovedadGuia(long numeroGuia);
        /// <summary>
        /// Envia correo al destinatario de acuerdo a la auditoria por peso realizada
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="asunto"></param>
        /// <param name="nombreAdjuntos"></param>
        /// <param name="remitente"></param>
        /// <param name="displayRemitente"></param>
        /// <param name="passwordRemitente"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EnviarComunicadoCServicio(string destinatario, string asunto, string nombreAdjuntos, string remitente = null,
                                               string displayRemitente = null, string passwordRemitente = null);


        /// <summary>
        /// Valida si el cupo de un clinte se ha excedido
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool validarCupoClienteCredito(int idCliente, decimal valorTransaccion);

        /// <summary>
        /// Calcula el precio de un guia para cambio de destino
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCBolsaNovedadesReliquidacionDC ReLiquidacionBolsaNovedades(ADGuia guia);


        /// <summary>
        /// Metodo para insertar novedades de control de liquidacion de auditoria de pesos, realizando comunicado 
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        CCRespuestaAuditoriaDC InsertarNovedadAuditoriaPorPeso(CCGuiaDC guia, ADGuiaUltEstadoDC guiaSinAuditar, CCEmpleadoNovaSoftDC empleadoNovasoft, bool guardatraza);
            #endregion
    }
}