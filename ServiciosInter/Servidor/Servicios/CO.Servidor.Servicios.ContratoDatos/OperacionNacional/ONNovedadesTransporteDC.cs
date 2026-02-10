using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Novedad de Transporte presentadas 
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONNovedadesTransporteDC : DataContractBase
    {
        /// <summary>
        /// Es el id del Manifiesto Operacion Nacional  (Autonumerico)
        /// </summary>
        [DataMember]
        public long IdManifiestoOperacionNacio { get; set; }
        
        /// <summary>
        /// Tipo de Novedad
        /// </summary>
        [DataMember]        
        public string NombreNovedad { get; set; }

        [DataMember]
        public string LugarIncidente { get; set; }

        /// <summary>
        /// Observaciones
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }

        /// <summary>
        /// Fecha y hora del Incidente
        /// </summary>
        [DataMember]
        public DateTime FechaNovedad { get; set; }

        /// <summary>
        /// Tiempo de duración
        /// </summary>
        [DataMember]
        public string Tiempo { get; set; }

        /// <summary>
        /// Fecha Estimada de Entrega
        /// </summary>
        [DataMember]
        public DateTime FechaEstimadaEntrega { get; set; }
    }
}
