using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Contiene la información necesaria para hacer un traslado de suministros entre un origen y un destino
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUTrasladoSuministroDC
  {
    [DataMember]
    public SUEnumSuministro Suministro { get; set; }

    [DataMember]
    public SUEnumGrupoSuministroDC GrupoSuministroOrigen { get; set; }

    [DataMember]
    public SUEnumGrupoSuministroDC GrupoSuministroDestino { get; set; }

    [DataMember]
    public long IdentificacionOrigen { get; set; }

    [DataMember]
    public long IdentificacionDestino { get; set; }

    [DataMember]
    public long NumeroSuministro { get; set; }

    [DataMember]
    public int Cantidad { get; set; }

    [DataMember]
    public SUSuministro SuministroTraslado { get; set; }
  }
}