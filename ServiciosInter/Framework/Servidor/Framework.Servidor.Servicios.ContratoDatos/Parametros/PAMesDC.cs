using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de los meses del año
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAMesDC : DataContractBase
  {
    [DataMember]
    public short IdMes { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}