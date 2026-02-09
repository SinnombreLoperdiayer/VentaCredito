using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion del tipos de las agencias
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PATipoZona : DataContractBase
  {
    [DataMember]
    public int IdTipoZona { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}