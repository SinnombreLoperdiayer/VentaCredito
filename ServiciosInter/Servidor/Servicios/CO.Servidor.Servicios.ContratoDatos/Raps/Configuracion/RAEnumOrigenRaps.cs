using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum RAEnumOrigenRaps
    {
        [EnumMember]
        Mensajero = 1,
        [EnumMember]
        Puntos = 2,
        [EnumMember]
        Agencias = 3,
        [EnumMember]
        NA = 4,
        [EnumMember]
        Centro_de_Acopio = 5,
        [EnumMember]
        Logistica_Inversa = 6,
        [EnumMember]
        Custodia = 7,
        [EnumMember]
        Cliente = 8
    }
}
