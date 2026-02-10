using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// contiene la informacion de la zona del Operador Postal
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAOperadorPostalZonaDC : DataContractBase
  {
    /// <summary>
    /// Es el id de la Zona
    /// </summary>
    [DataMember]
    public PAZonaDC Zona { get; set; }

    /// <summary>
    /// Es el id del Operador postal
    /// </summary>
    [DataMember]
    public PAOperadorPostal OperadorPostal { get; set; }

    /// <summary>
    /// Es el tiempo estimado de entrega dado en "DIAS"
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoEntrega", Description = "ToolTipTiempoEntrega")]
    public int TiempoEntrega { get; set; }

    /// <summary>
    /// Enumeracion que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}