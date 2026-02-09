using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Contiene la informacion de la Ruta y del Conductor
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONRutaConductorDC
  {
    [DataMember]
    public RURutaDC Ruta { get; set; }

    [DataMember]
    public POConductores Conductor { get; set; }

    [DataMember]
    public int IdVehiculo { get; set; }
    
    [DataMember]
    public int IdTipoVehiculo { get; set; }
  }
}