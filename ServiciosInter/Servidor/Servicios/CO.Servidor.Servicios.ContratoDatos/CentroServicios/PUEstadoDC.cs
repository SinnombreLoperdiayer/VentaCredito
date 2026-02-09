using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de los estados de los centros logisticos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUEstadoDC : DataContractBase
  {
    [DataMember]
    public string IdEstado { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}