using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la informacion de la tbl de Motivos de Solicitud
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIMotivoSolicitudDC : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("MOS_IdMotivoSolicitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdMotivoSolicitud", Description = "TooltipDescMotivo")]
    public int IdMotivo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [CamposOrdenamiento("MOS_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescMotivo")]
    [Filtrable("MOS_Descripcion", "Descripción Motivo", COEnumTipoControlFiltro.TextBox)]
    public string DescripcionMotivo { get; set; }

    [DataMember]
    [CamposOrdenamiento("TIS_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSolicitud", Description = "TooltipTipoSolicitud")]
    [Filtrable("TIS_Descripcion", "Tipo de Solicitud", COEnumTipoControlFiltro.TextBox)]
    //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public string DescripcionTipo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSolicitud", Description = "TooltipTipoSolicitud")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public GITipoSolicitudDC TipoSolicitud { get; set; }

    [IgnoreDataMember]
    public ObservableCollection<GITipoSolicitudDC> TiposSolicitudes { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    public bool EsEditable { get; set; }

    [DataMember]
    [Display(Name = "Retorna Flete", Description = "Indica si se hace la devolución del valor del flete del giro.")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public bool RetornaFlete { get; set; }
  }
}