using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Clase que contiene la informacion de las comisiones ganadas por una agencia por los puntos que administra
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CMComisionesServiciosCentroServiciosAdmin : DataContractBase
  {
    public event EventHandler OnValorPorcentajeCambio;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    public string NombreUnidadNegocio { get; set; }

    [DataMember]
    public string IdUnidadNegocio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "TooltipServicioComi")]
    public string NombreServicio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int? IdTipoComision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoComision", Description = "ToolTipTipoComision")]
    public string NombreComision { get; set; }

    [DataMember]
    public int? IdTipoComisionOriginal { get; set; }

    private decimal? porcentaje;

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

    [DataMember]
    public long IdCentroServicioServicio { get; set; }

    [DataMember]
    public long IdCentroServicioAdministrado { get; set; }

    [DataMember]
    public long? IdAgenciaAdministradora { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}