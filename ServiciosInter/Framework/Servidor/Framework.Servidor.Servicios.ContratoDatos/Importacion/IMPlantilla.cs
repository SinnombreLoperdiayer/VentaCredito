using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Importacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class IMPlantilla
  {
    [DataMember]
    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "CampoRequerido", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas))]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreTabla", Description = "TooltipNombreTabla")]
    public string NombreTabla { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceName = "CampoRequerido", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas))]
    [Range(1, 10, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "RangoIncorrecto")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroHoja", Description = "TooltipNumeroHoja")]
    public int NumeroHoja { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceName = "CampoRequerido", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas))]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombrePlantilla", Description = "TooltipNombrePlantilla")]
    public string NombrePlantilla { get; set; }

    [DataMember]
    public ObservableCollection<IMDetallePlantilla> Detalles { get; set; }
  }
}