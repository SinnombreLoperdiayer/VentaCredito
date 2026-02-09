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
  /// Clase que contiene la informacion de un servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUServicio : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "TooltipServicioComi")]
    public int IdServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "TooltipServicioComi")]
    public string NombreServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    public string IdUnidadNegocio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    public string NombreUnidadNegocio { get; set; }

    [DataMember]
    public long IdCentroServicio { get; set; }

    [DataMember]
    public long IdCentroServicioServicio { get; set; }

    [DataMember]
    public string NombreLocalidad { get; set; }

    [DataMember]
    public string NombreCompletoLocalidad { get; set; }

    [DataMember]
    public string IdLocalidad { get; set; }

      [DataMember]
    public List<PUHorariosServiciosCentroServicios> HorariosServicios { get; set; }

  }
}