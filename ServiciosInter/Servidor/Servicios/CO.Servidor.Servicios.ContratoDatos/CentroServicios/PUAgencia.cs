using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de las agencias
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUAgencia : DataContractBase
  {
    [DataMember]
    public PUCentroServiciosDC CentroServicios { get; set; }

    [DataMember]
    public long IdAgencia { get; set; }

    [DataMember]
    public string IdTipoAgencia { get; set; }

    [DataMember]
    public bool TienePuntosACargo { get; set; }

    [DataMember]
    public string IdLocalidad { get; set; }
  }
}