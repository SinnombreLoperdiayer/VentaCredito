using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de las condiciones  especiales de giros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLClienteCondicionGiroDC
  {
    [DataMember]
    public int IdContrato { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PermiteGiroConvenio", Description = "TooltipPermiteGiroConvenio")]
    public bool PermiteGiroConvenio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PermiteDispersion", Description = "TooltipPermiteDispersion")]
    public bool PermiteDispersion { get; set; }

    [DataMember]
    public bool ConvenioPagaPorte { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}