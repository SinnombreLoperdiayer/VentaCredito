using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ParamPublicidad
  {
    [DataMember]
    public string Imagen { get; set; }

    [DataMember]
    public int TiempoPublicidad { get; set; }
  }
}
