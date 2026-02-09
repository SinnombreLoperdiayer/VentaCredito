using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la informacion de la Tbl DocsCambioDestino
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIDocsCambDestDC : DataContractBase
  {
    [DataMember]
    public Int16? IdDocCambDest { get; set; }

    [DataMember]
    public string DescrTipDocCambDest { get; set; }

    [DataMember]
    public DateTime FechaGrab { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }
  }
}