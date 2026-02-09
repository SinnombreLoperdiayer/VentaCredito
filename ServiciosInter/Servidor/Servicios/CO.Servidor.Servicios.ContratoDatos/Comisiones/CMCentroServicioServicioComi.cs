using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Clase que contiene las comisiones por servicio de un centro de servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CMCentroServicioServicioComi : DataContractBase
  {
    public event EventHandler OnValorPorcentajeCambio;

    [DataMember]
    public long? IdCentroServicioServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    public string NombreUnidadNegocio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int? IdTipoComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoComision", Description = "ToolTipTipoComision")]
    public string NombreTipoComision { get; set; }

    private decimal? porcentaje;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "Servicio")]
    public int IdServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "Servicio")]
    public string NombreServicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Porcentaje", Description = "TooltipPorcentajeComision")]
    public decimal? Porcentaje
    {
      get { return porcentaje; }
      set
      {
        porcentaje = value;
        if (porcentaje != null && porcentaje.Value != 0)
        {
          this.Valor = 0;
          if (this.OnValorPorcentajeCambio != null)
            this.OnValorPorcentajeCambio(null, null);
        }
      }
    }

    private decimal? valor;

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "TooltipValorComision")]
    public decimal? Valor
    {
      get { return valor; }
      set
      {
        valor = value;
        if (valor != null && valor.Value != 0)
        {
          this.Porcentaje = 0;
          if (this.OnValorPorcentajeCambio != null)
            this.OnValorPorcentajeCambio(null, null);
        }
      }
    }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}