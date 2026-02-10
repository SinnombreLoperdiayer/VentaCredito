using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene la información de la tarea pendiente
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASTareaAsignada : DataContractBase
  {
    /// <summary>
    /// Comentarios de la tarea asignada
    /// </summary>
    [DataMember]
    [FiltrableAttribute("AST_Comentarios", "Comentario:", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ComentariosAsignacionTareaManual", Description = "TooltipComentariosAsignacionTareaManual")]
    [CamposOrdenamiento("AST_Comentarios")]
    public string Comentarios { get; set; }

    /// <summary>
    /// Descripción de la tarea
    /// </summary>
    [DataMember]
    [FiltrableAttribute("AST_DescripcionTarea", "Descripción:", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [CamposOrdenamiento("AST_DescripcionTarea")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescripcionTarea")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Falla a la que corresponde la tarea
    /// </summary>
    [DataMember]
    public ASFalla Falla { get; set; }

    /// <summary>
    /// Fecha en la cual se hizo la asignación de la tarea
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("AST_FechaAsignacion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaAsignaciontTarea", Description = "TooltipFechaAsignaciontTarea")]
    public DateTime FechaAsignacion { get; set; }

    /// <summary>
    /// Fecha límite en la cual se procederá al escalamiento
    /// </summary>
    [DataMember]
    public DateTime? FechaEscalamiento { get; set; }

    /// <summary>
    /// Identificador de la tarea asignada
    /// </summary>
    [DataMember]
    public long IdAsignacionTarea { get; set; }

    /// <summary>
    /// Identificador de la Tarea
    /// </summary>
    [DataMember]
    [FiltrableAttribute("AST_IdTarea", "Código:", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("AST_IdTarea")]
    [Editable(false)]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo")]
    public int IdTarea { get; set; }

    /// <summary>
    /// Tiempo de escalamiento
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("AST_TiempoEscalamiento")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoEscalamiento")]
    public int? TiempoEscalamiento { get; set; }

    /// <summary>
    /// Tiempo restante para finalizar la tarea en días
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoRestanteTarea", Description = "TooltipTiempoRestanteTarea")]
    public int TiempoRestante { get; set; }

    /// <summary>
    /// Tiempo restante para finalizar la tarea en días
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoExtension", Description = "TooltipTiempoExtension")]
    public int TiempoExtension { get; set; }

    /// <summary>
    /// Usuario responsable de la tarea
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("AST_UsuarioResponsable")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioResponsable", Description = "TooltipUsuarioResponsable")]
    public string UsuarioResponsable { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EventoAsignacion", Description = "TooltipEventoAsignacion")]
    public ASEEventoAsignacion EventoAsignacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaVencimiento", Description = "TooltipFechaVencimiento")]
    public DateTime FechaVencimiento { get; set; }

    /// <summary>
    /// Comentarios agregados a la tarea
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Comentarios", Description = "TooltipComentariosAsignacionTareaManual")]
    public string NuevosComentarios { get; set; }

    /// <summary>
    /// Indica si la tarea está cerrada, se usa es para indicar desde el Servidor hasta el servidor si la tarea se va a cerrar
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CerrarTarea")]
    public bool TareaCerrada { get; set; }

    /// <summary>
    /// Listado de los archivos adjuntos
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AdjuntosTarea", Description = "TooltipAdjuntosTarea")]
    public List<ASArchivoFramework> ArchivosAdjuntos { get; set; }
  }
}