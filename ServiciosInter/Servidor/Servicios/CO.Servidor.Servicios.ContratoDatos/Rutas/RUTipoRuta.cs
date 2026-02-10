using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
  /// <summary>
  /// Clase que contiene la informacion de los tipos de ruta
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class RUTipoRuta : DataContractBase
  {
    [DataMember]
    public int IdTipoRuta { get; set; }

    [DataMember]
    public string NombreTipoRuta { get; set; }
  }
}