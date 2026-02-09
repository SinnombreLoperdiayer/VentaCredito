using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiSolicitudRaps :ApiDominioBase
    {
        private static readonly ApiSolicitudRaps instacia = (ApiSolicitudRaps)FabricaInterceptorApi.GetProxy(new ApiSolicitudRaps(), COConstantesModulos.MODULO_RAPS);

        public static ApiSolicitudRaps Instancia
        {
            get { return ApiSolicitudRaps.instacia; }

        }

        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativaPersonalizada(RegistroSolicitudAppDC regSolicitud)
        {

            return FabricaServicios.ServicioSolicitudRaps.CrearSolicitudAcumulativaPersonalizada(regSolicitud);


        }

        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        internal bool CrearSolicitudAcumulativa(RegistroSolicitudAppDC regSolicitud)
        {
            return FabricaServicios.ServicioSolicitudRaps.CrearSolicitudAcumulativa(regSolicitud);
        }

        /// <summary>
        /// Crea una solicitud acumulativa para cuando se conoce el responsable
        /// </summary>
        /// <param name="regSolicitud"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativaManualesConoceResponsable(RegistroSolicitudAppDC registroSolicitud)
        {
            return FabricaServicios.ServicioSolicitudRaps.CrearSolicitudAcumulativaManualesConoceResponsable(registroSolicitud);
        }
    }
}