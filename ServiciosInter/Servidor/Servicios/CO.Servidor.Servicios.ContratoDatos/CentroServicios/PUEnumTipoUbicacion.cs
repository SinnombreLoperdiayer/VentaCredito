using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum PUEnumTipoUbicacion : int
    {
        [EnumMember]
        SinAsignacion = 0,
        [EnumMember]
        Casillero = 1,
        [EnumMember]
        Estiba = 2,
    }
}
