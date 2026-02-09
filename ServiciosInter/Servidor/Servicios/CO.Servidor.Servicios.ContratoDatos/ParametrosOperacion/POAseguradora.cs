using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion de las aseguradoras
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POAseguradora : DataContractBase
  {
    [DataMember]
    public int IdAseguradora { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public string Identificacion { get; set; }
  }
}