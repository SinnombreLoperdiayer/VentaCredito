using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  /// <summary>
  /// Clase que contiene la informacion del estado del usuario
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEEstadoUsuario : DataContractBase
  {
    [DataMember]
    public string IdEstado { get; set; }

    [DataMember]
    public string EstadoUsuario { get; set; }
  }
}