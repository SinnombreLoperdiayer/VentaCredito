using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract(Namespace = "http://interrapidisimo.com")]
  public class SUTamanoTulaDC
  {
    [DataMember]
    public short Id { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}
