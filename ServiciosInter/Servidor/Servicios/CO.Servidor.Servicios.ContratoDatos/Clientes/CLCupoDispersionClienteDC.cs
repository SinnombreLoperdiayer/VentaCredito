using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de los contratos de un cliente
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLCupoDispersionClienteDC
  {
    [DataMember]
    public decimal CupoDispersionAprobado { get; set; }
  }
}