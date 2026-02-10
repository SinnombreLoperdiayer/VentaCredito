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
  public class PRComisionPuntosDCDeprecated : DataContractBase
  {
    [DataMember]
    public long IdPuntosServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
    public string NombrePuntoServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseComision")]
    public decimal ValorBase { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorComision { get; set; }

    [DataMember]
    public decimal Total { get; set; }
  }
}