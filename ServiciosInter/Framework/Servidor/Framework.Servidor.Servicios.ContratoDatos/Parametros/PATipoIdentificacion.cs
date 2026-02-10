using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PATipoIdentificacion : DataContractBase
  {
    [DataMember]
    public string IdTipoIdentificacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "TipoIdentificacion", Description = "TipoIdentificacion")]
    public string DescripcionIdentificacion { get; set; }

    [DataMember]
    public string Estado { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}