using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumTipoEvidenciaDC : int
    {
        /********************* TIPO EVIDENCIA CONTROLLER APP ************************/
        [EnumMember]
        DESCARGUE_ENTREGA_MENSAJERO = 1,
        [EnumMember]
        DEVOLUCION_MENSAJERO = 2,
        [EnumMember]
        DESCARGUE_ENTREGA_AUDITOR = 3,
        [EnumMember]
        DEVOLUCION_RATIFICADA_AUDITOR = 4,
        [EnumMember]
        DESCARGUE_ENTREGA_MAESTRA_AUDITOR = 5
    }
}
