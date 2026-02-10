using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUCategoriaSuministro : DataContractBase
  {
    [DataMember]
    public int IdCategoria { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}