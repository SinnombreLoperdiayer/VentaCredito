using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    /// <summary>
    /// Clase que contiene la informacion de los medios de transporte
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PAMedioTransporte : DataContractBase
    {
        [DataMember]
        public int IdMedioTransporte { get; set; }

        [DataMember]
        public string NombreMedioTransporte { get; set; }

        /// <summary>
        /// Muestra en el Checkbok si esta Seleccionado
        /// </summary>
        [DataMember]
        public bool Asignado { get; set; }
    }
}