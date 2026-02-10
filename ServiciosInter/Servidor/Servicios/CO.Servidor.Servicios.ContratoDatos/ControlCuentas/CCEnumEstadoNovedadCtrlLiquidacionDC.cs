using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    public enum CCEnumEstadoNovedadCtrlLiquidacionDC
    {
        [EnumMember]
        CREADA = 1,
        [EnumMember]
        GESTIONADA = 2,
        [EnumMember]
        ANULADA = 3
    }
}
