using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de las facturas por contrato
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLFacturaNotacionDC : DataContractBase
  {
    [DataMember]
    public short IdNotacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Tipo")]
    public short IdTipo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Semana")]
    public short IdSemana { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Dia")]
    public string IdDia { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DiaOrdinal")]
    public short DiaOrdinal { get; set; }

    [DataMember]
    public string NombreDiaOrdinal { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}