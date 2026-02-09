using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
  /// <summary>
  /// Clase que contiene la informacion frecuencia de las rutas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class RUFrecuenciaRuta : DataContractBase
  {
    [DataMember]
    public int IdFrecuenciaRuta { get; set; }

    [DataMember]
    public int IdRuta { get; set; }

    [DataMember]
    public string IdDia { get; set; }

    [DataMember]
    public DateTime HoraSalida { get; set; }

    [DataMember]
    public DateTime HoraLlegada { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}