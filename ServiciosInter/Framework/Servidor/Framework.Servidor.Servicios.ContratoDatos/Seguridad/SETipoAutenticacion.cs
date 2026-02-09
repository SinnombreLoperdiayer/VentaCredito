using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  /// <summary>
  /// Contiene información del tipo de autenticación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SETipoAutenticacion : DataContractBase
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}