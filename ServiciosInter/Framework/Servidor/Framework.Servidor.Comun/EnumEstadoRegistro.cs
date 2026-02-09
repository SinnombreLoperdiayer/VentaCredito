using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Comun
{
  ///Enumeracion que indica el estado de un registro
  [Flags]
  [DataContract(Namespace = "http://contrologis.com")]
  public enum EnumEstadoRegistro
  {
    /// <summary>
    /// Sin cambios en el registro
    /// </summary>
    [EnumMember]
    SIN_CAMBIOS = 0x1,

    /// <summary>
    /// Registro adicionado
    /// </summary>
    [EnumMember]
    ADICIONADO = 0x2,

    /// <summary>
    /// Registro modificado
    /// </summary>
    [EnumMember]
    MODIFICADO = 0x4,

    /// <summary>
    /// Registro borrado
    /// </summary>
    [EnumMember]
    BORRADO = 0x8
  }
}