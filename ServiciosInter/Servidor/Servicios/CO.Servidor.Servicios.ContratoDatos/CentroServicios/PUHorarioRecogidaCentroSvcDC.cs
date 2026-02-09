using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// clase que continr la info de la tbl Horarios Recogidas Centro Svc
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUHorarioRecogidaCentroSvcDC : DataContractBase
  {
    /// <summary>
    /// Es el id del centro de servicio que tiene
    /// asignado un horario de Recogida
    /// </summary>
    [DataMember]
    public long IdCentroServicio { get; set; }

    /// <summary>
    /// Es el nombre del dia de la Semana
    /// </summary>
    [DataMember]
    public string NombreDia { get; set; }

    /// <summary>
    /// Es le dia de la recogida
    /// </summary>
    [DataMember]
    public string Dia { get; set; }

    /// <summary>
    /// Es la hora de la recogida
    /// </summary>
    [DataMember]
    public DateTime Hora { get; set; }
  }
}