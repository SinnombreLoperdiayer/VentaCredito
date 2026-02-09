using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion de las lineas de los vehiculos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POLinea : DataContractBase
  {
    [DataMember]
    public int IdLineaVehiculo { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}