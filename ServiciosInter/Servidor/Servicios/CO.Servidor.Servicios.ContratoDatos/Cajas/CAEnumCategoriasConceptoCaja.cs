using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum CAEnumCategoriasConceptoCaja
  {
    /// <summary>
    /// Categoria Mensajeria
    /// </summary>
    [EnumMember]
    VENTA_SERVICIOS_MENSAJERIA = 1,

    /// <summary>
    /// Categoria venta prepago,
    /// </summary>
    [EnumMember]
    VENTA_PREPAGO = 2,

    /// <summary>
    /// Categoria de Casa Matriz
    /// </summary>
    [EnumMember]
    MOVIMIENTO_CASA_MATRIZ = 3,

    /// <summary>
    /// Categoria de Bancos (id = 4)
    /// </summary>
    [EnumMember]
    MOVIMIENTO_BANCOS = 4,

    /// <summary>
    /// Categoria de RACOL (id = 5)
    /// </summary>
    [EnumMember]
    MOVIMIENTO_RACOL = 5,

    /// <summary>
    /// Categoria de Centro Servicio
    /// </summary>
    [EnumMember]
    MOVIMIENTO_CENTRO_SERVICIO = 6,

    /// <summary>
    /// Categoria de Mensajero
    /// </summary>
    [EnumMember]
    MENSAJERO = 7,

    /// <summary>
    /// Operaciones entre Cajas de centro de Servicio
    /// </summary>
    [EnumMember]
    OPERACIONES_ENTRE_CAJAS_DE_CENTRO_SERVICIOS = 9,

    /// <summary>
    /// Categoria Racol Operacion Nacional (id = 10)
    /// </summary>
    [EnumMember]
    MOVIMIENTO_RACOL_OPN = 10,

    /// <summary>
    /// Categoria movimientos desde Caja de Operación Nacional
    /// </summary>
    [EnumMember]
    MOVIMIENTO_DESDE_CAJA_OPN = 11,

    /// <summary>
    /// Categoria para ajustes de caja de los centros de servicio
    /// </summary>
    [EnumMember]
    AJUSTES_CAJA_CENTROS_DE_SERVICIO = 12,

      /// <summary>
    /// Categoria para ajustes de caja de los bancos
    /// </summary>
    [EnumMember]
    AJUSTES_CAJA_BANCOS = 13

  }
}