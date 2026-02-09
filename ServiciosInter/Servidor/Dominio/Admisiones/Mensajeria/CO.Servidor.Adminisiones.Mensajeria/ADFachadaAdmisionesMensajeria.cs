using CO.Servidor.Adminisiones.Mensajeria.Contado;
using CO.Servidor.Adminisiones.Mensajeria.Credito;
using CO.Servidor.Adminisiones.Mensajeria.GuiaInterna;
using CO.Servidor.Adminisiones.Mensajeria.Novedades;
using CO.Servidor.Adminisiones.Mensajeria.Volantes;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
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
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.Adminisiones.Mensajeria
{                                                 
    public class ADFachadaAdmisionesMensajeria : IADFachadaAdmisionesMensajeria
    {
        public ADFachadaAdmisionesMensajeria()
        {
        }

        private static readonly ADFachadaAdmisionesMensajeria instancia = new ADFachadaAdmisionesMensajeria();

        /// <summary>
        /// Retorna una instancia de la fachada de admisiones
        /// /// </summary>
        public static ADFachadaAdmisionesMensajeria Instancia
        {
            get { return ADFachadaAdmisionesMensajeria.instancia; }
        }

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para dicho trayecto
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, decimal pesoGuia)
        {
            return ADAdmisionContado.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, centroServiciosOrigen,pesoGuia);
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
        /// <param name="idListaPrecios">Identificador de la lista de precios asociada al contrato</param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idSucursal, int idCliente, int idListaPrecios, decimal pesoGuia)
        {
            return ADAdmisionCredito.Instancia.ValidarServicioTrayectoDestinoCliente(municipioOrigen, municipioDestino, servicio, idSucursal, idCliente, idListaPrecios, pesoGuia);
        }

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad()
        {
            return ADConsultas.Instancia.ObtenerMotivosNoUsoBolsaSeguridad();
        }

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion()
        {
            return ADConsultas.Instancia.ObtenerObjetosProhibidaCirculacion();
        }

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return ADConsultas.Instancia.ObtenerTiposEntrega();
        }

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            return ADConsultas.Instancia.ObtenerParametrosAdmisiones();
        }

        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(numeroGuia, idAdmision);
        }

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
        {
            return ADConsultas.Instancia.ObtenerMotivosAnulacion();
        }

        #region Cálculo de precios en tarifas

        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        public TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCentroCorrespondencia(idListaPrecios);
        }

        /// <summary>
        /// Calcular el precio para una tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        public TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioInternacional(idListaPrecios, tipoEmpaque, idLocalidadDestino, peso, idZona, valorDeclarado);
        }

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioTramiteDC CalcularPrecioTramites(int idListaPrecios, int idTramite)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioTramites(idListaPrecios, idTramite);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioMensajeriaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }


        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioCargaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCargaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiTulas(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresMsj(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiValoresMsj(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresCarga(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiValoresCarga(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiCargaConsolidado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiCargaConsolidado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValijas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiValijas(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
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
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCargaExpress(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioCargaAerea(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad)
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiPromocional(idListaPrecios, cantidad);
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
        public TAPrecioMensajeriaDC CalcularPrecioRapiradicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiRadicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiPersonalizado(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiHoy(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiEnvioContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiCargaContraPago(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiCarga(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioRapiAm(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            ITAFachadaTarifas tarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            return tarifas.CalcularPrecioNotificaciones(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        #endregion Cálculo de precios en tarifas

        #region Inserciones

        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        public long AdicionarAdmisionAnulada(ADGuia guia)
        {
            return ADAdmisionContado.Instancia.AdicionarAdmisionAnulada(guia);
        }

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        public ADGuiaInternaDC AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            return ADAdmisionGuiaInterna.Instancia.AdicionarGuiaInterna(guiaInterna);
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManual(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario);
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManualCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManual(guia, idCaja, remitenteDestinatario, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualCOL(guia, remitenteDestinatario, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManualInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            }
        }

        /// <summary>
        /// Registra un movimiento de admisiones de mensajería manual
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaManualInternacionalCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualInternacionalCOL(guia, remitenteDestinatario, tipoEmpaque, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Se registra un movimiento de mensajería
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        public ADResultadoAdmision RegistrarGuiaAutomatica(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomatica(guia, idCaja, remitenteDestinatario);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomatica(guia, idCaja, remitenteDestinatario);
            }
        }

        /// <summary>
        /// Valida si un al cobro especifico está asignado a un coordinador de col x vencimiento en el pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public bool AlCobroCargadoACoordinadorCol(long idAdmision)
        {
            return ADAdmisionMensajeria.Instancia.AlCobroCargadoACoordinadorCol(idAdmision);
        }

        /// <summary>
        /// Registra guía automática internacional
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="tipoEmpaque"></param>
        /// <returns></returns>
        public ADResultadoAdmision RegistrarGuiaAutomaticaInternacional(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, TATipoEmpaque tipoEmpaque)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaInternacional(guia, idCaja, remitenteDestinatario, tipoEmpaque);
            }
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        public ADResultadoAdmision RegistrarGuiaAutomaticaNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            }
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        public ADResultadoAdmision RegistrarGuiaManualNotificacion(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion);
            }
        }

        /// <summary>
        /// Registra guia cuy oservicio es notificación
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="notificacion"></param>
        public ADResultadoAdmision RegistrarGuiaManualNotificacionCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADNotificacion notificacion, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualNotificacion(guia, idCaja, remitenteDestinatario, notificacion, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualNotificacionCOL(guia, remitenteDestinatario, notificacion, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        public ADResultadoAdmision RegistrarGuiaAutomaticaRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPago(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago);
            }
        }

        /// <summary>
        /// Regitra guía cuyo servicio es rapi envío contra pago
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiEnvioContraPago"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiEnvioContraPagoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiEnvioContraPagoDC rapiEnvioContraPago, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiEnvioContraPago(guia, idCaja, remitenteDestinatario, rapiEnvioContraPago, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiEnvioContraPagoCOL(guia, remitenteDestinatario, rapiEnvioContraPago, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        public ADResultadoAdmision RegistrarGuiaAutomaticaRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaAutomaticaRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaAutomaticaRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            }
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiRadicado(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado, false);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado);
            }
        }

        /// <summary>
        /// Registra la guía cuyo servicio es rapi radicado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="idCaja"></param>
        /// <param name="remitenteDestinatario"></param>
        /// <param name="rapiRadicado"></param>
        /// <param name="destinosRadicado"></param>
        public ADResultadoAdmision RegistrarGuiaManualRapiRadicadoCOL(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, ADRapiRadicado rapiRadicado, long idAgenciaRegistraAdmision)
        {
            if (guia.IdCliente != 0)
            {
                return ADAdmisionCredito.Instancia.RegistrarGuiaManualRapiRadicado(guia, idCaja, remitenteDestinatario, rapiRadicado, true, idAgenciaRegistraAdmision);
            }
            else
            {
                return ADAdmisionContado.Instancia.RegistrarGuiaManualRapiRadicadoCOL(guia, remitenteDestinatario, rapiRadicado, idAgenciaRegistraAdmision);
            }
        }

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarPagadoGuia(long idAdmisionMensajeria, bool estaPagada=true)
        {
            ADAdmisionMensajeria.Instancia.ActualizarPagadoGuia(idAdmisionMensajeria, estaPagada);
        }

        /// <summary>
        /// Actualiza en supervision una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarSupervisionGuia(long idAdmisionMensajeria)
        {
            ADAdmisionMensajeria.Instancia.ActualizarSupervisionGuia(idAdmisionMensajeria);
        }

        #endregion Inserciones

        #region Consultas

        /// <summary>
        /// Obtiene la admision de mensajeria de rapi envio contra pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADRapiEnvioContraPagoDC ObtenerRapiEnvioContraPago(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerRapiEnvioContraPago(idAdmision);
        }

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerArchivoAlmacenGuia(numeroGuia);
        }

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
        public List<ADGuia> ObtenerGuiasParaCapturaAutomatica(short idFormaPago, DateTime fechaInicio, DateTime fechaFin, int? idCliente, long idRacol)
        {
            return ADConsultas.Instancia.ObtenerGuiasParaCapturaAutomatica(idFormaPago, fechaInicio, fechaFin, idCliente, idRacol);
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// La guía debe estar en estado "Devolución" o "Entrega" y la prueba de entrega o de devolución
        /// correspondiente debe estar digitalizada en la aplicación
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaParaRecibirManualNotificaciones(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaParaRecibirManualNotificaciones(numeroGuia);
        }

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            return Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerCondicionesPorOperadorPostal(idOperadorPostal);
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroGuia, long idSucursalCentroServicio)
        {
            return ADConsultas.Instancia.ObtenerPropietarioGuia(numeroGuia, idSucursalCentroServicio);
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioConvenio(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerAdmisionMensajeriaConvenioConvenio(idAdmision);
        }

        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision ObtenerGuiaPorGuid(string guid)
        {
            return ADConsultas.Instancia.ObtenerGuiaPorGuid(guid);
        }

        /// <summary>
        /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia)
        {
            return ADConsultas.Instancia.ObtenerMensajeriaUltimoEstado(idNumeroGuia);
        }

        /// <summary>
        /// Obtiene las formas de pago de una guia
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public List<ADGuiaFormaPago> ObtenerFormasPagoGuia(long idGuia)
        {
            return ADConsultas.Instancia.ObtenerFormasPagoGuia(idGuia);
        }

        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        public ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerMensajeriaFormaPago(idAdmision);
        }


                /// <summary>
        /// Consultar el contrato de n cliente Convenio
        /// </summary>
        /// <param name="TipoCliente"></param>
        /// <param name="idAdmisionMensajeria"></param>
        /// <returns></returns>
        public int ObtenerContratoClienteConvenio(ADEnumTipoCliente tipoCliente, long idAdmisionMensajeria)
        {
            return ADConsultas.Instancia.ObtenerContratoClienteConvenio(tipoCliente, idAdmisionMensajeria);
        }

        /// <summary>
        /// Metodo que obtiene la información de una admisión de mensajeria a partir del numero de la misma
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuia(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerGuia(idAdmision);
        }

        /// <summary>
        /// Método para calcular y guardar comisiones
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="tipoComision"></param>
        /// <param name="?"></param>
        public void AdicionarComision(ADGuia guia, CMEnumTipoComision tipoComision)
        {
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            CMComisionXVentaCalculadaDC comision = ADAdmisionMensajeria.Instancia.CalcularComisiones(guia, tipoComision, fachadaComisiones);
            fachadaComisiones.GuardarComision(comision);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuiaCredito(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaXNumeroGuiaCredito(numeroGuia);
        }
        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio - peaton
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioPeaton(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerAdmisionMensajeriaConvenioPeaton(idAdmision);
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente peaton - peaton
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonPeaton(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerAdmisionMensajeriaPeatonPeaton(idAdmision);
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerInfoGuiaXNumeroGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerInfoGuiaXNumeroGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene el identificador de la admisión
        /// </summary>
        /// <param name="numeroGuia">Número de guía</param>
        /// <returns>Identificador admisión</returns>
        public long ObtenerIdentificadorAdmision(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerIdentificadorAdmision(numeroGuia);
        }

        /// <summary>
        /// Retorna la información de una guía completa incluyendo la forma como se pagó, se construyó para generar impresión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADConsultas.Instancia.ObtenerGuiaPorNumeroDeGuiaCompleta(numeroGuia, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADConsultas.Instancia.ObtenerGuiasPorRemitenteParaHoy(idRemitente, tipoIdRemitente, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long? idCentroServicios, int? idSucursal, int? idCliente)
        {
            return ADConsultas.Instancia.ObtenerGuiasPorDestinatarioParaHoy(idDestinatario, tipoIdDestinatario, idCentroServicios, idSucursal, idCliente);
        }

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        public List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long idCentroServicio)
        {
            return ADConsultas.Instancia.ObtenerGuiasAlCobroNoPagas(indicePagina, registrosPorPagina, numeroGuia, fechaInicial, fechaFinal, idCentroServicio);
        }

        /// <summary>
        /// Actualiza la guía y registra el valor en Caja de la transaccion.
        /// </summary>
        /// <param name="guiaAlCobro">The guia al cobro.</param>
        public ADRecaudoAlCobro ActualizarGuiaAlCobro(ADRecaudarDineroAlCobroDC guiaAlCobro)
        {
            return ADAdmisionMensajeria.Instancia.ActualizarGuiaAlCobro(guiaAlCobro);
        }

        /// <summary>
        /// Método para obtener una guía de temelercado con sus respectivos valores adicionales
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaTelemercadeo(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaTelemercadeo(numeroGuia);
        }

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumeroGuias)
        {
            return ADConsultas.Instancia.ObtenerGuiasInternas(numeroInicial, numeroFinal, listaNumeroGuias);
        }

        /// <summary>
        /// Método para obtener una guía interna
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public ADGuiaInternaDC ObtenerGuiaInterna(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaInterna(numeroGuia);
        }

        /// <summary>
        /// Método para obtener una guía interna a partir de un numero de guia, si no existe la guia genere excepción
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public ADGuiaInternaDC ObtenerGuiaInternaNumeroGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerGuiaInternaNumeroGuia(numeroGuia);
        }

        public List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino)
        {
            return ADConsultas.Instancia.ObtenerGuiasGestion(idEstadoGuia, IdCentroServicioDestino);
        }

        public List<ADTrazaGuiaAgencia> ObtenerGuiasGestionAgencias(int idEstadoGuia, long IdCol)
        {
            return ADConsultas.Instancia.ObtenerGuiasGestionAgencias(idEstadoGuia, IdCol);
        }

        /// <summary>
        /// Genera solicitud de falla
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="valorCobrado"></param>
        /// <param name="valorCalculado"></param>
        public void GenerarFallaCalculoValorGuiaManual(long numeroGuia, decimal valorCobrado, decimal valorCalculado)
        {
            ADAdmisionContado.Instancia.GenerarFallaCalculoValorGuiaManual(numeroGuia, valorCobrado, valorCalculado);
        }

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerRapiradicadosGuia(numeroGuia);
        }

        /// <summary>
        /// Obtener las notificaciones de una guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADNotificacion ObtenerNotificacionGuia(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerNotificacionGuia(idAdmision);
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias)
        {
            return ADConsultas.Instancia.ObtenerListaGuias(listaNumerosGuias);
        }

        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return ADConsultas.Instancia.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro)
        {
            return ADConsultas.Instancia.ObtenerIdNotificaciones(filtro);
        }
        public ADGuia ConsultarGuia(int idCliente, long numeroGuia)
        {
            return ADConsultas.Instancia.ConsultarGuia(idCliente,numeroGuia);
        }
        /// <summary>
        /// Método para obtener las guías de servicio rapiradicado
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro)
        {
            return ADConsultas.Instancia.ObtenerGuiasRapiradicados(filtro);
        }

        /// <summary>
        /// Obtiene todas las guias en estado en centro de acopio en una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<ADGuiaUltEstadoDC> ObtenerGuiasEnCentroAcopioLocalidad(string idLocalidad)
        {
            return ADConsultas.Instancia.ObtenerGuiasEnCentroAcopioLocalidad(idLocalidad);
        }

        /// <summary>
        /// metodo que retona el ultimo numero de factura de la guia automatica
        /// </summary>
        /// <returns></returns>
        public SUNumeradorPrefijo ObtenerConsecutivoFacturaVenta()
        {
            return ADAdmisionContado.Instancia.ObtenerConsecutivoFacturaVenta();
        }

        /// <summary>
        /// Consulta la informacion remitente detinatario por numero guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerRemitenteDestinatarioGuia(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerRemitenteDestinatarioGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene la traza de una guia dependiendo del id unico de su estado en admisionmensajeria
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADEstadoGuia ObtenerEstadoGuiaTrazaPorIdEstado(ADEnumEstadoGuia idEstadoGuia, long numeroGuia)
        {
            return ADAdmisionMensajeria.Instancia.ObtenerEstadoGuiaTrazaPorIdEstado(idEstadoGuia, numeroGuia);
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

        #endregion Consultas

        #region Novedades

        /// <summary>
        /// Adicionar novedades de una guia
        /// </summary>
        /// <param name="novedad"></param>
        public void AdicionarNovedad(ADNovedadGuiaDC novedad, Dictionary<CCEnumNovedadRealizada, string> datosAdicionalesNovedad)
        {
            ADNovedades.Instancia.AdicionarNovedad(novedad, datosAdicionalesNovedad);
        }

        /// <summary>
        /// Actualizar destino de una guia
        /// </summary>
        /// <param name="centroServicioDestino"></param>
        public void ActualizarDestinoGuia(long idAdmisionMensajeria, PUCentroServiciosDC centroServicioDestino, CCReliquidacionDC valorReliquidado, TAEnumFormaPago? formaPago, string idTipoEntrega, string descripcionTipoEntrega)
        {
            ADNovedades.Instancia.ActualizarDestinoGuia(idAdmisionMensajeria, centroServicioDestino, valorReliquidado, formaPago, idTipoEntrega, descripcionTipoEntrega);
        }

        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void ActualizarRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia)
        {
            ADNovedades.Instancia.ActualizarRemitenteDestinatarioGuiaV7(novedadGuia);
        }

        /// <summary>
        /// Actualizar formas de pago de una guia
        /// </summary>
        public void ActualizarFormaPagoGuia(CCNovedadCambioFormaPagoDC novedadGuia)
        {
            ADNovedades.Instancia.ActualizarFormaPagoGuia(novedadGuia);
        }

        /// <summary>
        /// Actualizar es alcobro
        /// </summary>
        public void ActualizarEsAlCobro(long idAdmisionGuia, bool esAlCobro)
        {
            ADNovedades.Instancia.ActualizarEsAlCobro(idAdmisionGuia, esAlCobro);
        }

        /// <summary>
        /// Actualiza el tipo de servicio de una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void ActualizarTipoServicioGuia(long idAdmisionGuia, int idServicio)
        {
            ADNovedades.Instancia.ActualizarTipoServicioGuia(idAdmisionGuia, idServicio);
        }

        #endregion Novedades

        #region Eliminación

        /// <summary>
        /// Metodo para eliminar una admisión con auditoria
        /// </summary>
        /// <param name="idAdmision"></param>
        public void EliminarAdmision(long idAdmision)
        {
            ADAdmisionGuiaInterna.Instancia.EliminarAdmision(idAdmision);
        }

        /// <summary>
        /// Eliminar una guía interna
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            ADAdmisionMensajeria.Instancia.EliminarGuiaInterna(guiaInterna);
        }

        #endregion Eliminación

        #region Adicionar volantes

        /// <summary>
        /// Adiciona archivo de un volante
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivo(LIArchivosDC archivo)
        {
             ADAdmisionVolantes.Instancia.AdicionarArchivo(archivo);
        }

        /// <summary>
        /// Metodo para guardar un volante de devolución
        /// </summary>
        /// <param name="volante"></param>
        /// <returns></returns>
        public long AdicionarEvidenciaDevolucion(LIEvidenciaDevolucionDC evidenciaDevolucion)
        {
            return ADAdmisionVolantes.Instancia.AdicionarEvidenciaDevolucion(evidenciaDevolucion);
        }

        /// <summary>
        /// Edita un archivo de evidencia de devolución
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoEvidencia(LIArchivosDC imagen)
        {
            ADAdmisionVolantes.Instancia.EditarArchivoEvidencia(imagen);
        }

        /// <summary>
        /// Adiciona archivo de un radicado
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivosRapiradicado(ADRapiRadicado radicado)
        {
            ADAdmisionMensajeria.Instancia.AdicionarArchivosRapiradicado(radicado);
        }

        #endregion Adicionar volantes

        #region Actualizaciones

        public void ActualizarReintentosEntrega(long idAdmision)
        {
            ADAdmisionMensajeria.Instancia.ActualizarReintentoEntregasAdmision(idAdmision);
        }

        /// <summary>
        /// Método para actualizar el número de guía interna
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void ActualizarGuiaRapiradicado(long idRadicado, long numeroGuiaInterna)
        {
            ADAdmisionMensajeria.Instancia.ActualizarGuiaRapiradicado(idRadicado, numeroGuiaInterna);
        }

        /// <summary>
        /// Método para actualizar una notificación
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarNotificacion(long idAdmision, long numeroGuia)
        {
            ADNovedades.Instancia.ActualizarNotificacion(idAdmision, numeroGuia);
        }



        /// <summary>
        /// Método para actualizar una notificacion cuando es sacada de una planilla
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarNotificacionPlanilla(long idAdmision)
        {
            ADNovedades.Instancia.ActualizarNotificacionPlanilla(idAdmision);
        }

        /// <summary>
        /// Actualizar el valor total de una guía dada
        /// </summary>
        /// <param name="idAdmisionGuia"></param>
        /// <param name="ValorTotal"></param>
        public void ActualizarValorTotalGuia(CCNovedadCambioValorTotal novedadGuia)
        {
            ADNovedades.Instancia.ActualizarValorTotalGuia(novedadGuia);
        }

        /// <summary>
        /// Actualizar valores de la guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">nmero de la guia</param>
        /// <param name="valores">valores a modificar</param>
        /// <param name="valorAdicionales">valores adicionales que se agregan al total</param>
        public void ActualizarValoresGuia(long idAdmisionGuia, CCReliquidacionDC valores, decimal valorAdicionales)
        {
            ADNovedades.Instancia.ActualizarValoresGuia(idAdmisionGuia, valores, valorAdicionales);
        }

        /// <summary>
        /// Actualiza el Valor del Peso de la Guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">numeor de la Guía</param>
        /// <param name="valorPeso">Valor del peso a actualizar</param>
        public void ActualizarValorPesoGuia(long idAdmisionGuia, decimal valorPeso)
        {
            ADNovedades.Instancia.ActualizarValorPesoGuia(idAdmisionGuia, valorPeso);
        }

        /// <summary>
        /// Actualiza el valor del porcentaje de
        /// recargo
        /// </summary>
        /// <param name="valor">el valor a actualizar</param>
        public void ActualizarParametroPorcentajeRecargo(double porcentaje)
        {
            ADAdmisionMensajeria.Instancia.ActualizarParametroPorcentajeRecargo(porcentaje);
        }

        /// <summary>
        /// Método para actualizar el campo entregado en la tabla de admisión mensajeria
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void ActualizarEntregadoGuia(long numeroGuia)
        {
            ADAdmisionMensajeria.Instancia.ActualizarEntregadoGuia(numeroGuia);
        }

        #endregion Actualizaciones

        #region Reexpedicion

        /// <summary>
        /// Retorna lista de valores adicionales agregados a una admisión
        /// </summary>
        /// <param name="IdAdmision"></param>
        /// <returns></returns>
        public List<TAValorAdicional> ObtenerValoresAdicionales(long IdAdmision)
        {
            return ADConsultas.Instancia.ObtenerValoresAdicionales(IdAdmision);
        }

        /// <summary>
        /// Guarda la relacion de las guias
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void GuadarRelacionReexpedicionEnvio(long idAdmisionOriginal, long idAdmisionNueva)
        {
            ADAdmisionMensajeria.Instancia.GuadarRelacionReexpedicionEnvio(idAdmisionOriginal, idAdmisionNueva);
        }

        /// <summary>
        /// Obtiene la admision de mensajeria peaton-convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonConvenio(long idAdmision)
        {
            return ADConsultas.Instancia.ObtenerAdmisionMensajeriaPeatonConvenio(idAdmision);
        }

        #endregion Reexpedicion

        #region Orden de Servicio Cargue Masivo

        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        public long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio)
        {
            return ADAdmisionMensajeria.Instancia.CrearOrdenServicioMasivo(datosOrdenServicio);
        }

        /// <summary>
        /// Validar la existencia de una orden de servicio masiva
        /// </summary>
        /// <param name="ordenServicio">número de la orden de servicio a validar</param>
        /// <returns></returns>
        public bool ValidarOrdenServicio(long ordenServicio)
        {
            return ADAdmisionMensajeria.Instancia.ValidarOrdenServicio(ordenServicio);
        }

        /// <summary>
        /// Valida cada uno de los envíos que se desea asociar a una orden de servicio antes de almacenarla en la base de datos
        /// </summary>
        /// <param name="enviosParaValidar">Listado de los envíos que se desean validar</param>
        /// <returns>Listado de los envíos validados y su respectivo resultado</returns>
        public List<ADDatosValidadosDC> ValidarDatosGuiasOrdenMasiva(List<ADDatosValidacionDC> enviosParaValidar)
        {
            List<ADDatosValidadosDC> datosValidados = new List<ADDatosValidadosDC>();
            foreach (ADDatosValidacionDC datoValidacion in enviosParaValidar)
            {
                ADDatosValidadosDC datoValidado = new ADDatosValidadosDC();
                try
                {
                    datoValidado.NoFila = datoValidacion.NoFila;
                    datoValidado.ValidacionTrayectoDestino = ValidarServicioTrayectoDestinoCliente(datoValidacion.MunicipioOrigen, datoValidacion.MunicipioDestino, datoValidacion.TipoServicio,Convert.ToInt32( datoValidacion.IdSucursalClienteOrigen), 0, datoValidacion.IdListaPrecios, datoValidacion.peso);
                    switch (datoValidacion.TipoServicio.IdServicio)
                    {
                        case (int)EnumTipoServicio.Carga_Express:
                            datoValidado.PrecioMensajeria = CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true);
                            break;

                        case (int)EnumTipoServicio.Mensajería:
                            datoValidado.PrecioMensajeria = CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true);
                            break;

                        case (int)EnumTipoServicio.Rapi_AM:
                            datoValidado.PrecioMensajeria = CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true);
                            break;

                        case (int)EnumTipoServicio.Rapi_Carga:
                            TAPrecioCargaDC precioCarga = CalcularPrecioCargaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true);
                            datoValidado.PrecioMensajeria = new TAPrecioMensajeriaDC()
                            {
                                Impuestos = precioCarga.Impuestos.ToList(),
                                Valor = precioCarga.Valor,
                                ValorKiloAdicional = precioCarga.ValorKiloAdicional,
                                ValorKiloInicial = 0,
                                ValorPrimaSeguro = precioCarga.ValorPrimaSeguro
                            };
                            break;

                        case (int)EnumTipoServicio.Rapi_Hoy:
                            datoValidado.PrecioMensajeria = CalcularPrecioMensajeriaCredito((int)datoValidacion.TipoServicio.IdServicio, datoValidacion.IdListaPrecios, datoValidacion.MunicipioOrigen.IdLocalidad, datoValidacion.MunicipioDestino.IdLocalidad, datoValidacion.peso, datoValidacion.valorDeclarado, true);
                            break;
                    };
                }
                catch (FaultException<ControllerException> exc)
                {
                    datoValidado.Error = exc.Detail.Mensaje;
                }
                catch (Exception exc)
                {
                    datoValidado.Error = exc.Message;
                }
                datosValidados.Add(datoValidado);
            }
            return datosValidados;
        }

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        public List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex)
        {
            return ADAdmisionMensajeria.Instancia.ConsultarGuiasDeOrdenDeServicio(idOrdenServicio, pageSize, pageIndex);
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
            return ADAdmisionMensajeria.Instancia.ObtenerOrdenesServicioPorFecha(fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        public long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio)
        {
            return ADAdmisionMensajeria.Instancia.ConsultarCantidadGuiasOrdenSerServicio(idOrdenServicio);
        }

        /// <summary>
        /// Se registra un movimiento de mensajería asociado a una orden de servicio. Esto se utiliza en
        /// cuando se hace cargue masivo de guias o facturas.
        /// </summary>
        /// <param name="guia">Datos de la guia</param>
        /// <param name="idCaja">Caja que hace la operacion</param>
        /// <param name="remitenteDestinatario">Datos del remitente y el destinatario</param>
        /// <returns>Número de guía</returns>
        public List<ADRetornoAdmision> RegistrarGuiasDeOrdenDeServicio(List<ADGuiaOSDC> guias)
        {
            long numeroGuia = 0;
            List<ADRetornoAdmision> resultadosGrabacion = new List<ADRetornoAdmision>();

            // Se obtiene el número de guía
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            SUNumeradorPrefijo numeroSuministro;

            guias.ForEach((g) =>
            {
                ADRetornoAdmision retorno = new ADRetornoAdmision();
                try
                {
                    using (TransactionScope tx = new TransactionScope())
                    {
                        if (g.Guia.IdCliente != 0)
                        {
                            numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GUIA__TRANSPORTE_AUTOMATICA);
                        }
                        else
                        {
                            numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.FACTURA_VENTA_MENSAJERIA_AUTOMATICA);
                        }
                        tx.Complete();
                    }

                    g.Guia.NumeroGuia = numeroSuministro.ValorActual;
                    g.Guia.PrefijoNumeroGuia = numeroSuministro.Prefijo;

                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        ADAdmisionContado.Instancia.RegistrarGuiaAutomatica(g.Guia, g.IdCaja, g.DatosRemitenteDestinatario);
                        ADAdmisionMensajeria.Instancia.GuardarGuiaOrdenServicio(g.Guia.IdAdmision, numeroGuia, g.NoOrdenServicio);
                        transaccion.Complete();
                    }

                    retorno.Error = "";
                    retorno.FechaGrabacion = DateTime.Now;
                    retorno.NumeroGuia = g.Guia.NumeroGuia;
                    retorno.PrefijoGuia = g.Guia.PrefijoNumeroGuia;
                    retorno.NoFila = g.NoFila;
                }
                catch (FaultException<ControllerException> exc)
                {
                    retorno.Error = exc.Detail.Mensaje;
                    retorno.NoFila = g.NoFila;
                    retorno.FechaGrabacion = DateTime.Now;
                }
                catch (Exception exc)
                {
                    retorno.Error = exc.Message;
                    retorno.NoFila = g.NoFila;
                    retorno.FechaGrabacion = DateTime.Now;
                }
                resultadosGrabacion.Add(retorno);
            }
            );

            return resultadosGrabacion;
        }

        #endregion Orden de Servicio Cargue Masivo

        #region Notificaciones

        /// <summary>
        /// Obtiene la admision de mensajeria para el servicio de notificaciones
        /// </summary>
        /// <param name="numeroGuia"></param>
        public ADNotificacion ObtenerAdmMenNotEntregaDevolucion(long numeroGuia)
        {
            return ADConsultas.Instancia.ObtenerAdmMenNotEntregaDevolucion(numeroGuia);
        }

        /// <summary>
        /// Método para validar una guia notificacion en devolucion
        /// </summary>
        /// <param name="idAdmision"></param>
        public ADNotificacion ValidarNotificacionDevolucion(long idAdmision)
        {
            return  ADConsultas.Instancia.ValidarNotificacionDevolucion(idAdmision);
        }

                /// <summary>
        /// Método para actualizar la tabla notificacion campo ADN_EstaDevuelta , campo ADN_NumeroGuiaInterna
        /// </summary>
        /// <param name="guia"></param>
       public void ActualizarPLanilladaNotificacion(ADNotificacion guia)
        {
            ADConsultas.Instancia.ActualizarPLanilladaNotificacion(guia);
        }

       /// <summary>
       /// Inserta un registro de una notificacion
       /// </summary>
       /// <param name="numeroGuia"></param>
       public void AdicionarNotificacion(long numeroGuia)
    {
        ADAdmisionMensajeria.Instancia.AdicionarNotificacion(numeroGuia);
    }

        #endregion Notificaciones

        #region Confirmar Direccion

       /// <summary>
       /// Método para verificar una direccion
       /// </summary>
       /// <param name="numeroGuia"></param>
       /// <param name="verificadoPor"></param>
       /// <param name="destinatario"></param>
       public void ConfirmarDireccion(long numeroGuia, string verificadoPor, bool destinatario, bool remitente)
       {
           ADConsultas.Instancia.ConfirmarDireccion(numeroGuia, verificadoPor, destinatario, remitente);
       }

        #endregion


        #region SISPOSTAL

        public bool ValidarCredencialSispostal(credencialDTO credencial)
        {
            return ADAdmisionGuiaInterna.Instancia.ValidarCredencialSispostal(credencial);
        }
       #endregion

    }
}