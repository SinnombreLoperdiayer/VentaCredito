using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum CCEnumTipoNovedadCtrLiquidacionDC
    {
        [EnumMember]
        Peso = 1,
        [EnumMember]
        Ciudad_Destino = 2,
        [EnumMember]
        Servicio = 3,
        [EnumMember]
        Forma_Pago = 4,
        [EnumMember]
        Valor_Comercial = 5,
        [EnumMember]
        Valor_Total = 6,
        [EnumMember]
        Sin_Novedades = 7
    }
}
