using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene el clasificador del canal de ventas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUClasificadorCanalVenta : DataContractBase
  {
    [DataMember]
    
    public int IdClasificadorCanalVentas { get; set; }
    
    [DataMember]
    [Filtrable("CCV_ClasificadorCanalVenta", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "CodClasificadorCanalVenta", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodClasificadorCanalVenta", Description = "ToolTipCodClasificadorCanalVenta")]
    [StringLength(12, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [CamposOrdenamiento("CCV_ClasificadorCanalVenta")]
    public string ClasificadorCanalVenta { get; set; }
    
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombreClasificadoCanalVenta")]
    [StringLength(25, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Filtrable("CCV_Nombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Nombre", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("CCV_Nombre")]
    public string Nombre { get; set; }

    [DataMember]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoPropiedad", Description = "TooltipTipoPropiedad")]    
    public int IdTipoPropiedad { get; set; }
    
    
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoPropiedad", Description = "TooltipTipoPropiedad")]
    [CamposOrdenamiento("TPR_Descripcion")]
    public string NombreTipoPropiedad { get; set; }

    [DataMember]
       [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoCentroServicio", Description = "ToolTipTipoCentroServicio")]      
    public string IdTipoCentroServicios { get; set; }
    
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoCentroServicio", Description = "ToolTipTipoCentroServicio")]      
    [CamposOrdenamiento("TCS_Descripcion")]
    public string NombreTipoCentroServicios { get; set; }
    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}
