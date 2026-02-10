using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADRapiEnvioContraPagoDC
  {
    //[Required(ErrorMessageResourceType = typeof(Framework.Cliente.Servicios.ContratoDatos.Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PaisDestino", Description = "TooltipPaisDestino")]
    [DataMember]
    public PALocalidadDC PaisDestino { get; set; }

    //[Required(ErrorMessageResourceType = typeof(Framework.Cliente.Servicios.ContratoDatos.Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "TooltipCiudadDestino")]
    [DataMember]
    public PALocalidadDC CiudadDestino { get; set; }

    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoDestino", Description = "TooltipTipoDestino")]
    [DataMember]
    public TATipoDestino TipoDestino { get; set; }

    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorARecaudar", Description = "TooltipValorARecaudar")]
    [DataMember]
    public decimal ValorARecaudar { get; set; }

    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DescripcionProducto", Description = "TooltipDescripcionProducto")]
    [DataMember]
    public string DescripcionProducto { get; set; }

    [DataMember]
    public string TipoIdentificacionDestinatario { get; set; }

    [DataMember]
    public string IdentificacionDestinatario { get; set; }

    [DataMember]
    public string NombreDestinatario { get; set; }

    [DataMember]
    public string Apellido1Destinatario { get; set; }

    [DataMember]
    public string Apellido2Destinatario { get; set; }

    [DataMember]
    public string TelefonoDestinatario { get; set; }

    [DataMember]
    public string DireccionDestinatario { get; set; }

    [DataMember]
    public string EmailDestinatario { get; set; }
  }
}