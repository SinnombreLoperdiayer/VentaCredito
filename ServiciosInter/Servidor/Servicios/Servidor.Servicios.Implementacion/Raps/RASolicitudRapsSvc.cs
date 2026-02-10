using CO.Servidor.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace CO.Servidor.Servicios.Implementacion.Raps
{
    /// <summary>
    /// Clase para los servicios de gestion de las solicitudes de Raps
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RASolicitudRapsSvc
    {
        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativa(RegistroSolicitudAppDC regSolicitud)
        {
            return RASolicitud.Instancia.CrearSolicitudAcumulativa(regSolicitud);
        }

        /// <summary>
        /// Crea una solicitud personalizada
        /// <summary>
        /// <param name="regSolicitud"></param>
        /// <returns></returns>

        public bool CrearSolicitudAcumulativaPersonalizada(RegistroSolicitudAppDC regSolicitud)
        {
            return RASolicitud.Instancia.CrearSolicitudAcumulativaPersonalizada(regSolicitud);
        }

        /// <summary>
        /// Crear solicitud acumulativa manuales
        /// </summary>
        /// <param name="registroSolicitud"></param>
        /// <returns></returns>
        public bool CrearSolicitudAcumulativaManualesConoceResponsable(RegistroSolicitudAppDC registroSolicitud)
        {
            return RASolicitud.Instancia.CrearSolicitudAcumulativaManualesConoceResponsable(registroSolicitud);
        }

    }
}
