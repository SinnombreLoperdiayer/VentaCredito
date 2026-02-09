using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa uno de los posibles caminos que se pueden tomar para llegar de un origen a un destino
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class Camino : DataContractBase
    {
        /// <summary>
        /// Listado de aristas que constituyen el camino con su respectivo costo
        /// </summary>
        [DataMember]
        public Dictionary<int, CostoArista> CostosAristas
        {
            get;
            set;
        }
    }
}