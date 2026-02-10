using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  /// Clase que contiene la informacion de la Tbl Condicion Operador Postal
  /// </summary>
  [DataContract(Namespace = "http://controller.com")]
  public class PACondicionOperadorPostalDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo", Description = "TooltipNumConOperadorPostal")]
    [CamposOrdenamiento("COP_IdCondicionOperadorPostal")]
    public int IdConOperadorPostal { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "ToolTipCondicionOperadorPostal")]
    [CamposOrdenamiento("COP_Descripcion")]
    public string Descripcion { get; set; }

    [DataMember]
    //[Required]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "OperadorPostal", Description = "TooltipOperadorPostal")]
    [CamposOrdenamiento("OPO_Nombre")]
    public string OperadorPostalNombre { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [IgnoreDataMember]
    public ObservableCollection<PAOperadorPostal> OperadoresPostales { get; set; }

    [DataMember]
    public PAOperadorPostal OperadorPostal { get; set; }
  }
}