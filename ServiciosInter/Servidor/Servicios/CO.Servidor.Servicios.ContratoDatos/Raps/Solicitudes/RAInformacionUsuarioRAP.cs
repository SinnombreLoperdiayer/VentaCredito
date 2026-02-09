using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAInformacionUsuarioRAP
    {
        [DataMember]
        public long Identificacion { get; set; }

        [DataMember]
        public string IdCargo { get; set; }

        [DataMember]
        public string Correo { get; set; }

    }
}
