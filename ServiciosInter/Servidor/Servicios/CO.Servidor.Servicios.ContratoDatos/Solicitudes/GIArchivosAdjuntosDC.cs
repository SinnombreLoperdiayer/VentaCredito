using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la Informacion de la Tbl de Archivos de la Solicitud
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIArchivosAdjuntosDC : DataContractBase
  {
    /// <summary>
    /// Nombre del archivo adjunto
    /// </summary>
    [DataMember]
    public string NombreArchivo { get; set; }

    /// <summary>
    /// Identificador del archivo
    /// </summary>
    [DataMember]
    public long? IdArchivo { get; set; }
  }
}