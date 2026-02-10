using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Versionamiento
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class VECliente : DataContractBase
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}