using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiCentroAcopio : ApiDominioBase
    {
        private static readonly ApiCentroAcopio instancia = (ApiCentroAcopio)FabricaInterceptorApi.GetProxy(new ApiCentroAcopio(), COConstantesModulos.CENTROACOPIO);

        public static ApiCentroAcopio Instancia
        {
            get { return instancia; }
        }

        private ApiCentroAcopio()
        {
        }

        public MovimientoConsolidado MovimientoConsolidadoVigente(string numeroConsolidado, CACEnumTipoConsolidado tipoConsolidado)
        {            
            return FabricaServicios.ServicioCentroAcopio.MovimientoConsolidadoVigente(numeroConsolidado, tipoConsolidado);
        }

        public List<TipoConsolidado> ObtenerTipoConsolidado()
        {
            return FabricaServicios.ServicioCentroAcopio.ObtenerTipoConsolidado();
        }

        public void InsertarMovimientoConsolidado(MovimientoConsolidado movimientoConsolidado)
        {
            FabricaServicios.ServicioCentroAcopio.InsertarMovimientoConsolidado(movimientoConsolidado);
        }   
    }
}