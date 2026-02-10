using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum ADEnumTipoCliente
  {
    /// <summary>
    /// Peatón - Convenio
    /// </summary>
    [EnumMember]
    PCO,
    /// <summary>
    /// Peatón - Peatón
    /// </summary>
    [EnumMember]
    PPE,
    /// <summary>
    /// Convenio - Peatón
    /// </summary>
    [EnumMember]
    CPE,
    /// <summary>
    /// Convenio - Convenio
    /// </summary>
    [EnumMember]
    CCO,
    /// <summary>
    /// Interno
    /// </summary>
    [EnumMember]
    INT,
    /// <summary>
    /// Credito
    /// </summary>
    [EnumMember]
    CRE,
    /// <summary>
    /// Peaton
    /// </summary>
    [EnumMember]
    PEA
  }
}