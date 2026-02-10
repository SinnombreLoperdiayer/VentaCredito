using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene los posibles estados de una falla o una tarea
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASEstado : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstadoFalla")]
    public string IdEstado { get; set; }

    [DataMember]
    public string Estado { get; set; }
  }
}