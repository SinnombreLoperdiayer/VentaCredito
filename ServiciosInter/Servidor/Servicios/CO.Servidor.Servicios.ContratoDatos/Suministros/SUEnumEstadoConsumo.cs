using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum SUEnumEstadoConsumo
    {
        [EnumMember]
        CON,

        [EnumMember]
        ANU
    }
}