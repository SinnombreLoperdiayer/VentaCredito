using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de los tipos de cuenta
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUTipoCuenta : DataContractBase
  {
    [DataMember]
    public string IdTipoCuenta { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}