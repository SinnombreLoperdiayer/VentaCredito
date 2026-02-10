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
  /// Clase que contiene la informacion de un banco
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PABanco : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "IdBanco")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]

    [StringLength(25, MinimumLength = 5, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "LongitudCadena")]
    public string IdBanco { get; set; }

    [DataMember]
    [Filtrable("BAN_Descripcion", "Descripción: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("BAN_Descripcion")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Descripcion", Description = "TooltipDescBanco")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}