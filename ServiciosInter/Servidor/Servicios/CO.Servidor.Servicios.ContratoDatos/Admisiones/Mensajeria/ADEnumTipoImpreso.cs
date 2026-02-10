using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum ADEnumTipoImpreso : short
  {
    [EnumMember]
    SinImpreso = 0,

    [EnumMember]
    Planilla = 1,

    [EnumMember]
    Manifiesto = 2,

    [EnumMember]
    PlanillaGuiaInterna = 3,

    [EnumMember]
    PlanillaFacturas = 4,

    [EnumMember]
    ActaDisposicionFinal = 5,
  }
}