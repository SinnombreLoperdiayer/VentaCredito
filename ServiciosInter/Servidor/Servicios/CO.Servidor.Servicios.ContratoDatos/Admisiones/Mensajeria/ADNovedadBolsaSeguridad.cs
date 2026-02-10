using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  /// <summary>
  /// Contiene información de la novedad de una bolsa de seguridad
  /// </summary>
  [DataContract]
  public class ADNovedadBolsaSeguridad : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BolsaSeguridad", Description = "TooltipBolsaSeguridad")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string RegistroBolsaSeguridad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    public string ObservacionesBolsaSeguridad { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NovedadBolsaSeguridad", Description = "TooltipNovedadBolsaSeguridad")]
    public ADMotivoNoUsoBolsaSeguridad NovedadBolsaSeguridad { get; set; }
  }
}