using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion de los detalles de los tipos de consolidado
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONTipoConsolidadoDetalle : DataContractBase
  {
    [DataMember]
    public int IdTipoConsolidadoDetalle { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public int IdTipoConsolidado { get; set; }
  }
}