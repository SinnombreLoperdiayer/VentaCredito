using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase de Saldos Finales
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CASaldosFinalesDC : DataContractBase
  {
    /// <summary>
    /// Gets or sets the base inicial.
    /// </summary>
    /// <value>
    /// Base inicial del Centro de Servicio.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseInicial", Description = "ToolTipBaseInicial")]
    public decimal BaseInicial { get; set; }

    /// <summary>
    /// Gets or sets the saldo anterior efectivo.
    /// </summary>
    /// <value>
    /// Es el saldo en Efectivo del Cierre Anterior.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAnteriorEfectivo", Description = "ToolTipSaldoAnteriorEfectivo")]
    public decimal SaldoAnteriorEfectivo { get; set; }

    /// <summary>
    /// Gets or sets the saldo anterior efectivo.
    /// </summary>
    /// <value>
    /// Es el saldo en Efectivo del Cierre Anterior.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalIngresosEfectivo", Description = "ToolTipTotalIngresosEfectivo")]
    public decimal TotalIngresosEfectivo { get; set; }

    /// <summary>
    /// Gets or sets the saldo anterior efectivo.
    /// </summary>
    /// <value>
    /// Es el saldo en Efectivo del Cierre Anterior.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEgresosEfectivo", Description = "ToolTipTotalEgresosEfectivo")]
    public decimal TotalEgresosEfectivo { get; set; }

    /// <summary>
    /// Gets or sets the saldo anterior efectivo.
    /// </summary>
    /// <value>
    /// Es el saldo en Efectivo del Cierre Anterior.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoFinalEfectivo", Description = "ToolTipSaldoFinalEfectivo")]
    public decimal SaldoFinalEfectivo { get; set; }

    /// <summary>
    /// Gets or sets the saldo anterior efectivo.
    /// </summary>
    /// <value>
    /// Es el saldo en Efectivo del Cierre Anterior.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalAEmpresa", Description = "ToolTipTotalAEmpresa")]
    public decimal TotalAEmpresa { get; set; }
  }
}