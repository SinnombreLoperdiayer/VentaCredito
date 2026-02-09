using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum SEEnumSistemaInformacion : short
    {
        [EnumMember]
        Silverlight = 1,
        [EnumMember]
        WPFFinanciera = 2
    }
}
