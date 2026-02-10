using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  /// <summary>
  /// Contiene la información del responsable de una novedad
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CCTipoResponsableDC
  {
    [EnumMember]
    AGENCIA,
    [EnumMember]
    CLIENTE,
    [EnumMember]
    FUNCIONARIO
  }
}
