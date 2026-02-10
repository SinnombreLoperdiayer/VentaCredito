using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion de los vehiculos asociados a un racol
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PORacolVehiculo : DataContractBase
  {
    [DataMember]
    public long IdRacol { get; set; }

    [DataMember]
    public int IdVehiculo { get; set; }

    [DataMember]
    public string NombreRacol { get; set; }
  }
}