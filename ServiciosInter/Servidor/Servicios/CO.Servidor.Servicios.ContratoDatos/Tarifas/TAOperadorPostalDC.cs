using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Contiene la información de los Operadores Postales
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAOperadorPostalDC : DataContractBase
  {
    [DataMember]
    public int IdOperadorPostal { get; set; }

    [DataMember]
    public string Nombre { get; set; }
  }
}