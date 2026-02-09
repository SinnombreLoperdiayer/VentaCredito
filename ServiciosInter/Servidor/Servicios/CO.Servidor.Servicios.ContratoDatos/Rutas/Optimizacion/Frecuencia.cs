using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa una frecuencia de salida
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class Frecuencia : DataContractBase
    {
        /// <summary>
        /// Dia de salida
        /// </summary>
        [DataMember]
        public int Dia { get; set; }

        /// <summary>
        /// Hora de salida desde el origen
        /// </summary>
        [DataMember]
        public DateTime HoraSalidaOrigen { get; set; }

        /// <summary>
        /// Hora de llegada al destino
        /// </summary>
        [DataMember]
        public DateTime HoraLlegadaDestino { get; set; }
    }
}