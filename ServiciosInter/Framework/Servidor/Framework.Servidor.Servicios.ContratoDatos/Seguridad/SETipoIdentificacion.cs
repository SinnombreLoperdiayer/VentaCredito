using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SETipoIdentificacion : DataContractBase
  {
    [DataMember]
    public string IdTipoIdentificacion { get; set; }

    [DataMember]
    public string DescripcionIdentificacion { get; set; }
  }
}