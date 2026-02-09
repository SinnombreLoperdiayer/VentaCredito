using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de valor adicional
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAValorAdicional : DataContractBase
  {
    [DataMember]
    [Filtrable("TVA_IdTipoValorAdicional", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "IdTipoValorAdicional", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 3)]
    [CamposOrdenamiento("TVA_IdTipoValorAdicional")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdTipoValorAdicional", Description = "TooltipIdTipoValorAdicional")]
    [StringLength(3, MinimumLength = 3, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdTipoValorAdicional { get; set; }

    [DataMember]
    [Filtrable("TVA_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Descripcion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("TVA_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipValorAdicional")]
    [StringLength(25, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Descripcion { get; set; }

    [DataMember]
    public TAServicioDC Servicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "TooltipServicio")]
    public int IdServicio { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    /// <summary>
    /// Lista con los valores adicionales que puede utilizar el servicio de giros
    /// </summary>
    [DataMember]
    public IList<TACampoTipoValorAdicionalDC> CamposTipoValorAdicionalDC { get; set; }

    /// <summary>
    /// Almacena los precios por valor adicional para un servicio
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "Valor")]
    public decimal PrecioValorAdicional { get; set; }

    [DataMember]
    public bool EsEmbalaje { get; set; }
  }
}