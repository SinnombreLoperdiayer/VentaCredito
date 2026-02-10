using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADRotulo
  {
    [DataMember]
    public long NumeroGuia { get; set; }

    [DataMember]
    public bool Seleccionado { get; set; }

    [DataMember]
    public string Rotulo { get; set; }

    [DataMember]
    public int NumeroPieza { get; set; }
  }
}
