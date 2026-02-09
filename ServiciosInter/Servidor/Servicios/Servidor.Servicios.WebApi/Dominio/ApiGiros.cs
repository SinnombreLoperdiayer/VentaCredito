using CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.ExploradorGiros;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiGiros : ApiDominioBase
    {
        private static readonly ApiGiros instancia = (ApiGiros)FabricaInterceptorApi.GetProxy(new ApiGiros(), COConstantesModulos.GIROS);  

        public static ApiGiros Instancia
        {
            get { return ApiGiros.instancia; }
        }

        private ApiGiros()
        {
        }


        /// <summary>
        /// Obtiene la informacion de un giro a partir de un número de giro
        /// </summary>
        /// <param name="numeroGiro"></param>
        /// <param name="idRemitente"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public GIExploradorGirosWebDC ObtenerDatosGiros([FromBody]GIExploradorGirosWebDC informaciónGiro)
        {
            return GIAdministradorExploradorGiros.Instancia.ObtenerDatosGiros(informaciónGiro);
        }

        /// <summary>
        /// Obtiene la informacion del estado de un giro a partir de un número de giro
        /// </summary>
        /// <param name="idAdminGiro"></param>        
        /// <returns></returns>
        public IList<GIEstadosGirosDC> ObtenerEstadosGiro([FromUri]long idAdminGiro)
        {
            IList<GIEstadosGirosDC> retorno = FabricaServicios.ServicioGiros.ObtenerEstadosGiro(idAdminGiro);
            return retorno;
        }

    }
}