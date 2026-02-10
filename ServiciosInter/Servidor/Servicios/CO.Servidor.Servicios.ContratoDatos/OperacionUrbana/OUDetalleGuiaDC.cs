using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUDetalleGuiaDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "CiudadDestino")]
    public string CiudadDestino { get; set; }

    [DataMember]
    public string IdCiudadDestino { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RutaDestino", Description = "RutaDestino")]
    public string RutaDestino { get; set; }

    [DataMember]
    public string IdCiudadOrigen { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RutaDestino", Description = "RutaDestino")]
    public string CiudadOrigen { get; set; }

    [DataMember]
    public int IdRutaDestino { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoEnvio", Description = "TooltipTipoEnvio")]
    public string TipoEnvio { get; set; }

    /// <summary>
    /// Retorna o asigna el número de guía
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Guia", Description = "TooltipGuia")]
    public long NumeroGuia { get; set; }

    /// <summary>
    /// retorna o asigna el id del tipo de envio
    /// </summary>
    [DataMember]
    public short IdTipoEnvio { get; set; }

    /// <summary>
    /// retorna o asiga el background
    /// </summary>
    [DataMember]
    public string Background { get; set; }

    /// <summary>
    /// retorna o asigna el id del servicio
    /// </summary>
    [DataMember]
    public int IdServicio { get; set; }

    /// <summary>
    /// retorna o asiga el nombre del servicio
    /// </summary>
    [DataMember]
    public string NombreServicio { get; set; }
  }
}