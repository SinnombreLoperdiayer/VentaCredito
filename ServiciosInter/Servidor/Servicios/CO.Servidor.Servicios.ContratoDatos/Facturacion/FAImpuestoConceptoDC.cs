using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  /// <summary>
  /// Reprsenta los impuestos asociados a un concepto de una factura
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class FAImpuestoConceptoDC
  {
    public FAImpuestoConceptoDC()
    {
      BaseCalculo = 0;
      ValorPorc = 0;
    }

    /// <summary>
    /// Concepto al cual está asociado el impuesto
    /// </summary>
    [DataMember]
    public long IdConceptoFactura { get; set; }

    /// <summary>
    /// Id del impuesto
    /// </summary>
    [DataMember]
    public short IdImpuesto { get; set; }

    /// <summary>
    /// Descripción del impuesto
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "Descripcion")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Base para cálculo del impuesto
    /// </summary>
    [DataMember]
    public decimal BaseCalculo { get; set; }

    /// <summary>
    /// Porcentaje del impuesto
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Porcentaje", Description = "Porcentaje")]
    public decimal ValorPorc { get; set; }

    /// <summary>
    /// Total del impuesto
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorTotal", Description = "ValorTotal")]
    public decimal Total
    {
      get
      {
        return BaseCalculo * ValorPorc / 100;
      }
      set
      {
      }
    }

    /// <summary>
    /// Fecha de grabación del registro
    /// </summary>
    [DataMember]
    public System.DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Usuario que creó el registro
    /// </summary>
    [DataMember]
    public string CreadoPor { get; set; }
  }
}