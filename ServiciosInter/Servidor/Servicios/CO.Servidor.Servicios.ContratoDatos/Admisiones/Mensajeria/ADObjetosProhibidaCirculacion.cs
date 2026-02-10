using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  /// <summary>
  /// Contiene información de un objeto de prohibida circulación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADObjetoProhibidaCirculacion
  {
    /// <summary>
    /// Identificador del objeto
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Descripción del objeto
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ObjetoProhibidaCirculacion")]
    public string Descripcion { get; set; }
  }
}