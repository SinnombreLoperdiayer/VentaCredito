using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{

    [DataContract(Namespace = "http://contrologis.com")]
    public enum EnumTipoArchivo
    {
        [EnumMember]
        ENUM_SIN_VALOR = 0,
        [EnumMember]
        FORMATO_GUIA_MENSAJERIA = 1,
        [EnumMember]
        FORMATO_INTENTO_ENTREGA = 2
            
    }
}
