using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    /// <summary>
    /// Clase que contiene la informacion de los horarios de los centros de servicios
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUHorariosCentroServicios : DataContractBase
    {
        [DataMember]
        public string IdLocalidad { get; set; }

        [DataMember]
        public int IdHorarioCentroServicios { get; set; }

        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public string IdDia { get; set; }

        [DataMember]
        public string NombreDia { get; set; }

        [DataMember]
        public DateTime HoraInicio { get; set; }

        [DataMember]
        public DateTime HoraFin { get; set; }
    }
}