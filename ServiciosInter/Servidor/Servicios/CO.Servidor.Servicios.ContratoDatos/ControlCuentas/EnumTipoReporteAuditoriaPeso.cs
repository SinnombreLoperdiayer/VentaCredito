using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{

    [DataContract(Namespace = "http://contrologis.com")]
    public enum EnumTipoReporteAuditoriaPeso : short
    {
        [EnumMember]
        DescuentoMensajero = 1,
        [EnumMember]
        NotaCredito = 2,
        [EnumMember]
        AuditoriasDiarias = 3,
        [EnumMember]
        EnvioEstados= 4,
        [EnumMember]
        ErrorAdmisionFactMan = 5,
        [EnumMember]
        ErrorEdVentaFactMan = 6,
        [EnumMember]
        ValorMayorOMenor = 7,
        [EnumMember]
        SinProsupuesto = 8
    }
}
