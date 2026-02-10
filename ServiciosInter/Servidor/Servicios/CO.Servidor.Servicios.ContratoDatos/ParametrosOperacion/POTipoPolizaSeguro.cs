using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion de los tipos de poliza de seguro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POTipoPolizaSeguro : DataContractBase
  {
    [DataMember]
    public string IdTipoPoliza { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}