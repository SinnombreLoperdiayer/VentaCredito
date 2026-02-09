using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
  /// <summary>
  /// Representa una agencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class MEAgencia : DataContractBase
  {
    /// <summary>
    /// Identificador unico de la agencia
    /// </summary>
    [DataMember]
    public long IdAgencia
    {
      get;
      set;
    }

    /// <summary>
    /// Nombre de la agencia
    /// </summary>
    [DataMember]
    public string Descripcion
    {
      get;
      set;
    }
  }
}