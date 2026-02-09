using System;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros
{
  /// <summary>
  /// Clase que contiene la informacion de la tbl de Admision Giros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GINumeroGiro : DataContractBase
  {
    [DataMember]
    public long? IdGiro { get; set; }

    [DataMember]
    public string CodVerfiGiro { get; set; }

    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    [DataMember]
    public string PrefijoIdGiro { get; set; }

    /// <summary>
    /// Indica si el usuario debe diligenciar el formato de declaracion voluntaria de fondos
    /// </summary>
    [DataMember]
    public bool ObligaDeclaracionVoluntariaFondos { get; set; }
  }
}