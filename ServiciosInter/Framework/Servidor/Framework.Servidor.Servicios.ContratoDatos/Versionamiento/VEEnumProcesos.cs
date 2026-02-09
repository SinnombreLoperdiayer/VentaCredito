using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Versionamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public  enum VEEnumProcesos : int
    {
  
        [EnumMember]
        Admision  = 1,

        [EnumMember]
        Operacion = 2,

        [EnumMember]
        LogisticaInversa = 3,

        [EnumMember]
        Comercial = 4,

        [EnumMember]
        Configuracion = 5,

        [EnumMember]
        AdmyFinanciera = 6,

        [EnumMember]
        ServicioAlCliente = 7,

        [EnumMember]
        Agenda = 8,

    }
}
