using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModeloResponse.SolicitudRecogidasApp;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Servicios.Implementacion.Rutas;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiRutasCWeb : ApiDominioBase
    {
        private static readonly ApiRutasCWeb instancia = (ApiRutasCWeb)FabricaInterceptorApi.GetProxy(new ApiRutasCWeb(), COConstantesModulos.RUTASCWEB);
        #region Constructor

        public static ApiRutasCWeb Instancia
        {
            get { return ApiRutasCWeb.instancia; }
        }

        private ApiRutasCWeb()
        {

        }

        #endregion

        #region Metodos

        /// <summary>1
        /// Obtiene información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <returns></returns>
        /// 
        private static readonly RURutasSvc rutasCWeb = new RURutasSvc();

        public List<RURutaICWeb> obtenerRuta()
        {
            return FabricaServicios.RutasCWeb.obtenerRuta();
        }
        public List<RURutaCWebDetalleCentrosServicios> obtenerRutaDetalleCentroServiciosRuta(int idRuta)
        {
            return FabricaServicios.RutasCWeb.obtenerRutaDetalleCentroServiciosRuta(idRuta);
        }

        //public RURutaICWebDetalle ObtenerRutaDetalle(int idRuta)
        //{
        //    return FabricaServicios.RutasCWeb.obtenerRutaDetalleCentroServiciosRuta(idRuta);
        //}

        #endregion
    }
}