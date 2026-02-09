using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://interrapidisimo.com")]
  public class OUTipoConsolidadoDetalleDC
  {
    [DataMember]
    public short Id { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public List<OUColorTipoConsolidadoDetalleDC> Colores { get; set; }
  }
}