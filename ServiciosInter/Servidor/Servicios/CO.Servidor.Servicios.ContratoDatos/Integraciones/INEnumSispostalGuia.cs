using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    public enum INEnumSispostalGuia
    {
        [EnumMember]
        SINSALIR = 0,

        [EnumMember]
        ENZONA = 1,

        [EnumMember]
        DEVUELTO = 2,

        [EnumMember]
        ENTREGADO = 3,

        [EnumMember]
        TRANSITO = 4,

        [EnumMember]
        GUIASINFISICO = 5,

        [EnumMember]
        DEVOLUCIONINICIAL = 6,

        [EnumMember]
        ALISTAMIENTO = 7,

        [EnumMember]
        TELEMERCADEO = 8,

        [EnumMember]
        NONEGOCIACION = 9,

        [EnumMember]
        ANULADA = 10,

        [EnumMember]
        SINIESTRO = 11,

        [EnumMember]
        RETENCION = 12,

        
    }
}
