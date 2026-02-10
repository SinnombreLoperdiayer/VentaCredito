using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADGuiaAlCobro
  {
    [DataMember]
    public ADGuia Guia { get; set; }

    [DataMember]
    public bool SePuedeDescargar
    {
      get;
      set;
    }
  }
}
