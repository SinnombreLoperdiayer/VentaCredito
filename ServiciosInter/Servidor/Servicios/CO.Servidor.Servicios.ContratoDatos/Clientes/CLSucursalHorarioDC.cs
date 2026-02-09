using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase que contiene la informacion de los horarios de una sucursal
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLSucursalHorarioDC : DataContractBase
  {
    [DataMember]
    public int IdSucursalContrato { get; set; }

    [DataMember]
    public string IdDia { get; set; }
    [DataMember]
    public string NombreDia { get; set; }

    [DataMember]
    public DateTime Hora { get; set; }
  }
}