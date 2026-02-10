using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
  /// <summary>
  /// Representa una categoria de un mensaje
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class MECategoriaMensaje : DataContractBase
  {
    /// <summary>
    /// Identificador unico de la categoria de los mensajes
    /// </summary>
    [DataMember]
    public int IdCategoria
    {
      get;
      set;
    }

    /// <summary>
    /// Descripcion de la categoria de un mensaje
    /// </summary>
    [DataMember]
    public string Descripcion
    {
      get;
      set;
    }
  }
}