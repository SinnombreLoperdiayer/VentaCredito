using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion de las categorias de licencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POCategoriaLicencia : DataContractBase
  {
    [DataMember]
    public int IdCategoriaLicencia { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}