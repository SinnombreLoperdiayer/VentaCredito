using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de los Servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAServicioDC : DataContractBase
  {
    public event EventHandler OnUnidadNegocioCambio;
    public event EventHandler OnIdServicioCambio;

  
    private int idServicio;
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "Servicio")]
    public int IdServicio
    {
      get { return idServicio; }
      set {

        if (OnIdServicioCambio != null)
          OnIdServicioCambio("Anterior:"+idServicio.ToString() + "- Nuevo:" + value.ToString(),null);
        idServicio = value;
        }
    }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre")]
    [CamposOrdenamiento("SER_Nombre")]
    public string Nombre { get; set; }

    [DataMember]
    public string IdServicioERP { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    [CamposOrdenamiento("UNE_Descripcion")]
    public string UnidadNegocio { get; set; }

    private string idUnidadNegocio;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    public string IdUnidadNegocio
    {
      get { return idUnidadNegocio; }
      set
      {
        idUnidadNegocio = value;
        if (OnUnidadNegocioCambio != null)
          OnUnidadNegocioCambio(idUnidadNegocio, null);
      }
    }

    [DataMember]
    public string IdUnidadNegocioERP { get; set; }

    [DataMember]
    public int? TiempoEntrega { get; set; }

    [DataMember]
    public bool Asignado { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    /// <summary>
    /// Contiene el id del concepto de caja asociado al servicio
    /// </summary>
    [DataMember]
    public int IdConceptoCaja { get; set; }

    /// <summary>
    /// De acuerdo a la configuracioón del servicio, hay un peso mínimo que debe ser validado al momento de realizar admisión
    /// </summary>
    [DataMember]
    public decimal? PesoMinimo { get; set; }

    /// <summary>
    /// De acuerdo a la configuracioón del servicio, hay un peso máximo que debe ser validado al momento de realizar admisión
    /// </summary>
    [DataMember]
    public decimal? PesoMaximo { get; set; }
  }
}