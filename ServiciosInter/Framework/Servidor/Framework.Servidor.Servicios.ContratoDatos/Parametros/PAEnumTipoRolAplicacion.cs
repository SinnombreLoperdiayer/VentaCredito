using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    public enum PAEnumTipoRolAplicacion : int
    {
        [EnumMember]
        MENSAJERO = 10,

        [EnumMember]
        AUDITOR = 11,

        [EnumMember]
        AGENCIA = 12,

        [EnumMember]
        MENSAJERO_PAM = 13
    }
}
