using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIArchivosControlCtasExploradorDC : DataContractBase
  {
    /// <summary>
    /// Es el numero del Giro para
    /// consultar
    /// </summary>
    [DataMember]
    public long idGiro { get; set; }

    /// <summary>
    /// Es el numero del comprobante
    /// de pago del giro
    /// </summary>
    [DataMember]
    public long NumeroComprobantePago { get; set; }
  }
}