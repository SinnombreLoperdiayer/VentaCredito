using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de los tipos de los centros de servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUTipoCentroServicio : DataContractBase
  {
    [DataMember]
    public string IdTipo { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}