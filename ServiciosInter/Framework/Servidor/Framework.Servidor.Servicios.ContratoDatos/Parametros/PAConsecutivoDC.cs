using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la información de consecutivo
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAConsecutivoDC : DataContractBase
  {
    [DataMember]
    public short IdConsecutivo { get; set; }

    [DataMember]
    public short IdTipoConsecutivo { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public long Inicio { get; set; }

    [DataMember]
    public long Fin { get; set; }

    [DataMember]
    public long Actual { get; set; }

    [DataMember]
    public bool EstadoActivo { get; set; }
  }
}