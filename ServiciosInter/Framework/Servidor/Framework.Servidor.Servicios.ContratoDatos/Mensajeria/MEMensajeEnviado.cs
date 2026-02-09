using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
  /// <summary>
  /// Representa un mensaje enviado a un punto de atencion
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class MEMensajeEnviado 
  {
    /// <summary>
    /// Identificador unico del mensaje
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MEN_IdMensaje")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipIdMensaje", Description = "ToolTipIdMensaje")]
    public int IdMensaje
    {
      get;
      set;
    }

    /// <summary>
    /// Fecha de creación del mensaje
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MEN_FechaCreacion")]
    [Filtrable("MEN_FechaCreacion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "FechaCreacion", COEnumTipoControlFiltro.DatePicker)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "FechaCreacion")]
    public DateTime FechaCreacion
    {
      get;
      set;
    }

    /// <summary>
    /// Usuario que creó el mensaje
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MEN_UsuarioOrigen")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "Usuario")]
    public string UsuarioOrigen
    {
      get;
      set;
    }

    /// <summary>
    /// Asunto del mensaje
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [CamposOrdenamiento("MEN_Asunto")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipAsunto", Description = "ToolTipAsunto")]
    public string Asunto
    {
      get;
      set;
    }

    /// <summary>
    /// Texto del mensaje
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipMensaje", Description = "ToolTipMensaje")]
    public string Texto
    {
      get;
      set;
    }

    /// <summary>
    /// Estado de la mensaje
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MEN_EstadoNotificacion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "Estado")]
    public MEEnumEstadoNotificacion EstadoNotificacion
    {
      get;
      set;
    }

    
    /// <summary>
    /// Usuario que leyó el mensaje
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MEN_UsuarioQueLeyo")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipLeidoPor", Description = "ToolTipLeidoPor")]
    public string UsuarioQueLeyo
    {
      get;
      set;
    }

    /// <summary>
    /// Mensaje original del cual proviene este mensaje
    /// </summary>
    [DataMember]
    public MEMensajeEnviado MensajeOriginal
    {
      get;
      set;
    }

    /// <summary>
    /// Respuestas asociadas a este mensaje
    /// </summary>
    [DataMember]
    public List<Framework.Servidor.Servicios.ContratoDatos.Mensajeria.MERespuestaMensaje> RespuestasMensaje
    {
      get;
      set;
    }

    /// <summary>
    /// Categoria del mensaje
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipCategoria", Description = "ToolTipCategoria")]
    [Range(1, 100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int Categoria
    {
      get;
      set;
    }

    /// <summary>
    /// Centro de servicios al cual va dirigido el mensaje
    /// </summary>
    [DataMember]
    public long? CentroServicioDestino
    {
      get;
      set;
    }

    /// <summary>
    /// Descripcion del Centro de servicios al cual va dirigido el mensaje
    /// </summary>
    [DataMember]
    [Filtrable("MEN_IdPuntoAtencionDestino", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "ToolTipCentroServicio", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("MEN_IdPuntoAtencionDestino")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipCentroServicio", Description = "ToolTipCentroServicio")]
    public string DescCentroServicioDestino
    {
      get;
      set;
    }

    /// <summary>
    /// Indica si el mensaje acepta o no respuestas
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MEN_AceptaRespuestas")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipAceptaRespuestas", Description = "ToolTipAceptaRespuestas")]
    public bool AceptaRespuestas
    {
      get;
      set;
    }

    [DataMember]
    public List<MEAdjuntos> Adjuntos { get; set; }

  }
}