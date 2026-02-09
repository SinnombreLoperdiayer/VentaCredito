using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class RGRecogidaEsporadicaDC : DataContractBase
    {
        [DataMember]
        public long? IdSolRecogida { get; set; }

        [DataMember]
        public DateTime FechaHoraRecogida { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string DireccionRecogida { get; set; }

        [DataMember]
        public string DescripcionEstado { get; set; }

        [DataMember]
        public string Mensajero { get; set; }

        [DataMember]
        public string IdLocalidad { get; set; }


        [DataMember]
        public string Longitud { get; set; }

        [DataMember]
        public string Latitud { get; set; }
    }
}
