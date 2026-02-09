using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
    /// <summary>
    /// Solicitud creadas desde el modulo de pagos de giros
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class GISolicitdPagoDC : DataContractBase
    {
        /// <summary>
        /// Id admision del giro a crear la solicitud
        /// </summary>
        [DataMember]
        public long IdAdminGiro { get; set; }

        /// <summary>
        /// Id del centro de servicios que crea la solicitu
        /// </summary>
        [DataMember]
        public long IdCentroSolicita { get; set; }

        /// <summary>
        /// Nombre del centro de servicio que  creacion de la solicitud
        /// </summary>
        [DataMember]
        public string NombreCentroSolicita { get; set; }

        /// <summary>
        /// Id del racol
        /// </summary>
        [DataMember]
        public long IdRacol { get; set; }

        /// <summary>
        /// Nombre del racol
        /// </summary>
        [DataMember]
        public string NombreRacol { get; set; }

        /// <summary>
        /// Es el numero del giro del cliente
        /// </summary>
        [DataMember]
        public long IdGiro { get; set; }

        /// <summary>
        /// Es el Centro de Servicio Inicial de la Solicitud
        /// </summary>
        [DataMember]
        public PUCentroServiciosDC CentroServicioInicial { get; set; }

        /// <summary>
        /// Es el Centro de Servicio que quiere pagar giro
        /// </summary>
        [DataMember]
        public PUCentroServiciosDC CentroServicioSolicita { get; set; }
    }
}