using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa una arista de dos vertices adyancentes
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class Arista : DataContractBase
    {
        /// <summary>
        /// Vertice origen de la arista
        /// </summary>
        [DataMember]
        public Vertice VerticeOrigen
        {
            get;
            set;
        }

        /// <summary>
        /// Vertice destino de la arista
        /// </summary>
        [DataMember]
        public Vertice VerticeDestino
        {
            get;
            set;
        }

        /// <summary>
        /// Ruta a la que está asociada la arista
        /// </summary>
        [DataMember]
        public Ruta RutaArista
        {
            get;
            set;
        }

        /// <summary>
        /// Frecuencias que maneja la arista
        /// </summary>
        [DataMember]
        public List<Frecuencia> Frecuencias
        {
            get;
            set;
        }

        /// <summary>
        /// Tiempo que debe esperar el envío en el vértice origen antes de ser despachado para
        /// el vértice destino
        /// </summary>
        [DataMember]
        public decimal TiempoParadaEstacionorigen { get; set; }
    }
}