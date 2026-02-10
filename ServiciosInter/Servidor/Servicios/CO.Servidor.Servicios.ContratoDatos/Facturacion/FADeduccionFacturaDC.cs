using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FADeduccionFacturaDC
  {
    [DataMember]
    public int IdDeduccion { get; set; }

    [DataMember]
    public long NumeroFactura { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public decimal BaseCalculo { get; set; }

    [DataMember]
    public decimal ValorPorMonto { get; set; }

    [DataMember]
    public decimal ValorFijo { get; set; }

    [DataMember]
    public decimal TarifaPorcentual { get; set; }

    [DataMember]
    public decimal TotalCalculado { get; set; }

    [DataMember]
    public System.DateTime FechaGrabacion { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }
  }
}