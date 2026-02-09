using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
     [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumTipoTapaLogisticaDC : short
    {
        [Description(" ")]
        [EnumMember]
        None = 0,

        [Description("AUDITORIA")]
        [EnumMember]
        Auditoria = 1,

        [Description("RECLAME OFICINA")]
        [EnumMember]
        ReclameOficina = 2,

        [Description("NUEVA DIRECCION")]
        [EnumMember]
        NuevaDireccion = 3,

        [Description("CUSTODIA")]
        [EnumMember]
        Custodia = 4,

        [Description("GESTION AUDITOR")]
        [EnumMember]
        GestionAuditor = 5

    }
}
