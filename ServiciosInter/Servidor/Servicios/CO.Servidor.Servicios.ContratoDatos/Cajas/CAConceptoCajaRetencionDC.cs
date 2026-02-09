using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la Info de la
  /// liquidacion de la Retención
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAConceptoCajaRetencionDC : DataContractBase
  {
    /// <summary>
    /// Identificador del concepto de caja
    /// </summary>
    /// <value>
    /// The id concepto caja.
    /// </value>
    [DataMember]
    public int IdConceptoCaja { get; set; }

    /// <summary>
    /// Identificador de la retención
    /// </summary>
    /// <value>
    /// The id retencion.
    /// </value>
    [DataMember]
    public short IdRetencion { get; set; }

    /// <summary>
    /// Es la descripcion de la Retencion
    /// [Nombre de la Retencion]
    /// </summary>
    /// <value>
    /// The descripcion retencion.
    /// </value>
    [DataMember]
    public string DescripcionRetencion { get; set; }

    /// <summary>
    /// Valor fijo de la retención (escalar)
    /// </summary>
    /// <value>
    /// The valor fijo.
    /// </value>
    [DataMember]
    public decimal ValorFijo { get; set; }

    /// <summary>
    /// Porcentaje de la retención
    /// </summary>
    /// <value>
    /// The tarifa porcentual.
    /// </value>
    [DataMember]
    public decimal TarifaPorcentual { get; set; }

    /// <summary>
    /// Base sobre la cual aplica la retención
    /// </summary>
    /// <value>
    /// The base.
    /// </value>
    [DataMember]
    public decimal Base { get; set; }

    /// <summary>
    /// Es el valor de la Retencion aplicado.
    /// </summary>
    /// <value>
    /// The valor calculado retencion.
    /// </value>
    [DataMember]
    public decimal ValorCalculadoRetencion { get; set; }

    /// <summary>
    /// Es la fecha en la que se Realiza la Transaccion.
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Es el usuario que realizo la transaccion
    /// </summary>
    /// <value>
    /// The creado por.
    /// </value>
    [DataMember]
    public string CreadoPor { get; set; }
  }
}