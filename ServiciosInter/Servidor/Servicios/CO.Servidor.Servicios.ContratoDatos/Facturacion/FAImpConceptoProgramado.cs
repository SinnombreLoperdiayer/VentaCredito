using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FAImpConceptoProgramado
  {
    public FAImpConceptoProgramado()
    {
      BaseCalculo = 0;
      ValorPorc = 0;
    }

    [DataMember]
    public int IdConceptoProg { get; set; }

    [DataMember]
    public short IdImpuesto { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "Descripcion")]
    public string Descripcion { get; set; }

    [DataMember]
    public decimal BaseCalculo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Porcentaje", Description = "Porcentaje")]
    public decimal ValorPorc { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorTotal", Description = "ValorTotal")]
    public decimal Total
    {
      get
      {
        return BaseCalculo * ValorPorc / 100;
      }
      set
      {
      }
    }

    [DataMember]
    public System.DateTime FechaGrabacion { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }
  }
}