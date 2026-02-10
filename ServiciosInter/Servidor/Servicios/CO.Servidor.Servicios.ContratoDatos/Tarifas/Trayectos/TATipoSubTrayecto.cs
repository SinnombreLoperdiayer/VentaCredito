using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de los tipos de subtrayecto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATipoSubTrayecto : DataContractBase
  {
    [DataMember]
    [Filtrable("TST_IdTipoSubTrayecto", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "IdTipoSubTrayecto", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 8)]
    [CamposOrdenamiento("TST_IdTipoSubTrayecto")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(8, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdTipoSubTrayecto", Description = "TooltipIdSubTrayecto")]
    public string IdTipoSubTrayecto { get; set; }

    [DataMember]
    [Filtrable("TST_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Descripcion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("TST_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(25, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SubTrayecto", Description = "TooltipDesSubTrayecto")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}