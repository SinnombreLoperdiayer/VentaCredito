using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModeloResponse.SolicitudRecogidasApp;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiTarifas : ApiDominioBase
    {

        private static readonly ApiTarifas instancia = (ApiTarifas)FabricaInterceptorApi.GetProxy(new ApiTarifas(), COConstantesModulos.TARIFAS);

        #region Constructor

        public static ApiTarifas Instancia
        {
            get { return ApiTarifas.instancia; }
        }

        private ApiTarifas()
        {

        }

        #endregion

        #region Metodos

        /// <summary>1
        /// Obtiene los tipos de envio
        /// </summary>
        /// <returns></returns>
        public List<TATipoEnvio> ObtenerTipoEnvio()
        {
            return FabricaServicios.ServicioTarifas.ObtenerTipoEnvios();
        }


        public IList<PALocalidadDC> ObtenerCiudades()
        {
            return FabricaServicios.ServicioParametros.ObtenerLocalidadesNoPaisNoDepartamentoColombia().ToList();
        }


        public IList<TAServicioDC> ObtenerTipoServicios()
        {
            return FabricaServicios.ServicioTarifas.ObtenerServicios().ToList();
        }

        #endregion
    }
}
