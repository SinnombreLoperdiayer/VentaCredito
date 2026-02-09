using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Contrato de datos para manejo de impuestos de un servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAImpuestoDelServicio : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna el Identificador del servico
    /// </summary>
    [DataMember]
    public int IdServicio { get; set; }

    /// <summary>
    /// Retorna o asigna el identificador del impuesto
    /// </summary>
    [DataMember]
    public short IdImpuesto { get; set; }

    /// <summary>
    /// Retorna a asigna la descripción del impuesto
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Impuestos", Description = "TooltipDescImpuesto")]
    public string DescripcionImpuesto { get; set; }

    /// <summary>
    /// Retorna o asigna el valor del impuesto asignado al servicio
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorImpuestoPorcentaje", Description = "TooltipValorImpuesto")]
    public decimal ValorImpuesto { get; set; }

    /// <summary>
    /// Retorna el el valor del impuesto aplicado al servicio
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "TooltipValorImpuesto")]
    public decimal ValorImpuestoAplicado { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}