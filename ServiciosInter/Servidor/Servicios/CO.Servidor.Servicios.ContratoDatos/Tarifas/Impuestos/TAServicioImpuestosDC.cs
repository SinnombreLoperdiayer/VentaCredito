using System.Collections.Generic;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase para la configuración de los impuestos de un servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAServicioImpuestosDC : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna los impuestos asignados a un servico
    /// </summary>
    [DataMember]
    public IList<TAImpuestoDelServicio> ImpuestosAsignados { get; set; }

    /// <summary>
    /// Retorna o asigna la información del servicio
    /// </summary>
    [DataMember]
    public TAServicioDC Servicio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public IEnumerable<TAImpuestosDC> Impuestos { get; set; }
  }
}