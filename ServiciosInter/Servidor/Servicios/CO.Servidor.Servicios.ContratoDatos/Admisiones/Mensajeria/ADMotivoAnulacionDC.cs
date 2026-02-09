using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  /// <summary>
  /// Clase que contiene la información de los motivos de anulación de guías
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADMotivoAnulacionDC
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoAnulacion", Description = "ToolTipMotivoAnulacion")]
    public short IdMotivoAnulacion { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}