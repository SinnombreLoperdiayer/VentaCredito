using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  /// <summary>
  /// Clase que contiene la informacion para el cambio de clave de usuario
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEPasswordUsuario : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("USU_IdUsuario")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "TooltipIdUsuario")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(20, MinimumLength = 5, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Usuario { get; set; }

    [DataMember]
    public string Password { get; set; }

    public string PasswordAnterior { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PasswordNuevo", Description = "ToolTipPassword")]
    [StringLength(20, MinimumLength = 8, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "PasswordNocumple")]
    public string PasswordNuevo { get; set; }

    public string ConfirmarPassword { get; set; }
  }
}