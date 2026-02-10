using System;
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
  /// Clase que contiene la información de Cuenta Externa
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TACuentaExternaDC : DataContractBase
  {
    [DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CuentaExterna", Description = "TooltipCuentaExterna")]
    //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public short IdCuentaExterna { get; set; }

    [DataMember]
    [Filtrable("CEX_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "CuentaExterna", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("CEX_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CuentaExterna", Description = "TooltipCuentaExterna")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Descripcion { get; set; }

    [DataMember]
    [Filtrable("CEX_IdNaturaleza", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "IdNaturaleza", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("CEX_IdNaturaleza")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdNaturaleza", Description = "ToolTipIdNaturaleza")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdNaturaliza { get; set; }

    [DataMember]
    [Filtrable("CEX_Codigo", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Codigo", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [CamposOrdenamiento("CEX_Codigo")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo", Description = "ToolTipCodigoCuentaExterna")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Codigo { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}