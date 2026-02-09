using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de las zonas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAZonaDC : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("ZON_IdZona")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    //[Editable(false, AllowInitialValue = true)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo", Description = "ToolTipCodigoZona")]
    public string IdZona { get; set; }

    [DataMember]
    [CamposOrdenamiento("ZON_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "ToolTipDescripcionZona")]
    [Filtrable("ZON_Descripcion", "Descripción Zona", COEnumTipoControlFiltro.TextBox)]
    public string Descripcion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoZona", Description = "ToolTipTipoZona")]
    public int? IdTipoZona { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoZona", Description = "ToolTipTipoZona")]
    public string NombreTipoZona { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Asignado")]
    public bool AsignadoALocalidad { get; set; }

    [DataMember]
    public bool AsignadoALocalidadOrig { get; set; }

    /// <summary>
    /// Enumeracion que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}