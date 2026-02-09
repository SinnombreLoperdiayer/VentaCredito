using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la informacion de la Caja
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACajasDC : DataContractBase
  {
    /// <summary>
    /// Gets or sets the valor producto.
    /// </summary>
    /// <value>
    /// Es el valor del Producto Varlor del Servicio.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "IdFormaPago", Description = "ToolTipIdFormaPago")]
    public decimal ValorProducto { get; set; }

    /// <summary>
    /// Gets or sets the valor impuesto.
    /// </summary>
    /// <value>
    /// Es el Valor del Impuesto.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago", Description = "TooltipFormaPago")]
    public decimal ValorImpuesto { get; set; }

    /// <summary>
    /// Gets or sets the valor total.
    /// </summary>
    /// <value>
    /// Es el valor total del servicio + Impuesto.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "PagoMixto", Description = "TooltipPagoMixto")]
    public decimal ValorTotal { get; set; }
  }
}