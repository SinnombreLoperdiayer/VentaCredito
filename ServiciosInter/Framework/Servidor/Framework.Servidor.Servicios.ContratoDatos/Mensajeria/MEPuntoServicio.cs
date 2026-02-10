using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
  /// <summary>
  /// Representa un punto de servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class MEPuntoServicio : DataContractBase
  {
    /// <summary>
    /// Identificador unico del sistema
    /// </summary>
    [DataMember]
    public string IdPuntoServicio
    {
      get;
      set;
    }

    /// <summary>
    /// Descripcion del punto de servicio
    /// </summary>
    [DataMember]
    public string Descripcion
    {
      get;
      set;
    }
  }
}