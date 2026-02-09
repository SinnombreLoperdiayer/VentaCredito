using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de los horarios de los servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUHorariosServiciosCentroServicios : DataContractBase
  {
    [DataMember]
    public long IdCentroServicioSrvDia { get; set; }

    [DataMember]
    public long IdCentroServiciosServicio { get; set; }

    [DataMember]
    public string IdDia { get; set; }

    [DataMember]
    public DateTime HoraInicio { get; set; }

    [DataMember]
    public DateTime HoraFin { get; set; }

    [DataMember]
    public string NombreDia { get; set; }
  }
}