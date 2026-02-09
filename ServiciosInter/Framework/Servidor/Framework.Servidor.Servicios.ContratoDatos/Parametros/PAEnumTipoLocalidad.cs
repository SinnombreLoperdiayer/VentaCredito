using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Enumeracion de tipos de localidad
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum PAEnumTipoLocalidad : short
  {
    [EnumMember]
    PAIS = 1,

    [EnumMember]
    DEPARTAMENTO = 2,

    [EnumMember]
    MUNICIPIO = 3,

    [EnumMember]
    CORREGIMIENTO = 4,        
    [EnumMember]
    INSPECCION = 5,

    [EnumMember]
    CASERIO = 6
  }
}