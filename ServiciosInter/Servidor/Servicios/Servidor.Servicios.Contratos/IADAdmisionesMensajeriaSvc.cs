using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IADAdmisionesMensajeriaSvc
    {
        /// <summary>
        /// Genera solicitud de falla
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="valorCobrado"></param>
        /// <param name="valorCalculado"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GenerarFallaCalculoValorGuiaManual(long numeroGuia, decimal valorCobrado, decimal valorCalculado);

        /// <summary>
        /// Se registra un movimiento de mensajería internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque);

        /// <summary>
        /// Se registra un movimiento de mensajería internacional con DHL
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaAutomaticaInternacional_DHL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, ADGuiaInternacionalDC guiaInternacional);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualInternacionalCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal);

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad();

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion();

        /// <summary>
        /// Valida si el servicio está habilidado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para dicho trayecto
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, decimal pesoGuia, DateTime? fechadmisionEnvio);

        // TODO ID: Se adiciona este metodo para traer los datos del centro de Servicio Destino(AGE), cuando se activa en Admisiones el Tipo de entrega "RECLAME EN OFICINA"
        /// <summary>
        /// Valida la Agencia-Centro de Servicio Destino para tipo de Entrega "RECLAME EN OFICINA"
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADValidacionServicioTrayectoDestino ValidarCentroServicioDestino(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, long centroServiciosOrigen);

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para
        /// dicho trayecto, la duración en días y la prima de seguro acordaba con el cliente en el contrato
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="esDesdeSucursal">Indica si la transacción se va a realizar desde la propia sucursal del cliente o si se hace desde una agencia de Interrapidísimo</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idCliente">Identificador del cliente crédito</param>
        /// <param name="idListaPrecio">Identificador de la lista de precio asociada al contrato</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idSucursal, int idCliente, int idListaPrecio, decimal pesoGuia, DateTime? fechadmisionEnvio);

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<ADTipoEntrega> ObtenerTiposEntrega();

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADParametrosAdmisiones ObtenerParametrosAdmisiones();

                /// <summary>
        /// obtiene los parametros de encabezado de guia
        /// </summary>
        /// <param name="llave"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerParametrosEncabezado(string llave);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerImagenPublicitariaGuia();

        /// <summary>
        /// Retorna la información de una guía completa incluyendo la forma como se pagó, se construyó para generar impresión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long? idCentroServicios, int? idSucursal, int? idCliente);

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia);

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long? idCentroServicios, int? idSucursal, int? idCliente);

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long? idCentroServicios, int? idSucursal, int? idCliente);

        #region Cálculo de precios en tarifas

        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios);

        /// <summary>
        /// Calcular el precio para una tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado);

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioTramiteDC CalcularPrecioTramites(int idListaPrecios, int idTramite);

        /// <summary>
        /// Obtiene el precio del servicio rapi radicado
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiradicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiValoresMsj(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiValoresCarga(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiCargaConsolidado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiValijas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision);

        /// <summary>
        /// Obtiene el precio del servicio carga express
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TAPreciosAgrupadosDC> CalcularPrecioServicios(List<int> servicios, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega);

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad);

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioCargaDC CalcularPrecioRapiCargaContraPagoTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        #endregion Cálculo de precios en tarifas

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario);

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaAutomaticaNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion);

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaAutomaticaRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago);

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaAutomaticaRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion);

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualNotificacionCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago);

        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualRapiEnvioContraPagoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado);

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision RegistrarGuiaManualRapiRadicadoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRetornoAdmision ObtenerGuiaPorGuid(string guid);

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SUPropietarioGuia ObtenerPropietarioGuia(long numeroGuia, long idSucursalCentroServicio);

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long idCentroServicio);

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarPagadoGuia(long idAdmisionMensajeria);

        /// <summary>
        /// Actualiza la guía y registra el valor en Caja de la transaccion.
        /// </summary>
        /// <param name="guiaAlCobro">The guia al cobro.</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRecaudoAlCobro ActualizarGuiaAlCobro(ADRecaudarDineroAlCobroDC guiaAlCobro);

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuiaInternaDC AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna);

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumeroGuias);

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias);

        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio);

        /// <summary>
        /// Validar la existencia de una orden de servicio masiva
        /// </summary>
        /// <param name="ordenServicio">número de la orden de servicio a validar</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarOrdenServicio(long ordenServicio);

        /// <summary>
        /// Valida cada uno de los envíos que se desea asociar a una orden de servicio antes de almacenarla en la base de datos
        /// </summary>
        /// <param name="enviosParaValidar">Listado de los envíos que se desean validar</param>
        /// <returns>Listado de los envíos validados y su respectivo resultado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADDatosValidadosDC> ValidarDatosGuiasOrdenMasiva(List<ADDatosValidacionDC> enviosParaValidar, bool esAutomatica);

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex);

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio);

        /// <summary>
        /// Se registra un movimiento de mensajería asociado a una orden de servicio. Esto se utiliza en
        /// cuando se hace cargue masivo de guias o facturas.
        /// </summary>
        /// <param name="guia">Datos de la guia</param>
        /// <param name="idCaja">Caja que hace la operacion</param>
        /// <param name="remitenteDestinatario">Datos del remitente y el destinatario</param>
        /// <returns>Número de guía</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADRetornoAdmision> RegistrarGuiasDeOrdenDeServicio(List<ADGuiaOSDC> guias, bool conIngresoCentroAcopio);

        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision);

        /// <summary>
        /// Obtiene el listado de
        /// las ordenes de servicio por fecha
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>lista de ordenes por fecha</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADOrdenServicioMasivoDC> ObtenerOrdenesServicioPorFecha(DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Método para obtener la información de una guía rapiradicado
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADRapiRadicado ObtenerAdmisionRapiradicado(long numeroGuia);

        /// <summary>
        /// obtiene admisiones sin entregar
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]

        List<ADGuia> ObtenerAdmisionMensajeriaSinEntregar(AdEnvioNNFiltro envioNNFiltro);

        /// <summary>
        /// Adiciona archivo de un radicado
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarArchivosRapiradicado(ADRapiRadicado radicado);

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioServicioDC CalcularPrecioRapiPromocionalTipoEntrega(int idListaPrecios, decimal cantidad, string idTipoEntrega);

        /// <summary>
        /// Obtiene el precio del servicio carga express
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioCargaExpressTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizadoTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiHoyTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPagoTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioCargaDC CalcularPrecioRapiCargaTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiAmTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioNotificacionesTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioMensajeriaTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision);

        /// <summary>
        /// Obtiene el precio del servicio rapi radicado
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioRapiradicadoTipoEntrega(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true);

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerNotificacion(long numeroGuia);

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CES
        /// </summary>
        /// <param name="idAdmision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADNotificacion> ObtenerNotificacionesEntregaCES(long idCentroServicio, long idCentroServicioOrigen, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CRE
        /// </summary>
        /// <param name="idAdmision"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADNotificacion> ObtenerNotificacionesEntregaCRE(DateTime fechaInicial, DateTime fechaFinal, long idCol, long idSucursal);

        /// <summary>
        /// Método para obtener las guías internas de una planilla de notificación
        /// </summary>
        /// <param name="idplanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuiaInternaDC> ObtenerGuiasInternasNotificaciones(long idplanilla);

        /// <summary>
        /// Realizar un cambio de estado de guías masivo
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]

        Dictionary<long, string> GrabarCambioEstadoGuias(List<ADGuiaUltEstadoDC> guias);

        /// <summary>
        /// Obtiene la ubicacion de una guia para la app del cliente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADUbicacionGuia ObtenerUbicacionGuia(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarGuiaImpresa(long NumeroGuia);


        /// <summary>
    /// verifica si una guia existe
    /// </summary>
    /// <param name="numeroGuia"></param>
    /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificarSiGuiaExiste(long numeroGuia);

         /// <summary>
    /// Obtiene toda la informacion´de admisión a partir de una cadena separada por comas
    /// </summary>
    /// <param name="numeroGuia"></param>
    /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADTrazaGuia> ObtenerListaGuiasSeparadaComas(string listaNumerosGuias);

            /// <summary>
    /// Obtiene los estados  de una guia en una localidad
    /// </summary>
    /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia);

          /// <summary>
         /// Obtiene los Estados y Motivos de la Guia seleccionada
         /// </summary>
         /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCentroServicioDestino"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino);

        /// <summary>
        /// Retorna las guias por agencia a gestionar
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]        
        List<ADTrazaGuiaAgencia> ObtenerGuiasGestionAgencias(int idEstadoGuia, long IdCol);
        

        /// <summary>
        /// Obtener estado guia para gestion telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadoYMotivoGuiaDC ObtenerEstadoYMotivoGuia(long numeroGuia);

        /// <summary>
        /// Registra la auditoria de las guias de POS que al sincronizarlas ya se encuentran admitidas
        /// </summary>
        /// <param name="guiaSerializada"></param>
        /// <param name="objetoAdicionalSerializado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="idCentroServiciosOrigen"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RegistrarAuditoriaAdmisionesManualesDuplicadas(string guiaSerializada, string objetoAdicionalSerializado, long numeroGuia, long idCentroServiciosOrigen);

        /// <summary>
        /// obtiene informacion de una guia seleccionada ya sea que haya sido modificada o no
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerInformacionGuiaPorNumero(long numeroGuia);
        /// <summary>
        /// Obtiene una lista con las guias encontradas sea por numero de guia o por fecha
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiaPorNumeroOFecha(long? numeroGuia, DateTime fechaInicio, DateTime fechaFin, short index, short size);
/*
        /// <summary>
        /// Registrar multiples guias con una admision
        /// </summary>
        /// <param name="listaGuia"></param>
        /// <param name="idCaja"></param>
        /// <param name="mediosPago"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADRetornoAdmision> RegistrarGuiasMedioPago(
                            List<ADRegistroAdmisiones> listaGuia,
                            int idCaja,
                            List<ADRegistroMediosPagoDC> mediosPago
            );
        */
        /// <summary>
        /// Obtiene el Tipo de Formato de Impresión de una Localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>


        #region reimpresionesWPF

        /// <summary>
        /// Obtiene el Tipo de Formato de Impresión de una Localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ObtenerTipoFormatoImpresionLocalidad(long IdLocalidad);

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el destinatario indicado
        /// </summary>
        /// <param name="tipoDidentificacionDestinatario"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasPorDestinatario(string tipoDidentificacionDestinatario, string idDestinatario);

        #endregion  

        #region Nuevo Cotizador


        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<TAPreciosAgrupadosDC> ResultadoListaCotizar(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega);

        #endregion 
        
    }
}
