using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion
{
    /// <summary>
    /// Representa el costo en tiempo de una arista
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CostoArista : DataContractBase
    {
        /// <summary>
        /// Fecha de llegada estimada al destino de la arista
        /// </summary>
        [DataMember]
        public DateTime FechaLlegadaDestino
        {
            get;
            set;
        }

        /// <summary>
        /// Fecha de salida estimada del origen de la arista
        /// </summary>
        [DataMember]
        public DateTime FechaSalidaOrigen
        {
            get;
            set;
        }

        /// <summary>
        /// Número de horas que el envio debe esperar en la estación origen de la arista antes
        /// de ser enviado a la estación destino
        /// </summary>
        [DataMember]
        public decimal HorasEnEstacionOrigen
        {
            get;
            set;
        }

        /// <summary>
        /// Número de horas que el envío se demorará en llegar al destino de la arista desde que se embarca en el origen
        /// </summary>
        [DataMember]
        public decimal HorasEnTransito
        {
            get;
            set;
        }

        /// <summary>
        /// Datos de la arista
        /// </summary>
        [DataMember]
        public Arista Arista
        {
            get;
            set;
        }
    }
}