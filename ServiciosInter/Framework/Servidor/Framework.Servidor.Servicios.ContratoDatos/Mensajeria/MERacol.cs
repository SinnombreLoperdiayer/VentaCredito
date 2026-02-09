using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
  /// <summary>
  /// Representa un racol
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class MERacol : DataContractBase
  {
    /// <summary>
    /// Identificador unico del racol
    /// </summary>
    [DataMember]
    public long? IdRacol
    {
      get;
      set;
    }

    /// <summary>
    /// Descripcion del racol
    /// </summary>
    [DataMember]
    public string Descripcion
    {
      get;
      set;
    }

    /// <summary>
    /// Agencias asociadas al racol
    /// </summary>
    [DataMember]
    public List<Framework.Servidor.Servicios.ContratoDatos.Mensajeria.MEAgencia> Agencias
    {
      get;
      set;
    }
  }
}