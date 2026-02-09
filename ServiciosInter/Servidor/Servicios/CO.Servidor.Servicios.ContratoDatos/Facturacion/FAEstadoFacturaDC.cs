using CO.Cliente.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FAEstadoFacturaDC
  {
    [DataMember]
    public long NumeroFactura { get; set; }

    [DataMember]
    public long ValorFactura { get; set; }

    [DataMember]
    public string Estado { get; set; }

    [DataMember]
    public string DescripcionEstado { get; set; }

    [DataMember]
    public FAMotivoAnulacionDC Motivo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "Observaciones")]
    public string Observaciones { get; set; }

    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    [DataMember]
    public FANotaFacturaDC NotaFactura { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }
  }
}