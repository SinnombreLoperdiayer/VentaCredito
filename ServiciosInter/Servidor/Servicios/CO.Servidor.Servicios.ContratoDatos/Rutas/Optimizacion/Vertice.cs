using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa un vértice de un grafo
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class Vertice : DataContractBase
    {
        /// <summary>
        /// Identificador único del vértice.
        /// </summary>
        [DataMember]
        public string IdVertice
        {
            get;
            set;
        }

        /// <summary>
        /// Descripción del vertice.
        /// </summary>
        [DataMember]
        public string Descripcion
        {
            get;
            set;
        }
    }
}