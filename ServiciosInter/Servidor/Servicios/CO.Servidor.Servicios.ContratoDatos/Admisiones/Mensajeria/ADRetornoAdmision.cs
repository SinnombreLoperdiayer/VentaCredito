using System;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADRetornoAdmision
  {
    [DataMember]
    public long NumeroGuia { get; set; }

    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    [DataMember]
    public string PrefijoGuia { get; set; }

    /// <summary>
    /// Campo exclusivo para el cargue masivo de guias
    /// </summary>
    [DataMember]
    public string Error { get; set; }

    /// <summary>
    /// Campo exclusivo para el cargue masivo de guias
    /// </summary>
    [DataMember]
    public int NoFila { get; set; }

    /// <summary>
    /// Indica si se debe generar advertencia de porcentaje de cupo superado. Aplica para cliente crédito
    /// </summary>
    [DataMember]
    public bool? AdvertenciaPorcentajeCupoSuperadoClienteCredito { get; set; }

    [DataMember]
    public string DireccionAgenciaCiudadOrigen { get; set; }

    [DataMember]
    public string DireccionAgenciaCiudadDestino { get; set; }
  }
}