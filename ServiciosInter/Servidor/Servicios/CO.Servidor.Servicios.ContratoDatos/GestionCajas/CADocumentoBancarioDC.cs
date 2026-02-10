using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase de tipo de documento bancario
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CADocumentoBancarioDC : DataContractBase
  {
    [DataMember]
    public short TipoDocBancario { get; set; }

    [DataMember]
    public string DescripcionTipoDocBancario { get; set; }

    [DataMember]
    public string NumeroDocBancario { get; set; }
  }
}