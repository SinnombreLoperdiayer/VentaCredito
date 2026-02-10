using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum EnumActorMotivoSolRecogida
    {
        [DataMember]
        None = 0,
        [DataMember]
        Mensajero = 1,
        [DataMember]
        Administrador = 2,
        [DataMember]
        ClientePeaton = 3,
        [DataMember]
        ClienteCredito = 4,
    }
}
