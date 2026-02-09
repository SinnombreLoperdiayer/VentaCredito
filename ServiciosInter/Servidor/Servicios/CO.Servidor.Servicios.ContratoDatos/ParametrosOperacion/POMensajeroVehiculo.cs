using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion los mensajeros de los vehiculos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POMensajeroVehiculo : DataContractBase
  {
    [DataMember]
    public long IdMensajero { get; set; }

    [DataMember]
    public int IdVehiculo { get; set; }
  }
}