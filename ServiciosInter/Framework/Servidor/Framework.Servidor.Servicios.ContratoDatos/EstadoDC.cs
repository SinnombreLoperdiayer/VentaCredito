using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos
{
  /// <summary>
  /// Clase para manejo de estado código/descripción
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class EstadoDC
  {
    /// <summary>
    /// Retorna o asigna el identificador de estado
    /// </summary>
    [DataMember]
    public string IdEstado { get; set; }

    /// <summary>
    /// Retorna o asigna la descripción de estado
    /// </summary>
    [DataMember]
    public string EstadoDescripcion { get; set; }
  }
}