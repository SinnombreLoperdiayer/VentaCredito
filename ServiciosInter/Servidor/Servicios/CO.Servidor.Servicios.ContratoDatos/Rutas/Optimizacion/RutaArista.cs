using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa una ruta creada en el sistema
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class Ruta : DataContractBase
    {
        /// <summary>
        /// Identificador único de la ruta
        /// </summary>
        [DataMember]
        public int IdRuta { get; set; }

        /// <summary>
        /// Descrición de la ruta
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }

        /// <summary>
        /// Tipo de vehículo que se moviliza por la ruta
        /// </summary>
        [DataMember]
        public string TipoVehiculo { get; set; }

        /// <summary>
        /// Medio de transporte que se utiliza en la ruta
        /// </summary>
        [DataMember]
        public string MedioTransporte { get; set; }
    }
}