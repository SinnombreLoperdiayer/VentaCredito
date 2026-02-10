using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de los campos necesarios para adicionar un valor adicional
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TACampoTipoValorAdicionalDC : DataContractBase
  {
    [DataMember]
    public string IdTipoValorAdicional { get; set; }

    [DataMember]
    public string IdCampo { get; set; }

    [DataMember]
    public string Display { get; set; }

    [DataMember]
    public string TipoDato { get; set; }

    [DataMember]
    public string ValorCampo { get; set; }
  }
}