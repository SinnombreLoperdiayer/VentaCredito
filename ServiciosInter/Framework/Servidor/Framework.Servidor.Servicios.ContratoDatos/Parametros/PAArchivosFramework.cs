using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase con el DataContract de los archivos del framework
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAArchivosFramework : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public long IdArchivo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Nombre")]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string NombreAdjunto { get; set; }

    [DataMember]
    public byte[] Adjunto { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public Guid IdAdjunto { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NombreServidor { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NombreCompleto { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Fecha")]
    public DateTime FechaCargaArchivo { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Descripcion")]
    public string Descripcion { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}