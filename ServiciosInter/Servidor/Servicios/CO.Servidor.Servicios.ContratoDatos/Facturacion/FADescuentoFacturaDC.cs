using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FADescuentoFacturaDC
  {
    [DataMember]
    public long NumeroFactura { get; set; }

    [DataMember]
    public short IdDescuento { get; set; }

    [DataMember]
    public decimal Valor { get; set; }

    [DataMember]
    public string Motivo { get; set; }

    [DataMember]
    public System.DateTime FechaGrabacion { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }
  }
}