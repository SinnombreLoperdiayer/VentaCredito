using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  [DataContract]
  public class PACuentaExternaDC : DataContractBase
  {
    [DataMember]
    public int IdCuentaExterna { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public string IdNaturaleza { get; set; }

    [DataMember]
    public string Codigo { get; set; }
  }
}