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
  /// Clase que contiene la informacion de los tipos de actividad Economica
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PATipoActEconomica : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "IdTipoActEconomica")]
    public int IdTipoActEconomica { get; set; }

    [DataMember]
    [Filtrable("TAE_Descripcion", "Descripción: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("TAE_Descripcion")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Descripcion", Description = "TooltipDescTipoActEconomica")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(250, MinimumLength = 5, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}