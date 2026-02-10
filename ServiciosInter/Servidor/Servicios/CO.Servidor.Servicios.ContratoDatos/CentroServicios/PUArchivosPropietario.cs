using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase con el DataContract de los archivos de la personas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUArchivosPropietario : DataContractBase
  {
    [DataMember]
    public PUArchivosPersonas ArchivosPersonas { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdPropietario { get; set; }
  }
}