using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADRecaudoAlCobro
  {
    [DataMember]
    public string NumeroConsecutivo { get; set; }

    [DataMember]
    public string NitEmpresa { get; set; }

    [DataMember]
    public string IdCentroCosto { get; set; }
  }
}
