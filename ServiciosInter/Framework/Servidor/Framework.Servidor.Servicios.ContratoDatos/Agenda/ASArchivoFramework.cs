using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene la información de un archivo adjunto al framework
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASArchivoFramework
  {
    /// <summary>
    /// Identificador del archivo
    /// </summary>
    [DataMember]
    public long IdArchivo { get; set; }

    /// <summary>
    /// Nombre del archivo adjunto
    /// </summary>
    [DataMember]
    public string NombreArchivo { get; set; }

    /// <summary>
    /// Fecha de carga
    /// </summary>
    [DataMember]
    public DateTime Fecha { get; set; }
  }
}