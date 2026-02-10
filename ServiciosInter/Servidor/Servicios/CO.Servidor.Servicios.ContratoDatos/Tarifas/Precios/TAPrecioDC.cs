using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Precio del servicio de giros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioDC : DataContractBase
  {
    /// <summary>
    /// Servicios Adicionales al giro
    /// </summary>
    [DataMember]
    public ObservableCollection<TAValorAdicional> ServiciosSolicitados { get; set; }

    /// <summary>
    /// Impuestos del servicio
    /// </summary>
    [DataMember]
    public List<TAImpuestoDelServicio> InfoImpuestos { get; set; }

    /// <summary>
    /// Precio que vale el servicio Valor del Porte
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorFlete", Description = "ValorFlete")]
    public decimal ValorServicio { get; set; }

    /// <summary>
    /// sumatoria valor servicio(porte)  + servicios + impuestos
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorTotalServicio", Description = "TooltipValorTotalServicio")]
    public decimal ValorTotalServicio { get; set; }

    /// <summary>
    /// valor a enviar al destinatario en el servicio de giros
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorGiro", Description = "ToolTipValorGiro")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal ValorGiro { get; set; }

    /// <summary>
    /// Tarifa porcentual con la cual se liquido el porte del giro
    /// </summary>
    [DataMember]
    public decimal? TarifaPorcPorte { get; set; }

    /// <summary>
    /// Tarifa fija con la cual se liquido el porte del giro
    /// </summary>
    [DataMember]
    public decimal? TarifaFijaPorte { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorAdicional", Description = "ValorAdicional")]
    public decimal ValorAdicionales { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorImpuestos", Description = "ValorImpuestos")]
    public decimal ValorImpuestos { get; set; }

    /// <summary>
    /// Es la Suma de valor giro + porte + valores adicionales + impuestos
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorTotal", Description = "ValorTotal")]
    public decimal ValorTotal { get; set; }
  }
}