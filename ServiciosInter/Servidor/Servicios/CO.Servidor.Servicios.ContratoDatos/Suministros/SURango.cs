using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Contiene información de los rangos asignados al suministro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SURango
  {
    /// <summary>
    /// Prefijo
    /// </summary>
    [DataMember]
    public string Prefijo { get; set; }

    /// <summary>
    /// Inicio
    /// </summary>
    [DataMember]
    public long Inicio { get; set; }

    /// <summary>
    /// Fin
    /// </summary>
    [DataMember]
    public long Fin { get; set; }
  }
}