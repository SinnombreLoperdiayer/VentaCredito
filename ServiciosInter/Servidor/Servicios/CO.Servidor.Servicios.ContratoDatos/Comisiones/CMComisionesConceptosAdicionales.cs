using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Clase que contiene la informacion de los conceptos adicionales (comisiones fijas)
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CMComisionesConceptosAdicionales : DataContractBase
  {
    [DataMember]
    public long IdCentroServicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoComisionFija", Description = "ToolTipTipoComisionFija")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int? IdTipoComisionFija { get; set; }

    [DataMember]
    public int? IdTipoComisionFijaOriginal { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "ToolTipDescripcionConceptoAd")]
    [Filtrable("CSD_Descripcion", "Descripción:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Descripcion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "ToolTipValorComisionFija")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Retorna o asigna el valor a pagar al centro svc por la comision fija
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorComision", Description = "ValorComision")]
    public decimal ValorComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicio", Description = "ToolTipFechaInicioComisionFija")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public DateTime FechaInicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public bool Estado { get; set; }

    [DataMember]
    public long IdCentroSrvComiFijaContrato { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Contrato", Description = "ToolTipContratoComFija")]
    public int IdContrato { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Contrato", Description = "ToolTipContratoComFija")]
    [Filtrable("CON_NombreContrato", "Nombre Contrato:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    public string NombreContrato { get; set; }

    [DataMember]
    public DateTime FechaTeminacionContrato { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cliente", Description = "TooltipCliente")]
    [Filtrable("CLI_RazonSocial", "Cliente:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    public string RazonSocialCliente { get; set; }

    [DataMember]
    public int IdClienteCredito { get; set; }

    [DataMember]
    public bool ConContrato { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    /// <summary>
    /// Retorna o asigna los dias habiles para calcular la comision
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DiasHabiles")]
    public int DiasHabiles { get; set; }
  }
}