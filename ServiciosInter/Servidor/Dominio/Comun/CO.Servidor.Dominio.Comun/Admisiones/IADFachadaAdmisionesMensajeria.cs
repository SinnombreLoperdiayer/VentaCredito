using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;

namespace CO.Servidor.Dominio.Comun.Admisiones
{
    public interface IADFachadaAdmisionesMensajeria
    {
        /// <summary>
        /// Genera solicitud de falla
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="valorCobrado"></param>
        /// <param name="valorCalculado"></param>
        void GenerarFallaCalculoValorGuiaManual(long numeroGuia, decimal valorCobrado, decimal valorCalculado);

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal);

        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        ADRetornoAdmision ObtenerGuiaPorGuid(string guid);

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para dicho trayecto
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, decimal pesoGuia);

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para
        /// dicho trayecto, la duración en días y la prima de seguro acordaba con el cliente en el contrato
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="esDesdeSucursal">Indica si la transacción se va a realizar desde la propia sucursal del cliente o si se hace desde una agencia de Interrapidísimo</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idListaPrecios">Identificador de la lista de precios asociada al contrato</param>
        /// <returns></returns>
        ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idSucursal, int idCliente, int idListaPrecios, decimal pesoGuia);

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        IEnumerable<ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad();

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        IEnumerable<ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion();

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        IEnumerable<ADTipoEntrega> ObtenerTiposEntrega();

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        ADParametrosAdmisiones ObtenerParametrosAdmisiones();

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion();


        #region Cálculo de precios en tarifas

        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios);

        /// <summary>
        /// Calcular el precio para una tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado);

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
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
        TAPrecioMensajeriaDC CalcularPrecioRapiradicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

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
        TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad);

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        //ADGuia ConsultarGuia(int idCliente, long numeroGuia);

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>         

        TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        #endregion Cálculo de precios en tarifas

        #region Grabación de Guía

        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        long AdicionarAdmisionAnulada(ADGuia guia);

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        ADResultadoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque);

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        ADResultadoAdmision RegistrarGuiaManualInternacionalCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Registra guía automática internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        ADResultadoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque);

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        ADGuiaInternaDC AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario);

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        ADResultadoAdmision RegistrarGuiaAutomaticaNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion);

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        ADResultadoAdmision RegistrarGuiaAutomaticaRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago);

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        ADResultadoAdmision RegistrarGuiaAutomaticaRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        ADResultadoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario);

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        ADResultadoAdmision RegistrarGuiaManualCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        ADResultadoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion);

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        ADResultadoAdmision RegistrarGuiaManualNotificacionCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago);

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPagoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        ADResultadoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado);

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        ADResultadoAdmision RegistrarGuiaManualRapiRadicadoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, long idAgenciaRegistraAdmision);

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        void ActualizarPagadoGuia(long idAdmisionMensajeria, bool estaPagada=true);

        /// <summary>
        /// Actualiza en supervision una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        void ActualizarSupervisionGuia(long idAdmisionMensajeria);

        #endregion Grabación de Guía

        #region Consultas

        /// <summary>
        /// Valida si un al cobro especifico está asignado a un coordinador de col x vencimiento en el pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        bool AlCobroCargadoACoordinadorCol(long idAdmision);

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioConvenio(long idAdmision);

        /// <summary>
        /// Obtiene las formas de pago de una guia
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        List<ADGuiaFormaPago> ObtenerFormasPagoGuia(long idGuia);

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia);

        /// <summary>
        /// Retorna la información de una guía dada su forma de pago, en un rango de fechas de admisión, que pertenezcan al cliente dado y al RACOL dado, que
        /// sean del servicio de notificaciones, tipo de envío certificación, que estén descargadas como entrega correcta, que no tengan capturado los datos de
        /// recibido y estén digitalizadas
        /// </summary>
        /// <param name="idFormaPago">Forma de pago</param>
        /// <param name="fechaInicio">Fecha Inicial</param>
        /// <param name="fechaFin">Fecha Final</param>
        /// <param name="idCliente">Id del Cliente</param>
        /// <param name="idRacol">Id del Racol</param>
        /// <returns></returns>
        List<ADGuia> ObtenerGuiasParaCapturaAutomatica(short idFormaPago, DateTime fechaInicio, DateTime fechaFin, int? idCliente, long idRacol);

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// La guía debe estar en estado "Devolución" o "Entrega" y la prueba de entrega o de devolución
        /// correspondiente debe estar digitalizada en la aplicación
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADGuia ObtenerGuiaParaRecibirManualNotificaciones(long numeroGuia);

        /// <summary>
        /// Retorna las guias a gestionar en telemercadeo
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCentroServicioDestino"></param>
        /// <returns></returns>
        List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino);

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        SUPropietarioGuia ObtenerPropietarioGuia(long numeroGuia, long idSucursalCentroServicio);

        /// <summary>
        /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
        /// </summary>
        /// <param name="idNumeroGuia"></param>
        ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia);

        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision);

        /// <summary>
        /// Consultar el contrato de n cliente Convenio
        /// </summary>
        /// <param name="TipoCliente"></param>
        /// <param name="idAdmisionMensajeria"></param>
        /// <returns></returns>
        int ObtenerContratoClienteConvenio(ADEnumTipoCliente tipoCliente, long idAdmisionMensajeria);

        /// <summary>
        /// Metodo que obtiene la información de una admisión de mensajeria a partir del numero de la misma
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADGuia ObtenerGuia(long idAdmision);

        /// <summary>
        /// Método para calcular y guardar comisiones
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="tipoComision"></param>
        /// <param name="?"></param>
        void AdicionarComision(ADGuia guia, CMEnumTipoComision tipoComision);

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia);

        /// <summary>
        /// Obtiene el identificador de la admisión
        /// </summary>
        /// <param name="numeroGuia">Número de guía</param>
        /// <returns>Identificador admisión</returns>
        long ObtenerIdentificadorAdmision(long numeroGuia);

        /// <summary>
        /// Retorna la información de una guía completa incluyendo la forma como se pagó, se construyó para generar impresión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long? idCentroServicios, int? idSucursal, int? idCliente);

        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision);

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long? idCentroServicios, int? idSucursal, int? idCliente);

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <returns></returns>
        List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long? idCentroServicios, int? idSucursal, int? idCliente);

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long IdCentroServicio);

        /// <summary>
        /// Actualiza la guía y registra el valor en Caja de la transaccion.
        /// </summary>
        /// <param name="guiaAlCobro">The guia al cobro.</param>
        ADRecaudoAlCobro ActualizarGuiaAlCobro(ADRecaudarDineroAlCobroDC guiaAlCobro);

        /// <summary>
        /// Método para obtener una guía de temelercado con sus respectivos valores adicionales
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADGuia ObtenerGuiaTelemercadeo(long numeroGuia);

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// si la guia no existe retorna el idAdmision = 0
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADGuia ObtenerInfoGuiaXNumeroGuia(long numeroGuia);

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADGuia ObtenerGuiaXNumeroGuiaCredito(long numeroGuia);
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        ADGuia ConsultarGuia(int idCliente, long numeroGuia);

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumeroGuias);

        /// <summary>
        /// Método para obtener una guía interna
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        ADGuiaInternaDC ObtenerGuiaInterna(long numeroGuia);

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia);

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias);

        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro);

        /// <summary>
        /// Obtiene la admision de mensajeria de rapi envio contra pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADRapiEnvioContraPagoDC ObtenerRapiEnvioContraPago(long idAdmision);

        /// <summary>
        /// Obtener las notificaciones de una guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADNotificacion ObtenerNotificacionGuia(long idAdmision);

        /// <summary>
        /// Método para obtener las guías de servicio rapiradicado
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro);

        /// <summary>
        /// Obtiene todas las guias en estado en centro de acopio en una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        List<ADGuiaUltEstadoDC> ObtenerGuiasEnCentroAcopioLocalidad(string idLocalidad);

        /// <summary>
        /// Método para obtener una guía interna a partir de un numero de guia, si no existe la guia genere excepción
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        ADGuiaInternaDC ObtenerGuiaInternaNumeroGuia(long numeroGuia);

        /// <summary>
        /// Consulta la informacion remitente detinatario por numero guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADMensajeriaTipoCliente ObtenerRemitenteDestinatarioGuia(long numeroGuia);


        /// <summary>
        /// verifica si una guia existe
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        bool VerificarSiGuiaExiste(long numeroGuia);

        #endregion Consultas

        #region Novedades

        /// <summary>
        /// Adicionar novedades de una guia
        /// </summary>
        /// <param name="novedad"></param>
        void AdicionarNovedad(ADNovedadGuiaDC novedad, Dictionary<CCEnumNovedadRealizada, string> datosAdicionalesNovedad);

        /// <summary>
        /// Actualizar destino de una guia
        /// </summary>
        /// <param name="centroServicioDestino"></param>
        void ActualizarDestinoGuia(long idAdmisionMensajeria, PUCentroServiciosDC centroServicioDestino, CCReliquidacionDC valorReliquidado, TAEnumFormaPago? formaPago, string idTipoEntrega, string descripcionTipoEntrega);

        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        void ActualizarRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia);

        /// <summary>
        /// Actualizar formas de pago de una guia
        /// </summary>
        void ActualizarFormaPagoGuia(CCNovedadCambioFormaPagoDC novedadGuia);

        /// <summary>
        /// Actualizar es alcobro
        /// </summary>
        void ActualizarEsAlCobro(long idAdmisionGuia, bool esAlCobro);

        /// <summary>
        /// Actualizar numero de reintentos de entrega de una admision
        /// </summary>
        /// <param name="idAdmision"></param>
        void ActualizarReintentosEntrega(long idAdmision);

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio - peaton
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioPeaton(long idAdmision);

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente peaton - peaton
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonPeaton(long idAdmision);

        /// <summary>
        /// Actualiza el tipo de servicio de una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        void ActualizarTipoServicioGuia(long idAdmisionGuia, int idServicio);

        #endregion Novedades

        #region Eliminación

        /// <summary>
        /// Metodo para eliminar una admisión con auditoria
        /// </summary>
        /// <param name="idAdmision"></param>
        void EliminarAdmision(long idAdmision);

        /// <summary>
        /// Eliminar una guía interna
        /// </summary>
        /// <param name="manifiesto"></param>
        void EliminarGuiaInterna(ADGuiaInternaDC guiaInterna);

        #endregion Eliminación

        #region Adicionar volantes

        /// <summary>
        /// Adiciona archivo de un volante
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        void AdicionarArchivo(LIArchivosDC archivo);

        /// <summary>
        /// Metodo para guardar un volante de devolución
        /// </summary>
        /// <param name="volante"></param>
        /// <returns></returns>
        long AdicionarEvidenciaDevolucion(LIEvidenciaDevolucionDC evidenciaDevolucion);

        /// <summary>
        /// Edita un archivo de evidencia de devolución
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        void EditarArchivoEvidencia(LIArchivosDC imagen);

        #endregion Adicionar volantes

        #region Actualizaciones

        /// <summary>
        /// Método para actualizar el número de guía interna
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        void ActualizarGuiaRapiradicado(long idRadicado, long numeroGuiaInterna);

        /// <summary>
        /// Método para actualizar una notificación
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        void ActualizarNotificacion(long idAdmision, long numeroGuia);


        
        /// <summary>
        /// Método para actualizar una notificacion cuando es sacada de una planilla
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        void ActualizarNotificacionPlanilla(long idAdmision);

        /// <summary>
        /// Actualizar el valor total de una guía dada
        /// </summary>
        /// <param name="idAdmisionGuia"></param>
        /// <param name="ValorTotal"></param>
        void ActualizarValorTotalGuia(CCNovedadCambioValorTotal novedadGuia);

        /// <summary>
        /// Actualizar valores de la guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">nmero de la guia</param>
        /// <param name="valores">valores a modificar</param>
        /// <param name="valorAdicionales">valores adicionales que se agregan al total</param>
        void ActualizarValoresGuia(long idAdmisionGuia, CCReliquidacionDC valores, decimal valorAdicionales);

        /// <summary>
        /// Actualiza el valor del porcentaje de
        /// recargo
        /// </summary>
        /// <param name="valor">el valor a actualizar</param>
        void ActualizarParametroPorcentajeRecargo(double porcentaje);

        /// <summary>
        /// Actualiza el Valor del Peso de la Guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">numeor de la Guía</param>
        /// <param name="valorPeso">Valor del peso a actualizar</param>
        void ActualizarValorPesoGuia(long idAdmisionGuia, decimal valorPeso);


                /// <summary>
        /// Método para actualizar el campo entregado en la tabla de admisión mensajeria
        /// </summary>
        /// <param name="numeroGuia"></param>
        void ActualizarEntregadoGuia(long numeroGuia);

        #endregion Actualizaciones

        #region reexpedicion

        /// <summary>
        /// Guarda la relacion de las guias
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        void GuadarRelacionReexpedicionEnvio(long idAdmisionOriginal, long idAdmisionNueva);

        /// <summary>
        /// Retorna lista de valores adicionales agregados a una admisión
        /// </summary>
        /// <param name="IdAdmision"></param>
        /// <returns></returns>
        List<TAValorAdicional> ObtenerValoresAdicionales(long IdAdmision);

        /// <summary>
        /// Obtiene la admision de mensajeria peaton-convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonConvenio(long idAdmision);

        #endregion reexpedicion

        #region Orden de Servicio Cargue Masivo

        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio);

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex);

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio);

        /// <summary>
        /// Obtiene el listado de
        /// las ordenes de servicio por fecha
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>lista de ordenes por fecha</returns>
        List<ADOrdenServicioMasivoDC> ObtenerOrdenesServicioPorFecha(DateTime fechaInicial, DateTime fechaFinal);

        #endregion Orden de Servicio Cargue Masivo

        #region Notificaciones

        /// <summary>
        /// Obtiene la admision de mensajeria para el servicio de notificaciones
        /// </summary>
        /// <param name="numeroGuia"></param>
        ADNotificacion ObtenerAdmMenNotEntregaDevolucion(long numeroGuia);


                /// <summary>
        /// Método para validar una guia notificacion en devolucion
        /// </summary>
        /// <param name="idAdmision"></param>
        ADNotificacion ValidarNotificacionDevolucion(long idAdmision);

        /// <summary>
        /// Método para actualizar la tabla notificacion campo ADN_EstaDevuelta , campo ADN_NumeroGuiaInterna
        /// </summary>
        /// <param name="guia"></param>
        void ActualizarPLanilladaNotificacion(ADNotificacion guia);


        /// <summary>
        /// Inserta un registro de una notificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        void AdicionarNotificacion(long numeroGuia);
  
        #endregion Notificaciones


        #region Confirmar Direccion

        /// <summary>
        /// Método para verificar una direccion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="verificadoPor"></param>
        /// <param name="destinatario"></param>
        void ConfirmarDireccion(long numeroGuia, string verificadoPor, bool destinatario, bool remitente);


        #endregion

        TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1");

        #region SISPOSTAL

        bool ValidarCredencialSispostal(credencialDTO credencial);

        /// <summary>
        /// Obtiene la traza de una guia dependiendo del id unico de su estado en admisionmensajeria
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ADEstadoGuia ObtenerEstadoGuiaTrazaPorIdEstado(ADEnumEstadoGuia idEstadoGuia, long numeroGuia);

        #endregion

    }
}