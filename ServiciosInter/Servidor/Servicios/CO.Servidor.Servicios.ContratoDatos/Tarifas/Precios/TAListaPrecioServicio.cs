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
  /// Clase que contiene la información
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAListaPrecioServicio : DataContractBase
  {
    [DataMember]
    public int IdListaPrecioServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "TooltipServListaPrecio")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio")]
    public string Servicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
    public string Estado { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreListaPrecio", Description = "TooltipNomListaPrecio")]
    public string NombreListaPrecio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Range(0, 100)]
    //[Range(1d, 10d, ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "RangoIncorrecto")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimaSeguro", Description = "TooltipPrimaSeguro")]
    public decimal PrimaSeguro { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    public TAUnidadNegocio UnidadNegocio { get; set; }
  }
}