using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAMovEmpresaCentroSvcDC : DataContractBase
  {
    /// <summary>
    /// Identificador de la transacción sobre la caja de la casa matriz
    /// </summary>
    /// <value>
    /// The id caja casa matriz.
    /// </value>
    public long IdOperacionCasaMatriz { get; set; }

    /// <summary>
    /// Identificador de la operación en la caja de Operación Nacional
    /// </summary>
    /// <value>
    /// The id registro transaccion.
    /// </value>
    public long IdOperacionCajaOpn { get; set; }

    /// <summary>
    /// Fecah de Grabacion del Registro
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Registro Realizado por.
    /// </summary>
    /// <value>
    /// The creado por.
    /// </value>
    public string CreadoPor { get; set; }
  }
}