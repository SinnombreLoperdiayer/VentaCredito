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
  /// Clase que contiene la información de la lista restrictiva
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAListaRestrictivaDC : DataContractBase
  {
    [DataMember]
    public long IdListaRestrictiva { get; set; }

    [DataMember]
    [Filtrable("LIN_Identificacion", typeof(Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 15)]
    [CamposOrdenamiento("LIN_Identificacion")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Identificacion", Description = "ToolTipIdentiListaRestr")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Identificacion { get; set; }

    [DataMember]
    [Filtrable("LIN_Nombre", typeof(Etiquetas), "Nombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 100)]
    [CamposOrdenamiento("LIN_Nombre")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Nombre", Description = "ToolTipNomListRestri")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Nombre { get; set; }

    [DataMember]
    [CamposOrdenamiento("LIN_Concepto")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Concepto", Description = "ToolTipConcepListRestri")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Concepto { get; set; }

    [DataMember]
    [CamposOrdenamiento("LIN_Estado")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Estado", Description = "ToolTipEstListRestri")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Estado { get; set; }

    [DataMember]
    //[Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public PATipoListaRestrictivaDC TipoListaRestrictiva { get; set; }

    //[DataMember]
    //[CamposOrdenamiento("TLN_IdTipoListaNegra")]
    //[Display(ResourceType = typeof(Etiquetas), Name = "TipoListaRestrictiva", Description = "ToolTipTipListRestri")]
    //[Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    //public string TipoListaRestrictiva { get; set; }

    [DataMember]
    public IEnumerable<PATipoListaRestrictivaDC> ColeccionTiposListasRestricitvas { get; set; }

    [DataMember]
    public IEnumerable<PAEstadoActivoInactivoDC> ColeccionEstados { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}