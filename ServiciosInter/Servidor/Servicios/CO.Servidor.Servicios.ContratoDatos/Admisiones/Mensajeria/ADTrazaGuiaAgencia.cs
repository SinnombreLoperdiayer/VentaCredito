using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADTrazaGuiaAgencia : DataContractBase
    {
        /// <summary>
        /// Retorna o asigna el número de guía
        /// </summary>
        [DataMember]        
        public long? NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guía
        /// </summary>
        [DataMember]
        public short? IdEstadoGuia { get; set; }

        /// <summary>
        /// Retorna o asigna el id de admisión mensajería
        /// </summary>
        [DataMember]
        public long? IdAdmisionMensajeria { get; set; }


        /// <summary>
        /// Fecha de grabación del registro
        /// </summary>
        [DataMember]
        public DateTime FechaGrabacionEstado { get; set; }

        /// <summary>
        /// retorna o asigna el id de la ciudad del centro logistico
        /// </summary>
        [DataMember]        
        public string IdCiudadDestino { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad del centro logistico
        /// </summary>
        [DataMember]        
        public string CiudadDestino { get; set; }

        [DataMember]
        public string Observaciones { get; set; }
    }
}
