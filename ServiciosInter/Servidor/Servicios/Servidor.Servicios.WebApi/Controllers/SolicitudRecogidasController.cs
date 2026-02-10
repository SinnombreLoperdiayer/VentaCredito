using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.ModeloResponse.SolicitudRecogidasApp;
using CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [RoutePrefix("api/SolicitudRecogidas")]
    public class SolicitudRecogidasController : ApiController
    {


        #region Solicitud recogidas App

        /// <summary>
        /// Obtiene la lista de precios plena y vigente
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerIdListaPreciosPlenaVigente")]
        [SeguridadWebApi]
        public int ObtenerIdListaPreciosPlenaVigente()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerIdListaPreciosPlenaVigente();
        }


        /// <summary>
        /// Retorna las localidades que no son paises ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerLocalidadesColombia")]
        [SeguridadWebApi]
        public List<ResponseGenericoApp> ObtenerLocalidadesColombia()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerLocalidadesColombia();
        }
        /// <summary>
        /// Obtiene los tipos de envio
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTipoEnvio")]
        [SeguridadWebApi]
        public List<ResponseGenericoApp> ObtenerTipoEnvio()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerTipoEnvio();
        }


        /// <summary>
        /// Obtiene los tipos de entrega
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTipoEntrega")]
        [SeguridadWebApi]
        public List<ResponseGenericoApp> ObtenerTipoEntrega()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerTipoEntrega();
        }

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerPuntosAgenciasActivos")]
        [SeguridadWebApi]
        public List<CentroServiciosApp> ObtenerPuntosAgenciasActivos()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerPuntosAgenciasActivos();

        }


        /// <summary>
        /// Obtiene la ubicacion de una guia para la app del cliente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerUbicacionGuia")]
        [SeguridadWebApi]
        public UbicacionGuiaResponse ObtenerUbicacionGuia([FromUri]long numeroGuia)
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerUbicacionGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConsultarValorComercialPeso")]
        [SeguridadWebApi]
        public ValorComercialResponse ConsultarValorComercialPeso([FromUri]int peso)
        {

            return ApiSolicitudRecogidas.Instancia.ConsultarValorComercialPeso(peso);

        }

        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConsultarServiciosPesosMinimoxMaximos")]
        [SeguridadWebApi]
        public List<ServicioPesoResponse> ConsultarServiciosPesosMinimoxMaximos()
        {
            return ApiSolicitudRecogidas.Instancia.ConsultarServiciosPesosMinimoxMaximos();
        }


        /// <summary>
        /// Obtiene la imagen de una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        [HttpGet]
        [Route("ObtenerImagenGuia")]
        [SeguridadWebApi]
        public string ObtenerImagenGuia(long numeroGuia)
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerImagenGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene los tipos de envio para la app cliente de recogidas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTiposEnvio")]
        [SeguridadWebApi]
        public List<TipoEnvioApp> ObtenerTiposEnvio()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerTiposEnvio();
        }

        #endregion



        /// <summary>
        /// Guarda una solicitud de recogida de peaton
        /// </summary>
        /// <param name="recogida"></param>
        [HttpPost]
        [Route("GuardarSolicitudRecogidaPeaton")]
        [SeguridadWebApi]
        public long GuardarSolicitudRecogidaPeaton([FromBody]SolicitudRecogidaRequest recogida)
        {
            return ApiSolicitudRecogidas.Instancia.GuardarSolicitudRecogidaPeaton(recogida);
        }

        
        [HttpPost]
        [Route("ActualizaSolicitudClientePeaton")]
        [SeguridadWebApi]
        public void ActualizaSolicitudClientePeaton([FromBody]ActualizarRecogidaRequest recogida)
        {
            ApiSolicitudRecogidas.Instancia.ActualizaSolicitudClientePeaton(recogida);
        }

        /// <summary>
        /// consulta direcciones historico recogidas por documento
        /// </summary>
        /// <param name="SolicitudRecogidaPeaton"></param>
        [HttpPost]
        [Route("ObtenerDireccionesPeaton")]
        [SeguridadWebApi]
        public List<SolicitudRecogidaRequest> ObtenerDireccionesPeaton([FromBody]SolicitudRecogidaPeaton Peaton)
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerDireccionesPeaton(Peaton);
        }


        [HttpPost]
        [Route("CalificarSolicitudRecogida")]
        [SeguridadWebApi]
        public void CalificarSolicitudRecogida([FromBody] long idSolicitudRecogida, [FromBody] int calificacion, [FromBody] string observaciones)
        {
            ApiSolicitudRecogidas.Instancia.CalificarSolicitudRecogida(idSolicitudRecogida, calificacion, observaciones);
        }

        /// <summary>
        /// Obtener ultima direccion cliente externo aplicacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerInformacionRecogidaUsuarioExterno")]
        [SeguridadWebApi]
        public SolicitudRecogidaResponse ObtenerInformacionRecogidaUsuarioExterno([FromUri]string usuario)
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerInformacionRecogidaUsuarioExterno(usuario);
        }

        //PRUEBA RUTAS
        #region RutasCWeb

        /// <summary>
        /// Obtiene información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRuta")]
        [SeguridadWebApi]
        public List<RURutaICWeb> ObtenerRuta()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerRuta();
        }
        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("obtenerRutaDetalleCentroServiciosRuta")]
        [SeguridadWebApi]
        public List<RURutaCWebDetalleCentrosServicios> obtenerRutaDetalleCentroServiciosRuta([FromUri]int idruta, [FromUri]int id)
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerRutaDetalleCentroServiciosRuta(idruta, id);
        }

        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        [HttpPost]
        [Route("eliminarPtoRuta")]
        [SeguridadWebApi]
        public void eliminarPtoRuta([FromBody]PtoRuta datosPunto)
        {
            ApiSolicitudRecogidas.Instancia.EliminarPtoRuta(datosPunto);
        }
        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        [HttpPost]
        [Route("AgregarPtoRuta")]
        [SeguridadWebApi]
        public void AgregarPtoRuta([FromBody]PtoRuta datosPunto)
        {
            ApiSolicitudRecogidas.Instancia.AgregarPtoRuta(datosPunto);
        }
        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        [HttpPost]
        [Route("CrearPunto")]
        [SeguridadWebApi]
        public void CrearPunto([FromBody]PtoRuta datosPunto)
        {
            ApiSolicitudRecogidas.Instancia.CrearPunto(datosPunto);
        }
        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        [HttpPost]
        [Route("OrganizarPtos")]
        [SeguridadWebApi]
        public void OrganizarPtos([FromBody]PtoRuta datosPunto)
        {
            ApiSolicitudRecogidas.Instancia.OrganizarPtos(datosPunto);
        }
        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMediosTransporte")]
        [SeguridadWebApi]
        public List<RUMedioTransporte> ObtenerMediosTransporte()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerMediosTransporte();
        }
        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTiposVehiculos")]
        [SeguridadWebApi]
        public List<RUTipoVehiculo> ObtenerTiposVehiculos()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerTiposVehiculos();
        }
        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTiposRuta")]
        [SeguridadWebApi]
        public List<RUTipoRuta> ObtenerTiposRuta()
        {
            return ApiSolicitudRecogidas.Instancia.ObtenerTiposRuta();
        }
        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CrearRuta")]
        [SeguridadWebApi]
        public int CrearRuta(RURutaICWeb ruta)
        {
            return ApiSolicitudRecogidas.Instancia.CrearRuta(ruta);
        }
        #endregion
    }
}