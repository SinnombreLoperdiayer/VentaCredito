using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de precio del servicio internacional
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAServicioInternacionalPrecioDC : DataContractBase
  {
    [DataMember]
    public int IdPrecioInternacional { get; set; }

    [DataMember]
    public int IdListaPrecioServicio { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public PAZonaDC Zona { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public TATipoEmpaque TipoEmpaque { get; set; }

    [DataMember]
    [CamposOrdenamiento("PIN_Peso")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal Peso { get; set; }

    [DataMember]
    [CamposOrdenamiento("PIN_Valor")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal Valor { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public TAOperadorPostalDC OperadorPostal { get; set; }

    [DataMember]
    public IEnumerable<TAFormaPago> FormasPago { get; set; }

    [DataMember]
    public IList<PAZonaDC> ZonasDisponibles { get; set; }

    [DataMember]
    [CamposOrdenamiento("OPO_Nombre")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "OperadorPostal")]
    public string DescripcionOperadorPostal { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}