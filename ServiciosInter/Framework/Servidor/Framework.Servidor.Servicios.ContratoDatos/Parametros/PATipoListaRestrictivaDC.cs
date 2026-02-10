using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la información del tipo de lista restrictiva
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PATipoListaRestrictivaDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "TipoListaRestrictiva", Description = "ToolTipTipListRestri")]
    public short IdTipoListaRestrictiva { get; set; }

    [DataMember]
    [CamposOrdenamiento("TLN_IdTipoListaNegra")]
    [Display(ResourceType = typeof(Etiquetas), Name = "TipoListaRestrictiva", Description = "ToolTipTipListRestri")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Descripcion { get; set; }
  }
}