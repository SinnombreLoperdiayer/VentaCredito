using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros
{
  /// <summary>
  /// Informacion de un convenio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIConvenioDC : DataContractBase
  {
    [DataMember]
    public int IdConvenio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Nit", Description = "TooltipNit")]
    public string NitConvenio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "RazonSocial", Description = "TooltipRazonSocial")]
    public string RazonSocialConvenio { get; set; }

    /// <summary>
    /// Nit
    /// </summary>
    [DataMember]
    public string TipoConvenio { get; set; }
  }
}