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
  /// Clase con el DataContract del Servidor
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PATipoSectorCliente : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("TSC_IdTipoSectorServidor")]
    [Filtrable("TSC_IdTipoSectorServidor", "Codigo:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 12, MensajeError = "El campo debe ser de tipo numerico")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Codigo", Description = "ToolTipCodigoTipoSectorServidor")]
    public int IdTipoSectorServidor { get; set; }

    [DataMember]    
    public int IdTipoSectorCliente { get; set; }

        [DataMember]
    [Filtrable("TSC_Descripcion", "Nombre:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50, MensajeError = "El campo debe ser de tipo numerico")]
    [CamposOrdenamiento("TSC_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Nombre", Description = "ToolTipNombreTipoSectorServidor")]
    public string NombreTipo { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}
