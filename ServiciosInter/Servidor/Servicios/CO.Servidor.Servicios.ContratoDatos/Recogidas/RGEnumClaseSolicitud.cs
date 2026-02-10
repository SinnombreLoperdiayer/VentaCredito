using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RGEnumClaseSolicitud
    {
        [DataMember]   
        SinSolicitud = 0,
        [DataMember]   
        FijaCliente = 1,
        [DataMember]   
        ExporadicaClienteFijo = 2,
        [DataMember]   
        FijaCentroServicio = 3,
        [DataMember]
        Exporadica = 4
    }
}
