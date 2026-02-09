using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.ListaPrecios
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioMensajariaExpresaDC
  {
    [DataMember]
    public int IdListaPrecios { get; set; }
    [DataMember]
    public int IdServicio { get; set; }
    [DataMember]
    public IList<TAPrecioTrayectoControllerDC> PrecioTrayecto { get; set; }
  }

  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioTrayectoControllerDC
  {
    [DataMember]
    public long IdPrecioTrayecto { get; set; }
    [DataMember]
    public int IdTipoTrayectoSubTrayecto { get; set; }
    [DataMember]
    public string IdTipoTrayecto { get; set; }
    [DataMember]
    public string IdTipoSubTrayecto { get; set; }
    [DataMember]
    public decimal Valor { get; set; }
  }
}
