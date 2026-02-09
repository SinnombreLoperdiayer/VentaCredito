using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumPendientesAgencia : short
    {
        [EnumMember]
        Pendientes = 0,
        [EnumMember]
        Entregar = 1,
        [EnumMember]
        Vencido = 2,
        [EnumMember]
        Telemercadeo = 3,
        [EnumMember]
        Reenvio = 4,
        [EnumMember]
        Devolucion = 5,
        [EnumMember]
        Admision = 6

    }
}
