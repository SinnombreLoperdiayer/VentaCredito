using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRLiquidacionManualDCDeprecated : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdLiquidacion")]
    public long IdLiquidacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
    public long IdCentroServicios { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AgenciaPunto")]
    public string NombreCentroServicios { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaLiquidacion")]
    public DateTime FechaLiquidacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaAprobacion")]
    public DateTime FechaAprobacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCorte")]
    public DateTime FechaCorte { get; set; }

    [DataMember]
    public List<PRComisionConceptoDCDeprecated> ComisionXConcepto { get; set; }

    [DataMember]
    public List<CMComisionesConceptosAdicionales> ComisionesFijas { get; set; }

    [DataMember]
    public List<PRComisionPuntosDCDeprecated> ComisionesPuntos { get; set; }

    [DataMember]
    public List<PRDetalleNovedadDCDeprecated> NovedadReintegro { get; set; }

    [DataMember]
    public List<PRDetalleNovedadDCDeprecated> NovedadDescuento { get; set; }

    [DataMember]
    public List<PRDetalleRetencionesDCDeprecated> RetencionesConcepto { get; set; }

    [DataMember]
    public List<PRDetalleRetencionesDCDeprecated> RetencionesNovedad { get; set; }

    [DataMember]
    public decimal TotalComisiones { get; set; }

    [DataMember]
    public decimal TotalComisionesPuntos { get; set; }

    [DataMember]
    public decimal TotalComisionesFijas { get; set; }

    [DataMember]
    public decimal TotalNovedadReintegro { get; set; }

    [DataMember]
    public decimal TotalNovedadDescuento { get; set; }

    [DataMember]
    public decimal TotalRetencionesConcepto { get; set; }

    [DataMember]
    public decimal TotalRetencionesNovedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalPagos")]
    public decimal TotalPagos { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalDeducciones")]
    public decimal TotalDeducciones { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAcumuladoCaja")]
    public decimal SaldoCaja { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalFavorInter")]
    public decimal TotalFavorInter { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalFavorAgencia")]
    public decimal TotalFavorAgencia { get; set; }

    [DataMember]
    public CMComisionesConceptosAdicionales ComisionFija { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Aprobada")]
    public bool EstaAprobada { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioAprobacion")]
    public string UsuarioAprobacion { get; set; }

    [DataMember]
    public long IdRacolLiquidacion { get; set; }

    [DataMember]
    public string IdPaisDefecto { get; set; }

    [DataMember]
    public string NombrePaisDefecto { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuiaInterna")]
    public long NumeroGuiaInterna { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGiro")]
    public long NumeroGiro { get; set; }

    [DataMember]
    public string PrefijoGiro { get; set; }

    [DataMember]
    public bool Imprimir { get; set; }

    [DataMember]
    public long IdProgramacion { get; set; }
  }
}