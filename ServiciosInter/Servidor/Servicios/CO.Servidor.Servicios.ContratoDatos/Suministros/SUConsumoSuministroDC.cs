using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Contiene la información asociada al consumo de un sumini
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUConsumoSuministroDC
  {
    [DataMember]
    public SUEnumSuministro Suministro { get; set; }

    [DataMember]
    public SUEnumGrupoSuministroDC GrupoSuministro { get; set; }

    [DataMember]
    public long IdDuenoSuministro { get; set; }

    [DataMember]
    public long NumeroSuministro { get; set; }

    [DataMember]
    public int Cantidad { get; set; }

    [DataMember]
    public SUEnumEstadoConsumo EstadoConsumo { get; set; }

    [DataMember]
    public int IdServicioAsociado { get; set; }
  }
}