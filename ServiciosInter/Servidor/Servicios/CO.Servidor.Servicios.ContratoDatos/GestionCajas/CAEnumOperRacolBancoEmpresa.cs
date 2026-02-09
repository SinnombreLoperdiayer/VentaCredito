using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Enumerador que contiene las combinaciones de las operaciones del centro de servicios con una caja de gestión (Casa Matriz, Banco, Operaación Nacional)
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEnumOperCajaCentroServicosGestion
  {
    /// <summary>
    /// Operaciones entre Centro Servicios la Caja Banco
    /// </summary>
    [EnumMember]
    TransBancoCentroServicio = 0,

    /// <summary>
    /// Operacion entre Centros de Servicio
    /// </summary>
    [EnumMember]
    TransCentroSvcCentroSvc,

    /// <summary>
    /// Operacion Empresa ó Casa Matriz y Banco
    /// </summary>
    [EnumMember]
    TransCasaMatrizBanco,

    /// <summary>
    /// Operacion Casa Matriz y Operación Nacional
    /// </summary>
    [EnumMember]
    TranCasaMatrizOperacioNacional,

    /// <summary>
    /// Operacion Banco y Operación Nacional
    /// </summary>
    [EnumMember]
    TranBancoOperacioNacional,

    /// <summary>
    /// Operacion RACOL y Operación Nacional
    /// </summary>
    [EnumMember]
    TranRacolOperacioNacional
  }
}