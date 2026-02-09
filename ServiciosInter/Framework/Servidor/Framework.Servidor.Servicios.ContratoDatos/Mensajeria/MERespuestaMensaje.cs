using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
  /// <summary>
  /// Respuesta asociada a un mensaje
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class MERespuestaMensaje : DataContractBase
  {
    /// <summary>
    /// Usuario que respondió el mensaje
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "Usuario")]
    public string Usuario
    {
      get;
      set;
    }

    /// <summary>
    /// Fecha de respuesta del mensaje
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "FechaCreacion")]
    public DateTime Fecha
    {
      get;
      set;
    }

    /// <summary>
    /// Texto asociado a la respuesta del mensaje
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipRespuesta", Description = "ToolTipRespuesta")]
    public string Texto
    {
      get;
      set;
    }

    /// <summary>
    /// Identificador unico de la respuesta del mensaje
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipIdMensaje", Description = "ToolTipIdMensaje")]
    public int IdRespuesta
    {
      get;
      set;
    }

    /// <summary>
    /// Identificador unico del mensaje
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ToolTipIdMensaje", Description = "ToolTipIdMensaje")]
    public int IdMensaje
    {
      get;
      set;
    }
  }
}