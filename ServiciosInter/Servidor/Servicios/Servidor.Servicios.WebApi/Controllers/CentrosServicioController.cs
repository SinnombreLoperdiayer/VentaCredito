using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.Implementacion.CentroServicios;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Bodega;
using CO.Servidor.Servicios.WebApi.ModelosRequest.CentroServicio;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [RoutePrefix("api/CentrosServicio")]
    ///Clase que expone los servicios REST para el modulo de CentrosServicio PUA
    public class CentrosServicioController : ApiController
    {



        [HttpGet]
        [Route("ObtenerCentroServicio")]
        // [SeguridadWebApi]
        public List<PUCentroServiciosDC> ObtenerCentroServicio()
        {
            return ApiCentrosServicio.Instancia.ObtenerCentroServicio();
        }

        [HttpGet]
        [Route("ConsultaGuiasCustodia/{idTipoMovimiento}/{idEstadoGuia}/{numeroGuia}/{muestraReportemuestraTodosreporte}")]
        [SeguridadWebApi]
        [AllowAnonymous]
        public List<PUCustodia> CunsultaGuiasCustodia([FromUri]int idTipoMovimiento, [FromUri]Int16 idEstadoGuia, [FromUri]long? numeroGuia, [FromUri]bool muestraReportemuestraTodosreporte)
        {
            return ApiCentrosServicio.Instancia.ConsultaGuiasCustodia(idTipoMovimiento, idEstadoGuia, numeroGuia, muestraReportemuestraTodosreporte);
        }

        /// <summary>
        /// Obtiene los horarios de un centro de servicios para la app de recogidas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerHorariosCentroServicioAppRecogidas")]
        // [SeguridadWebApi]
        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {
            return ApiCentrosServicio.Instancia.ObtenerHorariosCentroServicioAppRecogidas(idCentroServicio);
        }

        //Lista los motivos de salida de custodia(Disposicion final)
        [HttpGet]
        [Route("ObtenerMotivosSalida")]
        [SeguridadWebApi]
        [AllowAnonymous]
        public List<ADMotivoGuiaDC> ObtenerMotivosSalida()
        {
            return null;
            //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed) { Content = new StringContent("arreglar metodo") });

            /*

            try
            {
                List<ADMotivoGuiaDC> retorno = FabricaServicios.ServicioLogisticaInversa.ObtenerMotivosGuias(ADEnumTipoMotivoDC.DisposicionFinal).ToList();
                int a = 0;
                return retorno;
            }
            catch (System.ServiceModel.FaultException<Framework.Servidor.Excepciones.ControllerException> exController)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed) { Content = new StringContent(exController.Detail.Mensaje) });
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }*/

        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerInformacionGeneralCentrosServicio")]
        //[SeguridadWebApi]        
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            return ApiCentrosServicio.Instancia.ObtenerInformacionGeneralCentrosServicio();
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerPosicionesCanalesVenta")]
        //[SeguridadWebApi]        
        public List<PUCentroServicioInfoGeneral> ObtenerPosicionesCanalesVenta([FromUri]DateTime fechaInicial, [FromUri]DateTime fechaFinal, [FromUri]int idEstado, [FromUri]string idMensajero = null, [FromUri]string idCentroServicio=null)
        {
            return ApiCentrosServicio.Instancia.ObtenerPosicionesCanalesVenta(fechaInicial, fechaFinal, idMensajero, idCentroServicio, idEstado);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerServiciosCentroServicio/{idCentroServicio}")]
        //[SeguridadWebApi]      
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            return ApiCentrosServicio.Instancia.ObtenerServiciosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerCentrosServicioPorServicio/{idServicio}")]
        public List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio)
        {
            return ApiCentrosServicio.Instancia.ObtenerCentrosServicioPorServicio(idServicio);
        }

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        [HttpGet]
        [Route("ObtenerTodosColes")]
        public List<PUCentroServiciosDC> ObtenerTodosColes()
        {
            return ApiCentrosServicio.Instancia.ObtenerTodosColes();
        }

        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("ObtenerPuntosAgenciasCol/{idCol}")]
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasCol(long idCol)
        {
            return ApiCentrosServicio.Instancia.ObtenerPuntosAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene los centros de servicio a los cuales tiene acceso el usuario
        /// </summary>
        /// <param name="identificacionUsuario"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerLocacionesAutorizadas/{usuario}")]
        public List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario)
        {
            return ApiCentrosServicio.Instancia.ObtenerLocacionesAutorizadas(usuario);
        }

        /// <summary>
        /// Obtiene el numero total de envios en custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerConteoGuiasCustodia/{idTipoMovimiento}/{idEstadoGuia}")]
        public int ObtenerConteoGuiasCustodia([FromUri]int idTipoMovimiento, [FromUri]int idEstadoGuia)
        {
            return ApiCentrosServicio.Instancia.ObtenerConteoGuiasCustodia(idTipoMovimiento, idEstadoGuia);
        }

        /// <summary>
        /// Obtiene el numero total de envios en pendientyes por ingr a custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerConteoPendIngrCustodia/{idTipoMovimiento}/{idEstadoGuia}")]
        public int ObtenerConteoPendIngrCustodia([FromUri]int idTipoMovimiento, [FromUri]int idEstadoGuia)
        {
            return ApiCentrosServicio.Instancia.ObtenerConteoPendIngrCustodia(idTipoMovimiento, idEstadoGuia);
        }


        /// <summary>
        /// Obtiene los municipios segun el id del col
        /// </summary>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("ObtenerMunicipiosXCol/{idCol}")]
        public List<PALocalidadDC> ObtenerMunicipiosXCol([FromUri]long idCol)
        {
            return ApiCentrosServicio.Instancia.ObtenerMunicipiosXCol(idCol);
        }

        /// <summary>
        /// Obtiene las agencias y puntos por Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerCentrosServiciosPorRacol/{idRacol}")]
        [SeguridadWebApi]
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosPorRacol([FromUri]long idRacol)
        {
            return ApiCentrosServicio.Instancia.ObtenerCentrosServiciosPorRacol(idRacol);
        }

        /// <summary>
        /// Obtiene los horarios de recogida del centro de servicio
        /// </summary>
        /// <param name="idCentroSvc"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerHorariosRecogidasCentroSvc/{idCentrServicio}")]
        [SeguridadWebApi]
        public IList<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc)
        {
            return ApiCentrosServicio.Instancia.ObtenerHorariosRecogidasCentroSvc(idCentroSvc);
        }


        [HttpGet]
        [Route("ObtenerCentrosServicio")]
        [SeguridadWebApi]
        public List<PUCentroServiciosDC> ObtenerCentrosServicio()
        {
            return ApiCentrosServicio.Instancia.ObtenerCentrosServicio();
        }

        [HttpGet]
        [Route("ObtenerCentrosServicioTipo")]
        [SeguridadWebApi]
        public List<PUCentroServiciosDC> ObtenerCentrosServicioTipo()
        {
            return ApiCentrosServicio.Instancia.ObtenerCentrosServicioTipo();
        }

        [HttpGet]
        [Route("ObtenerTodosCentrosServicioPorLocalidad/{idMunicipio}")]
        [SeguridadWebApi]
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio)
        {
            return ApiCentrosServicio.Instancia.ObtenerTodosCentrosServicioPorLocalidad(idMunicipio);
        }

        [HttpGet]
        [Route("ObtenerTiposCiudades")]
        [SeguridadWebApi]
        public List<PUTipoCiudad> ObtenerTiposCiudades()
        {
            return ApiCentrosServicio.Instancia.ObtenerTiposCiudades();
        }

        [HttpGet]
        [Route("ObtenerTiposZona")]
        [SeguridadWebApi]
        public List<PUTipoZona> ObtenerTiposZona()
        {
            return ApiCentrosServicio.Instancia.ObtenerTiposZona();
        }




        #region Inter Logis 

        /// <summary>
        /// Metodo para obtener los centro servicios activos 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerCentrosServiciosActivosInterLogis")]
        [SeguridadWebApi]
        public List<PUCentroServicioResponse> ObtenerCentrosServiciosActivosInterLogis()
        {
            return ApiCentrosServicio.Instancia.ObtenerCentrosServiciosActivosInterLogis();
        }

        #endregion
                      

    }
}
