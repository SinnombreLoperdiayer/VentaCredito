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
  public class GIArchSolicitudDC : DataContractBase
  {
    [DataMember]
    public int IdAdjunto { get; set; }

    [DataMember]
    public long? IdArchivo { get; set; }

    [DataMember]
    public long? IdSolicitud { get; set; }

    /// <summary>
    /// Tipos de Documentos Asociados al Cambio
    /// de Destinatario
    /// </summary>
    [DataMember]
    public GIDocsCambDestDC TipDocAsoCambDest { get; set; }

    [DataMember]
    public string NombreDocAsociado { get; set; }

    [DataMember]
    public DateTime FechaCarga { get; set; }

    [DataMember]
    public string Usuario { get; set; }

    [DataMember]
    public string DescripCargaArchivo { get; set; }

    [DataMember]
    public string DirAdjunto { get; set; }
  }
}