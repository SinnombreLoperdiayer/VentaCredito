using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "http://controllogis.com")]
    public class HorarioMensajesTextoDC : DataContractBase
    {
        [DataMember]
        public int IdDia { get; set; }

        [DataMember]
        public DateTime HoraInicio { get; set; }

        [DataMember]
        public DateTime HoraFin { get; set; }
    }
}
