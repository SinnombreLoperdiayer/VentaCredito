using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRDetalleNovedadDCDeprecated : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal Valor { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo")]
    public string Motivo { get; set; }

    [DataMember]
    public int IdMotivo { get; set; }

    [DataMember]
    public decimal Total { get; set; }
  }
}