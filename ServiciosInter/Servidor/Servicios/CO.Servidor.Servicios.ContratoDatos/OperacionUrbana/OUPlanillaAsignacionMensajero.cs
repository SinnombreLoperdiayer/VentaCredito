using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUPlanillaAsignacionMensajero
  {
    [DataMember]
    public OUMensajeroDC Mensajero { get; set; }

    [DataMember]
    public long NumeroGuia { get; set; }

    [DataMember]
    public long IdPlanillaAsignacionEnvio { get; set; }

    [DataMember]
    public string EstadoEnPlanilla { get; set; }
  }
}
