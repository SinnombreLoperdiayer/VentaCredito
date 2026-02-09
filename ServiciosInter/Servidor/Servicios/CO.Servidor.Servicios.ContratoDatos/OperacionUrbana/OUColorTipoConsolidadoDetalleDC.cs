using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://interrapidisimo.com")]
  public class OUColorTipoConsolidadoDetalleDC
  {
    [DataMember]
    public short Id { get; set; }

    [DataMember]
    public string Color { get; set; }
  }
}
