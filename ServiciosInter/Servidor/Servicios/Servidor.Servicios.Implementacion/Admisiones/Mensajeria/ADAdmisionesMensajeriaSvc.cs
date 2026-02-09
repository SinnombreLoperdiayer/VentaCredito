using CO.Servidor.Adminisiones.Mensajeria;
using CO.Servidor.Adminisiones.Mensajeria.Movil;
using CO.Servidor.LogisticaInversa.Telemercadeo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;

namespace CO.Servidor.Servicios.Implementacion.Admisiones.Mensajeria
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ADAdmisionesMensajeriaSvc : IADAdmisionesMensajeriaSvc
    {
        // Forzar cultura en el servidor
        public ADAdmisionesMensajeriaSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #region Consultas

        /// <summary>
        /// Genera solicitud de falla
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="valorCobrado"></param>
        /// <param name="valorCalculado"></param>
        public void GenerarFallaCalculoValorGuiaManual(long numeroGuia, decimal valorCobrado, decimal valorCalculado)
        {
            ADFachadaDominio.Instancia.GenerarFallaCalculoValorGuiaManual(numeroGuia, valorCobrado, valorCalculado);
        }

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad()
        {
            return ADFachadaDominio.Instancia.ObtenerMotivosNoUsoBolsaSeguridad();
        }

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            return ADFachadaDominio.Instancia.ObtenerCondicionesPorOperadorPostal(idOperadorPostal);
        }

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion()
        {
            return ADFachadaDominio.Instancia.ObtenerObjetosProhibidaCirculacion();
        }

        /// <summary>
        /// Valida si el servicio está habilidado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para dicho trayecto
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, decimal pesoGuia, DateTime? fechadmisionEnvio)
        {
            return ADFachadaDominio.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, centroServiciosOrigen, pesoGuia, fechadmisionEnvio);
        }

        // TODO ID: Se adiciona este metodo para traer los datos del centro de Servicio Destino(AGE), cuando se activa en Admisiones el Tipo de entrega "RECLAME EN OFICINA"
        /// <summary>
        /// Valida la Agencia-Centro de Servicio Destino para tipo de Entrega "RECLAME EN OFICINA"
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarCentroServicioDestino(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, long centroServiciosOrigen)
        {
            return ADFachadaDominio.Instancia.ValidarCentroServicioDestino(municipioOrigen, municipioDestino, centroServiciosOrigen);
        }

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
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idSucursal, int idCliente, int idListaPrecio, decimal pesoGuia, DateTime? fechadmisionEnvio)
        {
            return ADFachadaDominio.Instancia.ValidarServicioTrayectoDestinoCliente(municipioOrigen, municipioDestino, servicio, idSucursal, idCliente, idListaPrecio, pesoGuia, fechadmisionEnvio);
        }

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return ADFachadaDominio.Instancia.ObtenerTiposEntrega();
        }

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            return ADFachadaDominio.Instancia.ObtenerParametrosAdmisiones();
        }
        /// <summary>
        /// obtiene los parametros de encabezado de guia
        /// </summary>
        /// <param name="llave"></param>
        /// <returns></returns>
        public string ObtenerParametrosEncabezado(string llave)
        {
            return ADFachadaDominio.Instancia.ObtenerParametrosEncabezado(llave);
        }

        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision ObtenerGuiaPorGuid(string guid)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiaPorGuid(guid);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroGuia, long idSucursalCentroServicio)
        {
            return ADFachadaDominio.Instancia.ObtenerPropietarioGuia(numeroGuia, idSucursalCentroServicio);
        }

        /// <summary>
        /// Retorna la información de una guía completa incluyendo la forma como se pagó, se construyó para generar impresión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiaPorNumeroDeGuiaCompleta(numeroGuia, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
        }


        /// <summary>
        /// consulta informacion de una guia Sispostal a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaSispostalXNumeroGuia(long numeroGuia)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiaSispostalXNumeroGuia(numeroGuia);
        }





        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiasPorRemitenteParaHoy(idRemitente, tipoIdRemitente, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiasPorDestinatarioParaHoy(idDestinatario, tipoIdDestinatario, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        public List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long idCentroServicio)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiasAlCobroNoPagas(indicePagina, registrosPorPagina, numeroGuia, fechaInicial, fechaFinal, idCentroServicio);
        }

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarPagadoGuia(long idAdmisionMensajeria)
        {
            ADFachadaDominio.Instancia.ActualizarPagadoGuia(idAdmisionMensajeria);
        }

        /// <summary>
        /// Actualiza la guía y registra el valor en Caja de la transaccion.
        /// </summary>
        /// <param name="guiaAlCobro">The guia al cobro.</param>
        public ADRecaudoAlCobro ActualizarGuiaAlCobro(ADRecaudarDineroAlCobroDC guiaAlCobro)
        {
            return ADFachadaDominio.Instancia.ActualizarGuiaAlCobro(guiaAlCobro);
        }

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumeroGuias)
        {
            return ADFachadaDominio.Instancia.ObtenerGuiasInternas(numeroInicial, numeroFinal, listaNumeroGuias);
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias)
        {
            return ADFachadaDominio.Instancia.ObtenerListaGuias(listaNumerosGuias);
        }


        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una cadena separada por comas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerListaGuiasSeparadaComas(string listaNumerosGuias)
        {
            return ADFachadaDominio.Instancia.ObtenerListaGuiasSeparadaComas(listaNumerosGuias);
        }


        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision)
        {

            return ADFachadaDominio.Instancia.ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(numeroGuia, idAdmision);
        }

        /// <summary>
        /// Método para obtener la información de una guía rapiradicado
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADRapiRadicado ObtenerAdmisionRapiradicado(long numeroGuia)
        {
            return ADFachadaDominio.Instancia.ObtenerAdmisionRapiradicado(numeroGuia);
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerNotificacion(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerNotificacion(numeroGuia);
        }

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CES
        /// </summary>
        /// <param name="idAdmision"></param>
        public List<ADNotificacion> ObtenerNotificacionesEntregaCES(long idCentroServicio, long idCentroServicioOrigen, DateTime fechaInicial, DateTime fechaFinal)
        {
            return ADConsultas.Instancia.ObtenerNotificacionesEntregaCES(idCentroServicio, idCentroServicioOrigen, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CRE
        /// </summary>
        /// <param name="idAdmision"></param>
        public List<ADNotificacion> ObtenerNotificacionesEntregaCRE(DateTime fechaInicial, DateTime fechaFinal, long idCol, long idSucursal)
        {
            return ADConsultas.Instancia.ObtenerNotificacionesEntregaCRE(fechaInicial, fechaFinal, idCol, idSucursal);
        }

        /// <summary>
        /// Método para obtener las guías internas de una planilla de notificación
        /// </summary>
        /// <param name="idplanilla"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasNotificaciones(long idplanilla)
        {
            return ADConsultas.Instancia.ObtenerGuiasInternasNotificaciones(idplanilla);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ObtenerImagenPublicitariaGuia()
        {
            return ADConsultas.Instancia.ObtenerImagenPublicitariaGuia();
        }

        public List<ADGuia> ObtenerAdmisionMensajeriaSinEntregar(AdEnvioNNFiltro envioNNFiltro)
        {
            return ADConsultas.Instancia.ObtenerAdmisionMensajeriaSinEntregar(envioNNFiltro);
        }


        #endregion Consultas

        #region Cálculo de precios en tarifas

        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        public TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioCentroCorrespondencia(idListaPrecios);
        }

        /// <summary>
        /// Calcular el precio para una tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        public TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioInternacional(idListaPrecios, tipoEmpaque, idLocalidadDestino, peso, idZona, valorDeclarado);
        }

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioTramiteDC CalcularPrecioTramites(int idListaPrecios, int idTramite)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioTramites(idListaPrecios, idTramite);
        }

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
        public TAPrecioMensajeriaDC CalcularPrecioRapiradicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiradicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

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
        public TAPrecioMensajeriaDC CalcularPrecioRapiradicadoTipoEntrega(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiradicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiTulas(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresMsj(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiValoresMsj(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresCarga(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiValoresCarga(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiCargaConsolidado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiCargaConsolidado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValijas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiValijas(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeriaTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

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
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioCargaExpress(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioCargaAerea(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        public List<TAPreciosAgrupadosDC> CalcularPrecioServicios(List<int> servicios, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioServicios(servicios, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);
        }

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
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpressTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioCargaExpress(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiPromocional(idListaPrecios, cantidad);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioServicioDC CalcularPrecioRapiPromocionalTipoEntrega(int idListaPrecios, decimal cantidad, string idTipoEntrega)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiPromocional(idListaPrecios, cantidad, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiPersonalizado(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizadoTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiPersonalizado(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiHoy(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiHoyTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiHoy(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiEnvioContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPagoTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiEnvioContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiCargaContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCargaContraPagoTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiCargaContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiCarga(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCargaTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiCarga(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiAm(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiAmTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioRapiAm(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioNotificaciones(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
        }

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioNotificacionesTipoEntrega(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, bool esPrimeraAdmision = true)
        {
            return ADFachadaDominio.Instancia.CalcularPrecioNotificaciones(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        #endregion Cálculo de precios en tarifas

        #region Grabación admisión

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaAutomatica(guia, idCaja, remitenteDestinatario);
            ADRetornoAdmision retorno = new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito,
                DireccionAgenciaCiudadDestino = resultado.DireccionAgenciaCiudadDestino,
                DireccionAgenciaCiudadOrigen = resultado.DireccionAgenciaCiudadOrigen
            };


            ADFachadaDominio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, "RegistrarGuiaAutomatica", retorno, guia, remitenteDestinatario);

            return retorno;
        }

        /// <summary>
        /// Se registra un movimiento de mensajería internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaAutomaticaInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            ADRetornoAdmision retorno = new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };

            ADFachadaDominio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, "RegistrarGuiaAutomaticaInternacional", retorno, guia, remitenteDestinatario, tipoEmpaque);

            return retorno;



        }

        // todo:id Servicio para Guardar Guia-DHL
        /// <summary>
        /// Se registra un movimiento de mensajería internacional con DHL
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaAutomaticaInternacional_DHL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, ADGuiaInternacionalDC guiaInternacional)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaAutomaticaInternacional_DHL(guia, idCaja, remitenteDestinatario, tipoEmpaque, guiaInternacional);
            ADRetornoAdmision retorno = new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };

            ADFachadaDominio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, "RegistrarGuiaAutomaticaInternacional_DHL", retorno, guia, remitenteDestinatario, tipoEmpaque);
            return retorno;
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualCOL(guia, idCaja, remitenteDestinatario, idAgenciaRegistraAdmision);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualInternacionalCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, long idAgenciaRegistraAdmision)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualInternacionalCOL(guia, idCaja, remitenteDestinatario, tipoEmpaque, idAgenciaRegistraAdmision);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaAutomaticaNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaAutomaticaNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            ADRetornoAdmision retorno = new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito,
                DireccionAgenciaCiudadDestino = resultado.DireccionAgenciaCiudadDestino,
                DireccionAgenciaCiudadOrigen = resultado.DireccionAgenciaCiudadOrigen
            };

            ADFachadaDominio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, "RegistrarGuiaAutomaticaNotificacion", retorno, guia, remitenteDestinatario, notificacion);

            return retorno;
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualNotificacionCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, long idAgenciaRegistraAdmision)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualNotificacionCOL(guia, idCaja, remitenteDestinatario, notificacion, idAgenciaRegistraAdmision);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaAutomaticaRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaAutomaticaRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            ADRetornoAdmision retorno = new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };

            ADFachadaDominio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, "RegistrarGuiaAutomaticaRapiEnvioContraPago", retorno, guia, remitenteDestinatario, rapiEnvioContraPago);

            return retorno;
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualRapiEnvioContraPagoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, long idAgenciaRegistraAdmision)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualRapiEnvioContraPagoCOL(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago, idAgenciaRegistraAdmision);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaAutomaticaRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaAutomaticaRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            ADRetornoAdmision retorno = new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito,
                DireccionAgenciaCiudadDestino = resultado.DireccionAgenciaCiudadDestino,
                DireccionAgenciaCiudadOrigen = resultado.DireccionAgenciaCiudadOrigen
            };

            ADFachadaDominio.Instancia.GuardarAuditoriaGrabacionAdmisionMensajeria(idCaja, "RegistrarGuiaAutomaticaRapiRadicado", retorno, guia, remitenteDestinatario, rapiRadicado);

            return retorno;
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision RegistrarGuiaManualRapiRadicadoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, long idAgenciaRegistraAdmision)
        {
            ADResultadoAdmision resultado = ADFachadaDominio.Instancia.RegistrarGuiaManualRapiRadicadoCOL(guia, idCaja, remitenteDestinatario, rapiRadicado, idAgenciaRegistraAdmision);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia,
                AdvertenciaPorcentajeCupoSuperadoClienteCredito = resultado.AdvertenciaPorcentajeCupoSuperadoClienteCredito
            };
        }

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        public ADGuiaInternaDC AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            return ADFachadaDominio.Instancia.AdicionarGuiaInterna(guiaInterna);
        }

        #endregion Grabación admisión

        #region Orden de Servicio Cargue Masivo

        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        public long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio)
        {
            return ADFachadaDominio.Instancia.CrearOrdenServicioMasivo(datosOrdenServicio);
        }

        /// <summary>
        /// Validar la existencia de una orden de servicio masiva
        /// </summary>
        /// <param name="ordenServicio">número de la orden de servicio a validar</param>
        /// <returns></returns>
        public bool ValidarOrdenServicio(long ordenServicio)
        {
            return ADFachadaDominio.Instancia.ValidarOrdenServicio(ordenServicio);
        }

        /// <summary>
        /// Valida cada uno de los envíos que se desea asociar a una orden de servicio antes de almacenarla en la base de datos
        /// </summary>
        /// <param name="enviosParaValidar">Listado de los envíos que se desean validar</param>
        /// <returns>Listado de los envíos validados y su respectivo resultado</returns>
        public List<ADDatosValidadosDC> ValidarDatosGuiasOrdenMasiva(List<ADDatosValidacionDC> enviosParaValidar, bool esAutomatica)
        {
            return ADFachadaDominio.Instancia.ValidarDatosGuiasOrdenMasiva(enviosParaValidar, esAutomatica);
        }

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        public List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex)
        {
            return ADFachadaDominio.Instancia.ConsultarGuiasDeOrdenDeServicio(idOrdenServicio, pageSize, pageIndex);
        }

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        public long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio)
        {
            return ADFachadaDominio.Instancia.ConsultarCantidadGuiasOrdenSerServicio(idOrdenServicio);
        }

        /// <summary>
        /// Se registra un movimiento de mensajería asociado a una orden de servicio. Esto se utiliza en
        /// cuando se hace cargue masivo de guias o facturas.
        /// </summary>
        /// <param name="guia">Datos de la guia</param>
        /// <param name="idCaja">Caja que hace la operacion</param>
        /// <param name="remitenteDestinatario">Datos del remitente y el destinatario</param>
        /// <returns>Número de guía</returns>
        public List<ADRetornoAdmision> RegistrarGuiasDeOrdenDeServicio(List<ADGuiaOSDC> guias, bool conIngresoCentroAcopio)
        {
            return ADFachadaDominio.Instancia.RegistrarGuiasDeOrdenDeServicio(guias, conIngresoCentroAcopio);
        }

        /// <summary>
        /// Obtiene el listado de
        /// las ordenes de servicio por fecha
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>lista de ordenes por fecha</returns>
        public List<ADOrdenServicioMasivoDC> ObtenerOrdenesServicioPorFecha(DateTime fechaInicial, DateTime fechaFinal)
        {
            return ADFachadaDominio.Instancia.ObtenerOrdenesServicioPorFecha(fechaInicial, fechaFinal);
        }

        #endregion Orden de Servicio Cargue Masivo

        #region Rapiradicados

        /// <summary>
        /// Adiciona archivo de un radicado
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivosRapiradicado(ADRapiRadicado radicado)
        {
            ADFachadaAdmisionesMensajeria.Instancia.AdicionarArchivosRapiradicado(radicado);
        }

        #endregion Rapiradicados

        /// <summary>
        /// Realizar un cambio de estado de guías masivo
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        public Dictionary<long, string> GrabarCambioEstadoGuias(List<ADGuiaUltEstadoDC> guias)
        {
            return ADAdmisionMensajeria.Instancia.GrabarCambioEstadoGuias(guias);
        }

        /// <summary>
        /// Obtiene la ubicacion de una guia para la app del cliente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADUbicacionGuia ObtenerUbicacionGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerUbicacionGuia(numeroGuia);
        }

        public void ActualizarGuiaImpresa(long NumeroGuia)
        {
            ADConsultas.Instancia.ActualizarGuiaImpresa(NumeroGuia);
        }



        /// <summary>
        /// verifica si una guia existe
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool VerificarSiGuiaExiste(long numeroGuia)
        {
            return ADAdmisionMensajeria.Instancia.VerificarSiGuiaExiste(numeroGuia);
        }

        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
        {
            return ADConsultas.ObtenerEstadosGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        public List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
        {
            return ADConsultas.ObtenerEstadosMotivosGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene las guias a gestionar en telemercadeo
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCentroServicioDestino"></param>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino)
        {
            return ADConsultas.Instancia.ObtenerGuiasGestion(idEstadoGuia, IdCentroServicioDestino);
        }

        /// <summary>
        /// Obtiene las guias por agencia a gestionar en telemercadeo
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        public List<ADTrazaGuiaAgencia> ObtenerGuiasGestionAgencias(int idEstadoGuia, long IdCol)
        {
            return ADConsultas.Instancia.ObtenerGuiasGestionAgencias(idEstadoGuia, IdCol);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIEstadoYMotivoGuiaDC ObtenerEstadoYMotivoGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerEstadoYMotivoGuia(numeroGuia);
        }

        /// <summary>
        /// Registra la auditoria de las guias de POS que al sincronizarlas ya se encuentran admitidas
        /// </summary>
        /// <param name="guiaSerializada"></param>
        /// <param name="objetoAdicionalSerializado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="idCentroServiciosOrigen"></param>
        public void RegistrarAuditoriaAdmisionesManualesDuplicadas(string guiaSerializada, string objetoAdicionalSerializado, long numeroGuia, long idCentroServiciosOrigen)
        {
            ADAdmisionMensajeria.Instancia.RegistrarAuditoriaAdmisionesManualesDuplicadas(guiaSerializada, objetoAdicionalSerializado, numeroGuia, idCentroServiciosOrigen);
        }
        /// <summary>
        /// obtiene informacion de una guia seleccionada ya sea que haya sido modificada o no
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerInformacionGuiaPorNumero(long numeroGuia)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerInformacionGuiaPorNumero(numeroGuia);
        }
        /// <summary>
        /// Obtiene una lista con las guias encontradas sea por numero de guia o por fecha
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiaPorNumeroOFecha(long? numeroGuia, DateTime fechaInicio, DateTime fechaFin, short index, short size)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerGuiaPorNumeroOFecha(numeroGuia, fechaInicio, fechaFin, index, size);
        }


        #region reimpresionesWPF

        /// <summary>
        /// Método para obtener el Tipo de Impresion de una Localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public long ObtenerTipoFormatoImpresionLocalidad(long IdLocalidad)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerTipoFormatoImpresionLocalidad(IdLocalidad);
        }



        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el destinatario indicado
        /// </summary>
        /// <param name="tipoDidentificacionDestinatario"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatario(string tipoDidentificacionDestinatario, string idDestinatario)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerGuiasPorDestinatario(tipoDidentificacionDestinatario, idDestinatario);
        }

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
        public List<TAPreciosAgrupadosDC> ResultadoListaCotizar(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            // return ADFachadaDominio.Instancia.CalcularPrecioServiciosMobile(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);           
            return ADFachadaDominio.Instancia.CalculaServicioCotizador(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);
        }

        public List<ADGuia> ObtenerReporteCajaGuiasMensajeroApp(long idMensajeria)
        {
            return ADFachadaDominio.Instancia.ObtenerReporteCajaGuiasMensajeroApp(idMensajeria);
        }

        #endregion 

        #region
        /// <summary>
        /// Método nuevo para calcular servicio a Cotizar
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        public List<TAPreciosAgrupadosDC> CalculaServicioCotizador(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            return ADFachadaDominio.Instancia.CalculaServicioCotizador(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);
        }
        #endregion


        #region AppMovil admision

        /// <summary>
        /// Método para registrar una guia manual desde app
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        /// <param name="radicado"></param>
        /// <returns></returns>
        public ADRetornoAdmision RegistrarGuiaManualMovil(ADGuia guia, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion = null, ADRapiRadicado radicado = null)
        {
            ADResultadoAdmision resultado = ADAdmisionMovil.Instancia.RegistrarGuiaManualMovil(guia, remitenteDestinatario, notificacion, radicado);
            return new ADRetornoAdmision
            {
                NumeroGuia = resultado.NumeroGuia,
                FechaGrabacion = System.DateTime.Now,
                PrefijoGuia = guia.PrefijoNumeroGuia
            };
        }        
        #endregion
    }
}