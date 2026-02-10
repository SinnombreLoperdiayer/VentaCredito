using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase con Info de la tbl AperturacajaCentroServicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAAperturaCajaCentroSvcDC : DataContractBase
  {
    /// <summary>
    /// Es la Llave de la tbl con AperturaCaja
    /// </summary>
    /// <value>
    /// The id apertura caja centro SVC.
    /// </value>
    [DataMember]
    public long IdAperturaCajaCentroSvc { get; set; }

    /// <summary>
    /// Es le id del centro de servicio
    /// </summary>
    /// <value>
    /// The id centro servicio.
    /// </value>
    [DataMember]
    public long IdCentroServicio { get; set; }

    /// <summary>
    /// es le nombre del centro de servicio
    /// </summary>
    /// <value>
    /// The nombre centro servicio.
    /// </value>
    [DataMember]
    public string NombreCentroServicio { get; set; }
  }
}