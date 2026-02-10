using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de los tipos
  /// documento del banco
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PATipoDocumBancoDC : DataContractBase
  {
    /// <summary>
    /// id del tipo de doc
    /// </summary>
    [DataMember]
    public short IdTipoDocumento { get; set; }

    /// <summary>
    /// Descripcion del tipo de Documento
    /// Consignacion-retiro-transferencia
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "TipoDocumento", Description = "ToolTipTipoDocumento")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Estado del tipo de documento:
    /// Activo: ACT, Inactivo: INA
    /// </summary>
    [DataMember]
    public string Estado { get; set; }
  }
}