using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum CCEnumTipoNovedadGuia : short
    {
        [EnumMember]
        SinValor = 30,
        [EnumMember]
        ModificarDestinoGuia = 1,
        [EnumMember]
        ModificarFormaPagoGuia = 2,
        [EnumMember]
        ModificarTipoServicioGuia = 3,
        [EnumMember]
        AnulacionGuia = 4,
        [EnumMember]
        ModificarRemDest = 5,
        [EnumMember]
        ModificarValorTotalGuia = 6,
        [EnumMember]
        ModificarPesoGuia = 7,
        [EnumMember]
        ModificarValorTransporte = 8,
        [EnumMember]
        ModificarValorPrima = 9,
        [EnumMember]
        ModificarValorDeclarao = 10,
    }
}