using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum PUEnumTipoMovimientoInventario : short
    {
        [EnumMember]
        SinTipoMovimiento = 0,
        [EnumMember]
        Asignacion = 1,
        [EnumMember]
        Ingreso = 2,
        [EnumMember]
        Salida = 3
    }
}
