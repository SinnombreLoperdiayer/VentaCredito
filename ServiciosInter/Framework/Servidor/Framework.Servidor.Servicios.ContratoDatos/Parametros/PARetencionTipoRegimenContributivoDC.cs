using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que continie la info de la tabla
  /// RetencionTipoRegimenContributivo
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PARetencionTipoRegimenContributivoDC : DataContractBase
  {
    /// <summary>
    /// Es el Id de la Retencion
    /// </summary>
    /// <value>
    /// The id retencion.
    /// </value>
    [DataMember]
    public short IdRetencion { get; set; }

    /// <summary>
    /// Es el Nombre de la Retencion
    /// </summary>
    /// <value>
    /// The descripcion retencion.
    /// </value>
    [DataMember]
    public string DescripcionRetencion { get; set; }

    /// <summary>
    /// Es el Id del Regimen
    /// </summary>
    /// <value>
    /// The id regimen.
    /// </value>
    [DataMember]
    public short IdRegimen { get; set; }
  }
}