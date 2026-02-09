using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract()]
    public enum EnumOrigenGuia
    {
        NONE = 0,
        SilverLight = 1,
        POS = 2,
        WEB = 3,
        PAM = 4,
        MASIVOS = 5,
        IVR = 6,
        APP = 7,
        INTRANET = 8,
        CONTROLLER_APP = 9,
        rRecogidas = 10,
        AministradorRecogidas = 11
    }
}
