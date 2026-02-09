using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa la ruta óptima calculada desde un origen a un destino
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class RURutaOptimaCalculada : DataContractBase
    {
        /// <summary>
        /// Listado de todos los posibles caminos que se pueden tomar.
        /// </summary>
        [DataMember]
        public List<Camino> TodosLosCaminos { get; set; }

        /// <summary>
        /// Camino más corto encontrado
        /// </summary>
        [DataMember]
        public Camino CaminoMasCorto
        {
            get;
            set;
        }

        /// <summary>
        /// Camino más largo encontrado
        /// </summary>
        [DataMember]
        public Camino CaminoMasLargo
        {
            get;
            set;
        }

        /// <summary>
        /// Ciudad destino
        /// </summary>
        [DataMember]
        public Vertice Destino
        {
            get;
            set;
        }

        /// <summary>
        /// Ciudad Origen
        /// </summary>
        [DataMember]
        public Vertice Origen
        {
            get;
            set;
        }
    }
}