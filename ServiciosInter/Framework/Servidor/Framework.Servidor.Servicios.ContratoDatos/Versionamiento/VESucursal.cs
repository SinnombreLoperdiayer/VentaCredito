using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.Servicios.ContratoDatos.Versionamiento
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class VESucursal : DataContractBase
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public PALocalidadDC Ciudad { get; set; }

    [DataMember]
    public PALocalidadDC Pais { get; set; }

    [DataMember]
    public string CodigoPostal { get; set; }
  }
}