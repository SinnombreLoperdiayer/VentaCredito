using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion del cierre de un manifiesto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONCierreDescargueManifiestoDC
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEnviosManifestados")]
    public int TotalEnviosManifestado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEnviosSobrantes")]
    public long TotalEnviosSobrantes { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEnviosPendientes")]
    public long TotalEnviosPendientes { get; set; }
  }
}