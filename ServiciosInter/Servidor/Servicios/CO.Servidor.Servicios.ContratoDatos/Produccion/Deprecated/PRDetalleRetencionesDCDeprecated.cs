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
  public class PRDetalleRetencionesDCDeprecated : DataContractBase
  {
    /// <summary>
    /// retorna o asigna el valor base de las comisiones para calcular la retencion
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseComision")]
    public decimal ValorBase { get; set; }

    /// <summary>
    /// retorna o asigna el valor configurado para el calculo de la retencion
    /// </summary>
    [DataMember]
    public decimal ValorBaseRetencion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorRetencion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Porcentaje")]
    public decimal PorcentajeRetencion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorFijo")]
    public decimal ValorFijo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorMonto")]
    public decimal ValorPorMonto { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public string DescripcionServicio { get; set; }

    [DataMember]
    public int IdTipoComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    public string DescripcionConceptoRetencion { get; set; }

    [DataMember]
    public string DescripcionTipoComision { get; set; }

    [DataMember]
    public CAConceptoCajaDC ConceptoCaja { get; set; }

    [DataMember]
    public PRMotivoNovedadDCDeprecated Novedad { get; set; }

    [DataMember]
    public int IdRetencion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Retencion")]
    public string Retencion { get; set; }
  }
}