using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum ADEnumResultadoEscaner : short
    {
        [EnumMember]
        SinAsignar = 0,
        [EnumMember]
        Exitosa = 1,
        [EnumMember]
        NoAdmitida = 2,
        [EnumMember]
        EstadoInvalido = 3,
        [EnumMember]
        Duplicada = 4,
        [EnumMember]
        NoIdentificada = 5,
    }
}