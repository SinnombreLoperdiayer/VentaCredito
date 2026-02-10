using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiIntegracionesRaps : ApiDominioBase
    {
        #region Singleton
        public static readonly ApiIntegracionesRaps instancia=(ApiIntegracionesRaps)FabricaInterceptorApi.GetProxy(new ApiIntegracionesRaps(), COConstantesModulos.MODULO_RAPS);

        public static ApiIntegracionesRaps Instancia
        {
            get { return ApiIntegracionesRaps.instancia; }
        }

        #endregion

        #region Constructor

        private ApiIntegracionesRaps(){}
        #endregion

        #region IntegracionesRapsManuales
       
        /// <summary>
        /// obtiene resposables con novedad  por id origen
        /// </summary>
        /// <param name="idOrigen"></param>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerResponsableTipoNovedad(int idOrigen)
        {
            return FabricaServicios.ServicioIntegracionesRaps.ObtenerResponsableTipoNovedad(idOrigen);
        }

        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        /// 
        public List<LIParametrizacionIntegracionRAPSDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            return FabricaServicios.ServicioIntegracionesRaps.ListaParametrosPersonalizacionPorNovedad(idTipoNovedad);
        }

        /// <summary>
        /// obtener parametros por id  novedad padre
        /// </summary>
        /// <param name="idResponsable"></param>
        /// <param name="idNovedadPadre"></param>
        /// <returns></returns>
        public List<RAParametrosPersonalizacionRapsDC> ObtenerParametrosVisiblesGlobales(int idNovedadPadre, int idOrigen)
        {
            return FabricaServicios.ServicioIntegracionesRaps.ObtenerParametrosVisiblesGlobales(idNovedadPadre, idOrigen);
        }

        #endregion

    }
}