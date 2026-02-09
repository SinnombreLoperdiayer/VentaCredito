using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  /// <summary>
  /// Enumeracion para establecer las acciones de un control
  /// </summary>
  //[Flags]
  [DataContract(Namespace = "http://contrologis.com")]
  public enum SEEnumAcciones
  {
    /// <summary>
    /// Ninguna acción permitida
    /// </summary>
    [EnumMember]
    NINGUNA = 0x1,

    /// <summary>
    /// Permite consultar
    /// </summary>
    [EnumMember]
    OBTENER = 0x2,

    /// <summary>
    /// Permite adicionar
    /// </summary>
    [EnumMember]
    ADICIONAR = 0x4,

    /// <summary>
    /// Permite eliminar
    /// </summary>
    [EnumMember]
    ELIMINAR = 0x6,

    /// <summary>
    /// Permite Editar
    /// </summary>
    [EnumMember]
    EDITAR = 0x8,

    /// <summary>
    /// Permite Imprimir
    /// </summary>
    [EnumMember]
    IMPRIMIR = 0x10,

    /// <summary>
    /// Permite acciones del
    /// Coordinador Financiero
    /// </summary>
    [EnumMember]
    CASAMATRIZ = 0x12,

    /// <summary>
    /// Permite Acciones del
    /// Operador Financiero
    /// </summary>
    [EnumMember]
    OPE_RACOL = 0x14
  }
}