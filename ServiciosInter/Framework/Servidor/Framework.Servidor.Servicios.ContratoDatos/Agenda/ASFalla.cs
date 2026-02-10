using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene la información de una Falla
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASFalla : DataContractBase
  {
    /// <summary>
    /// Identificador de la falla
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("FAL_IdFalla")]
    [Editable(false, AllowInitialValue = true)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo")]
    public int IdFalla { get; set; }

    /// <summary>
    /// Descripción de la falla
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("FAL_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescripcionFalla")]
    [Filtrable("FAL_Descripcion", "Descripción Falla", COEnumTipoControlFiltro.TextBox)]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Estado de la Falla
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
    public ASEstado Estado { get; set; }

    /// <summary>
    /// Indica si es editable
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EsEditable", Description = "TooltipEsEditable")]
    public bool EsEditable { get; set; }

    /// <summary>
    /// Tipo de falla para su asignación
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoFalla", Description = "TooltipTipoFalla")]
    public ASETipoFalla TipoFalla { get; set; }

    /// <summary>
    /// Tareas asignadas a la falla
    /// </summary>
    [DataMember]
    [Display(AutoGenerateField = false)]
    public IList<ASTarea> Tareas { get; set; }

    /// <summary>
    /// Id del módulo
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulo", Description = "TooltipModuloFalla")]
    public string IdModulo { get; set; }

    /// <summary>
    /// Descripción del módulo
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MOD_Descripcion")]
    [Filtrable("MOD_Descripcion", "Descripción:", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulo", Description = "TooltipModuloFalla")]
    public string ModuloDescripcion { get; set; }

    /// <summary>
    /// Contiene los tipos de falla para mostrar en el combobox de tipos de falla
    /// </summary>
    [IgnoreDataMember]
    public List<ASETipoFalla> TiposFalla
    {
      get
      {
        return new List<ASETipoFalla>(new List<ASETipoFalla>() { ASETipoFalla.Automática, ASETipoFalla.Manual });
      }
    }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}