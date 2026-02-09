using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEnumConceptoCajaEmpBanRacol
  {
    /// <summary>
    /// Concepto de Caja de Consignación sobre Bancos, es un Ingreso (id = 24)
    /// </summary>
    [EnumMember]
    CONSIGNACION = 24,

    /// <summary>
    /// Concepto de Caja para Retiro desde Banco, es un Egreso (i d = 25)
    /// </summary>
    [EnumMember]
    RETIRO = 25,

    /// <summary>
    /// Concepto de caja de Abastecimiento (id = 26)
    /// es un Ingreso
    /// </summary>
    [EnumMember]
    ABASTECIMIENTO = 26,

    /// <summary>
    /// Concepto de reporte dinero punto Servcio (id = 27)
    /// </summary>
    [EnumMember]
    REPORTE_DINERO_PUNTO = 27,

    /// <summary>
    /// Concepto de traslado de dinero de Operación Nacional a RACOL (id = 32)
    /// </summary>
    [EnumMember]
    TRASLADO_OPN_A_RACOL = 32,

    /// <summary>
    /// Concepto de traslado de dinero de Operación Nacional a Casa Matriz (id = 33)
    /// </summary>
    [EnumMember]
    TRASLADO_OPN_A_CM = 33
  }
}