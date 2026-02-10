using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum COEnumIdentificadorAplicacion
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
        [DataMember]
        IVR = 6,
        [DataMember]
        APP = 7,
        [DataMember]
        INTRANET = 8,
        [DataMember]
        CONTROLLERAPP = 9,
        [DataMember]
        MotorRecogidas = 10,
        [DataMember]
        AdministradorRecogidas = 11,
    }
}
