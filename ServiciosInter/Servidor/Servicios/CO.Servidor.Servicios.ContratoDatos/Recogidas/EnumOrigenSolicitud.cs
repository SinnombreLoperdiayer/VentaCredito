using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum EnumOrigenSolicitud: int
    {
        [DataMember]   
        NoIdentificado = 0,
        [DataMember]   
        SilverLight = 1,
        [DataMember]   
        POS = 2,
        [DataMember]   
        WEB = 3,
        [DataMember]   
        PAM = 4,
        [DataMember]
        MASIVOS = 5,
    }
}
