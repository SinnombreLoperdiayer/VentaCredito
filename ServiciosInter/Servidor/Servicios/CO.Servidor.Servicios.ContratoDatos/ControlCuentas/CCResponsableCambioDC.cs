using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCResponsableCambioDC
  {
    [DataMember]
    public CCTipoResponsableDC Responsable { get; set; }

    [DataMember]
    public string IdentificadorResponsable { get; set; }

    [DataMember]
    public string DescripcionResponsable { get; set; }

    [DataMember]
    public string CargoResponsable { get; set; }
  }
}
