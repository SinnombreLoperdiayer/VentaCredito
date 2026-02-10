using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la información de los estados de la aplicación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAEstadoActivoInactivoDC : DataContractBase
  {
    [DataMember]
    public string IdEstado { get; set; }

    [DataMember]
    public string Estado { get; set; }
  }
}