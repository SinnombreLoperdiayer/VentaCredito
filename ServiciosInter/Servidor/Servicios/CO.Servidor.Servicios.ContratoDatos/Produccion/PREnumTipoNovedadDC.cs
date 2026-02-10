using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{

    [DataContract(Namespace = "http://contrologis.com")]
    public enum PREnumTipoNovedadDC:short
    {
        [EnumMember]
        DESCUENTO = 1,
        [EnumMember]
        REINTEGRO = 2,
        [EnumMember]
        SERVICIO=3
    }
}
