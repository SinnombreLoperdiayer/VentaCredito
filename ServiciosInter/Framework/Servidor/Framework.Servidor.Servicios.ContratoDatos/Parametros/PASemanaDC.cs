using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de los dias de la semana
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PASemanaDC : DataContractBase
  {
    [DataMember]
    public short IdSemana { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}