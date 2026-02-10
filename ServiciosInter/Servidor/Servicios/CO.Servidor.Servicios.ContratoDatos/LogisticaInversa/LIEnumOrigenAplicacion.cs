using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum LIEnumOrigenAplicacion
    {
        [Description("No definido")]
        [EnumMember]
        None = 0,

        [Description("Origen SilverLight")]
        [EnumMember]
        SilverLight = 1,

        [Description("Origen POS")]
        [EnumMember]
        POS = 2,

        [Description("Origen WEB")]
        [EnumMember]
        WEB = 3,

        [Description("Origen PAM")]
        [EnumMember]
        PAM = 4,

        [Description("Origen Masivos")]
        [EnumMember]
        Masivos = 5,

        [Description("Origen IVR")]
        [EnumMember]
        IVR = 6,

        [Description("Origen App cliente peaton")]
        [EnumMember]
        APP_CLIENTE_PEATON = 7,

        [Description("Origen Intranet")]
        [EnumMember]
        INTRANET = 8,

        [Description("Origen Controller App")]
        [EnumMember]
        CONTROLLER_APP = 9

        



    }
}
