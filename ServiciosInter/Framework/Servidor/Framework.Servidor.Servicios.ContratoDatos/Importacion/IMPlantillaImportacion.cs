using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Importacion
{
  /// <summary>
  /// Plantilla de importación para archivos de excel
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class IMPlantillaImportacion
  {
    /// <summary>
    /// Nombre de la plantilla de importación
    /// </summary>
    [DataMember]
    [Required(AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombrePlantilla", Description = "TooltipNombrePlantilla")]
    public string Nombre { get; set; }

    /// <summary>
    /// Contenido de la plantilla en string
    /// </summary>
    [DataMember]
    public string Plantilla { get; set; }
  }
}