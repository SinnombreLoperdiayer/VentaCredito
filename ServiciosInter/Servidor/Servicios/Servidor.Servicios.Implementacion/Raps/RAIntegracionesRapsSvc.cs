using CO.Servidor.Raps;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace CO.Servidor.Servicios.Implementacion.Raps
{
    /// <summary>
    /// Clase para los servicios de integración Raps
    /// </summary>
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]

    public class RAIntegracionesRapsSvc
    {
        #region IntegracionesRapsManuales

        /// <summary>
        /// obtiene resposables con novedad  por id origen
        /// </summary>
        /// <param name="idOrigen"></param>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerResponsableTipoNovedad(int idOrigen)
        {
            return RAIntegracionRaps.Instancia.ObtenerResponsableTipoNovedad(idOrigen);
        }
        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            return RAIntegracionRaps.Instancia.ListaParametrosPersonalizacionPorNovedad(idTipoNovedad);
        }

        /// <summary>
        /// obtener parametros por id  novedad padre
        /// </summary>
        /// <param name="idResponsable"></param>
        /// <param name="idNovedadPadre"></param>
        /// <returns></returns>
        public List<RAParametrosPersonalizacionRapsDC> ObtenerParametrosVisiblesGlobales(int idNovedadPadre, int idOrigen)
        {
            return RAIntegracionRaps.Instancia.ObtenerParametrosVisiblesGlobales(idNovedadPadre,idOrigen);
        }

        #endregion

    }
}
