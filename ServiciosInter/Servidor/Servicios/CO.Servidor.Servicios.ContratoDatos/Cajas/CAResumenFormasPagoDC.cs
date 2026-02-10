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
  /// Clase que contiene la Informacion
  /// del resumen de ingresos, egresos
  /// por Formas de Pago
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAResumenFormasPagoDC : DataContractBase
  {
    /// <summary>
    /// Es el Id de la forma pago.
    /// </summary>
    [DataMember]
    public short IdFormaPago { get; set; }

    /// <summary>
    /// Es el nombre de la forma pago.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago")]
    public string NombreFormaPago { get; set; }

    /// <summary>
    /// Es el valor de la forma pago.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago")]
    public decimal ValorFormaPago { get; set; }

    /// <summary>
    /// Valor total del Servicio incluye Vr Servicio, Vr Adicionales
    /// Vr Prima, Vr tercers, Vr Impuestos,Vr Retenciones se evalua
    /// si es un ingreso
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ingreso", Description = "ToolTipIngreso")]
    public decimal Ingreso { get; set; }

    /// <summary>
    /// Valor total del Servicio incluye Vr Servicio, Vr Adicionales
    /// Vr Prima, Vr tercers, Vr Impuestos,Vr Retenciones se evalua
    /// si es un Egreso
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Egreso", Description = "ToolTipEgreso")]
    public decimal Egreso { get; set; }

    /// <summary>
    /// Es la cantidad por Concepto del servicio.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
    public int Cantidad { get; set; }
  }
}