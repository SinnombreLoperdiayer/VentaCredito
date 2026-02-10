using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Web.Http;



namespace CO.Servidor.Servicios.WebApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/AdmisionMensajeria")]
    ///Clase que expone los servicios REST para el modulo de Mensajeria MEN
    public class AdmisionMensajeriaController : ApiController
    {


        [HttpPost]
        [Route("RegistrarGuiaAutomatica")]
        [SeguridadWebApi]
        public ADRetornoAdmision RegistrarGuiaAutomatica([FromBody]AdmisionRequest guia)
        {
            return ApiAdmisionMensajeria.Instancia.RegistrarGuiaAutomatica(guia);
        }

        [HttpGet]
        [Route("ObtenerGuiaNumeroGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public ADGuia ObtenerGuiaNumeroGuia([FromUri]long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerGuiaNumeroGuia(numeroGuia);
        }

        /// <summary>
        /// Registra una guia manual offline
        /// </summary>
        /// <param name="guia"></param>
        [HttpPost]
        [Route("RegistrarGuiaManualOffLine")]
        [SeguridadWebApi]
        public ADRetornoAdmision RegistrarGuiaManualOffLine([FromBody]AdmisionRequest guia)
        {
            return ApiAdmisionMensajeria.Instancia.RegistrarGuiaManualOffLine(guia);
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una cadena separada por comas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerListaGuiasSeparadaComas/{listaNumerosGuias}")]
        [SeguridadWebApi]
        public List<ADTrazaGuia> ObtenerListaGuiasSeparadaComas([FromUri]string listaNumerosGuias)
        {
            List<ADTrazaGuia> aaa = ApiAdmisionMensajeria.Instancia.ObtenerListaGuiasSeparadaComas(listaNumerosGuias);
            return aaa;
        }


        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerEstadosGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerEstadosGuia(numeroGuia);
        }

        [HttpGet]
        [Route("ObtenerEstadoGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public List<ADTrazaGuia> ObtenerEstadoGuia(long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerEstadosGuia(numeroGuia);
        }

        [HttpGet]
        [Route("ObtenerGuiasGestion/{idEstadoGuia}/{IdCentroServicioDestino}")]
        [SeguridadWebApi]
        public List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerGuiasGestion(idEstadoGuia, IdCentroServicioDestino);
        }

        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("ObtenerEstadosMotivosGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerEstadosMotivosGuia(numeroGuia);
        }


        [HttpGet]
        [Route("ObtenerArchivoGuiaxNumeroGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerArchivoGuiaxNumeroGuia(numeroGuia);
        }

        [HttpGet]
        [Route("ObtenerInformacionTelemercadeoGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerInformacionTelemercadeoGuia(numeroGuia);
        }

        [HttpGet]
        [Route("ObtenerNovedadesTransporteGuia/{numeroGuia}")]
        [SeguridadWebApi]
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerNovedadesTransporteGuia(numeroGuia);
        }

        [HttpGet]
        [Route("ObtenerVolantesGuia")]
        [SeguridadWebApi]
        public List<string> ObtenerVolantesGuia([FromUri]long numeroGuia)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerVolantesGuia(numeroGuia);
        }

        [HttpPost]
        [Route("RegistrarGuiaManual")]
        [SeguridadWebApi]
        public ADRetornoAdmision RegistrarGuiaManual([FromBody]AdmisionRequest guia)
        {
            return ApiAdmisionMensajeria.Instancia.RegistrarGuiaManual(guia);
        }

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        [HttpPost]
        [Route("AdicionarGuiaInterna")]
        [SeguridadWebApi]
        public ADGuiaInternaDC AdicionarGuiaInterna([FromBody]AdmisionGuiainternaRequest guiaInterna)
        {
            return ApiAdmisionMensajeria.Instancia.AdicionarGuiaInterna(guiaInterna);
        }


        /// <summary>
        /// Metodo para obtener el rastreo general de las guías solicitadas
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRastreoGuias")]
        [SeguridadWebApi]
        public List<ADRastreoGuiaDC> ObtenerRastreoGuias([FromUri]string guias)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerRastreoGuias(guias);
        }


        #region Calculo fechas Estimada entrega
        /// <summary>
        /// Calcula la fecha estimada de entrega 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <param name="fechadmisionEnvio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidarServicioTrayectoDestino")]
        [SeguridadWebApi]
        public ADTiemposFechaEstimadaDC ValidarServicioTrayectoDestino([FromUri]string idLocalidadOrigen, [FromUri]string idLocalidadDestino, [FromUri]int idServicio, [FromUri]long centroServiciosOrigen, [FromUri]decimal pesoGuia, [FromUri]DateTime? fechadmisionEnvio)
        {
            PALocalidadDC municipioOrigen = new PALocalidadDC();
            PALocalidadDC municipioDestino = new PALocalidadDC();
            TAServicioDC servicio = new TAServicioDC();
            municipioOrigen.IdLocalidad = idLocalidadOrigen;
            municipioDestino.IdLocalidad = idLocalidadDestino;
            servicio.IdServicio = idServicio;
            ADTiemposFechaEstimadaDC tiempos = new ADTiemposFechaEstimadaDC();
            ADValidacionServicioTrayectoDestino validacion = ApiAdmisionMensajeria.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, centroServiciosOrigen, pesoGuia, fechadmisionEnvio);
            tiempos.DuracionTrayectoEnHoras = validacion.DuracionTrayectoEnHoras;
            tiempos.NumeroHorasDigitalizacion = validacion.NumeroHorasDigitalizacion;
            tiempos.NumeroHorasArchivo = validacion.NumeroHorasArchivo;
            return tiempos;
        }

        #endregion

        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResultadoListaCotizar/{idLocalidadOrigen}/{idLocalidadDestino}/{peso}/{valorDeclarado}/{idTipoEntrega}")]
        [SeguridadWebApi]
        public List<TAPreciosAgrupadosDC> ResultadoListaCotizar(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            return ApiAdmisionMensajeria.Instancia.ResultadoListaCotizar(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);
        }

        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ResultadoListaCotizarApp/{idLocalidadOrigen}/{idLocalidadDestino}/{peso}/{valorDeclarado}/{idTipoEntrega}")]
        [SeguridadWebApi]
        public List<TAPreciosAgrupadosDC> ResultadoListaCotizarApp([FromUri] string idLocalidadOrigen, [FromUri] string idLocalidadDestino, [FromUri] decimal peso, [FromUri] decimal valorDeclarado, [FromUri] string idTipoEntrega)
        {
            return ApiAdmisionMensajeria.Instancia.ResultadoListaCotizar(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);
        }

        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerReporteCajaGuiasMensajeroApp/{idMensajero}")]
        [SeguridadWebApi]
        public List<ADGuia> ObtenerReporteCajaGuiasMensajeroApp([FromUri] long idMensajero)
        {
            return ApiAdmisionMensajeria.Instancia.ObtenerReporteCajaGuiasMensajeroApp(idMensajero);
        }
        
    }




}
