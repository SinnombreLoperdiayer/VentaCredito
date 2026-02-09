using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de las observaciones y novedades de un centros de servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUObservacionCentroServicioDC
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "Motivo")]
    public string Motivo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedades", Description = "Novedades")]
    public string Obsevacion { get; set; }
  }
}