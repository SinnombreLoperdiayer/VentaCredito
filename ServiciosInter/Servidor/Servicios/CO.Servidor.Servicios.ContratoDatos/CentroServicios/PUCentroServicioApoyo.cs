using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene los centros de servicio que sirven de apoyo a otros centros de servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUCentroServicioApoyo : DataContractBase
  {
    [DataMember]
    [Display(Name = "Nombre Col")]
    public string NombreCentroServicio { get; set; }

    [DataMember]
    [Display(Name = "Id Col")]
    public long IdCentroservicio { get; set; }

    [DataMember]
    public long IdRacol { get; set; }

    [DataMember]
    public string TipoCentroServicio { get; set; }

    [DataMember]
    public string DireccionCentroServicio { get; set; }
  }
}