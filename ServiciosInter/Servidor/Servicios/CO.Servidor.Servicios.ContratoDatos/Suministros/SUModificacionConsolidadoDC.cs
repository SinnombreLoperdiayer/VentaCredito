using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract(Namespace = "http://interrapidisimo.com")]
  public class SUModificacionConsolidadoDC
  {
    [DataMember]
    public string CodigoConsolidado { get; set; }

    [DataMember]
    public SUMotivoCambioDC MotivoCambio { get; set; }

    [DataMember]
    public string Observaciones { get; set; }
  }
}
