using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRComisionConceptoDCDeprecated : DataContractBase
  {
    [DataMember]
    public CAConceptoCajaDC ConceptoCaja { get; set; }

    [DataMember]
    public string IdUnidadNegocio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    public string DescripcionUnidadNegocio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    public string DescripcionConceptoLiquidacion { get; set; }

    [DataMember]
    public int IdTipoComision { get; set; }

    [DataMember]
    public string DescripcionTipoComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
    public int CantidadComisiones { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseComision")]
    public decimal ValorBaseComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Porcentaje")]
    public decimal PorcentajeComision { get; set; }

    [DataMember]
    public decimal Total { get; set; }
  }
}