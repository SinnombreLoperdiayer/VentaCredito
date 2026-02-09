using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADRangoCasillero
  {
    [DataMember]
    public decimal RangoInicial { get; set; }

    [DataMember]
    public decimal RangoFinal { get; set; }

    [DataMember]
    public string Casillero { get; set; }
  }
}
