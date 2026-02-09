using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum ONEnumGuiaIndvNovedadRuta
    {
        [EnumMember]
        EstacionRuta = 0,
        [EnumMember]
        Mensajero = 1,
        [EnumMember]
        ClienteCredito = 2
    }
}
