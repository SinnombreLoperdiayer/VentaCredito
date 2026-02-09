using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase que contiene
  /// la info de tbl BancoCentroServicioMov
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAMovBancoCentroSvcDC : DataContractBase
  {
    /// <summary>
    /// Es el Id de la caja del Banco
    /// </summary>
    /// <value>
    /// The id caja banco.
    /// </value>
    [DataMember]
    public long IdCajaBanco { get; set; }

    /// <summary>
    /// Es el id del Centro de Servicio que realiza la Transaccion
    /// </summary>
    /// <value>
    /// The id centro servicio.
    /// </value>
    [DataMember]
    public long IdCentroServicio { get; set; }

    /// <summary>
    /// Nombre del centro de Servicio que realiz la transaccicon
    /// </summary>
    /// <value>
    /// The nombre centro servicio.
    /// </value>
    [DataMember]
    public string NombreCentroServicio { get; set; }

    /// <summary>
    /// Identificador del registro de caja del centro de
    /// servicios al cual se le hace el movimiento de caja
    /// </summary>
    /// <value>
    /// The id registro transaccion.
    /// </value>
    [DataMember]
    public long IdRegistroTransaccion { get; set; }

    /// <summary>
    /// Fecha de Grabacion de la transaccion
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Usuario que realiza la Transaccion
    /// </summary>
    /// <value>
    /// The craedo por.
    /// </value>
    [DataMember]
    public string CreadoPor { get; set; }
  }
}