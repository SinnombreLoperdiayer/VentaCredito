using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase con la informacion de los motivos de elimnacion
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONMotivoElimGuiaMani : DataContractBase
  {
    [DataMember]
    public long IdManifiestoOperacionNacional { get; set; }

    [DataMember]
    public long NumeroManifiestoCarga { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoEliminacion", Description = "ToolTipMotivoEliminacionGuia")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Range(1,int.MaxValue,ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdTipoMotivoEliminacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoEliminacion", Description = "ToolTipMotivoEliminacionGuia")]
    public string NombreTipoMotivoEliminacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
    public long NumeroGuia { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Observaciones { get; set; }

    [DataMember]
    public bool GuiaSuelta { get; set; }

    [DataMember]
    public long IdManifiestoGuia { get; set; }

    [DataMember]
    public long IdManifiestoConsolidadoDetalle { get; set; }

    [DataMember]
    public int TipoRuta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDespachoManifiesto", Description = "ToolTipCiudadDespachoManifiesto")]
    public string IdLocalidadDespacho { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDespachoManifiesto", Description = "ToolTipCiudadDespachoManifiesto")]
    public string NombreLocalidadDespacho { get; set; }
    
    
  }
}