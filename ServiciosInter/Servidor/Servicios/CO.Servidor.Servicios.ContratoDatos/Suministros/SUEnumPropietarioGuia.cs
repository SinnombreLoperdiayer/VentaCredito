using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract]
  public enum SUEnumPropietarioGuia
  {
    [EnumMember]
    CentroServicio,

    [EnumMember]
    Mensajero,

    [EnumMember]
    Sucursal,

    [EnumMember]
    Proceso,

    [EnumMember]
    NoHayPropietario
  }
}