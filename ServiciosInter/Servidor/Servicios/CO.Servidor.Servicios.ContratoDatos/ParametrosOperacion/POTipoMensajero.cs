using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion los tipos de mensajeros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POTipoMensajero : DataContractBase
  {
    [DataMember]
    public int IdTipoMensajero { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public bool EsVehicular { get; set; }
  }
}