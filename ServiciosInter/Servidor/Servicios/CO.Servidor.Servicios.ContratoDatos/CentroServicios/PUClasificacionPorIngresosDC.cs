using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de la clasificacion de los centros de serviciso  para giros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUClasificacionPorIngresosDC
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ClasificacionPorIngresos", Description = "ClasificacionPorIngresos")]
    public string IdClasificacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ClasificacionPorIngresos", Description = "ClasificacionPorIngresos")]
    public string Descripcion { get; set; }
  }
}