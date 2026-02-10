using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene la información de una Tarea
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASTarea : DataContractBase
  {
    /// <summary>
    /// Identificador de la Tarea
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("TAR_IdTarea")]
    [Editable(false)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo")]
    public int IdTarea { get; set; }

    /// <summary>
    /// Descripción de la tarea
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("TAR_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescripcionTarea")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Cargo responsable de la tarea
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]

    public ASCargo Cargo { get; set; }

    /// <summary>
    /// Tiempo de escalamiento
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("TAR_TiempoEscalamiento")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoEscalamiento", Description = "TooltipTiempoEscalamiento")]
    public int TiempoEscalamiento { get; set; }

    /// <summary>
    /// Falla a la que corresponde la tarea
    /// </summary>
    [DataMember]
    public int IdFalla { get; set; }

    /// <summary>
    /// Estado de la tarea
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("TAR_Estado")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstadoTarea")]
    public string Estado { get; set; }

    /// <summary>
    /// Enumeracion que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}