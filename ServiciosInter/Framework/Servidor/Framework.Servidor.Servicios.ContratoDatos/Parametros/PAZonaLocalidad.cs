using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene las zonas que pertenecen a una localidad y de las localidades que pertenecen a una zona
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAZonaLocalidad : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "Zona", Description = "TooltipZona")]
    public PAZonaDC Zona { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "Localidad")]
    public PALocalidadDC Localidad { get; set; }
  }
}