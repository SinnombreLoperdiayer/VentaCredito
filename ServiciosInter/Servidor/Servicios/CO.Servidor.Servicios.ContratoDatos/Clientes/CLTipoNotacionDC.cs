using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de los tipos de notacion de las facturas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLTipoNotacionDC : DataContractBase
  {
    [DataMember]
    public short Idtipo { get; set; }

    [DataMember]
    [StringLength(50, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }
  }
}