using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Contiene información de los suministros asociados aun centro de servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUCentroServiciosSuministro : DataContractBase
  {
    /// <summary>
    /// Id de centroServicioSuministro
    /// </summary>
    [DataMember]
    public int IdCentroServicioSuministro { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCentroServicio", Description = "TooltipIdCentroServicio")]
    public long IdCentroServicios { get; set; }

    [DataMember]
    [CamposOrdenamiento("CSS_IdSuministro")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Suministro", Description = "ToolTipSuministro")]
    public int IdSuministro { get; set; }

    [DataMember]
    [StringLength(20, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [CamposOrdenamiento("SUM_Descripcion")]
    [Filtrable("SUM_Descripcion", "Suministro:", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombreSuministro")]
    public string NombreSuministro { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CantidadAsignada", Description = "ToolTipCantidadAsignadaSuministro")]
    public decimal CantidadAsignada { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "StockMinimoSuministro", Description = "ToolTipStockMinimoSuministro")]
    public decimal StockMinimo { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}