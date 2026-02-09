using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos
{
  /// <summary>
  /// Clase que contiene la información del Módulo de la aplicación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class VEModulo : DataContractBase
  {
    /// <summary>
    /// Identificador del módulo
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MOD_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulo", Description = "TooltipModuloFalla")]
    public string IdModulo { get; set; }

    /// <summary>
    /// Descripción del módulo
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("MOD_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulo", Description = "TooltipModuloFalla")]
    public string Descripcion { get; set; }

    /// <summary>
    /// URL del servicio del módulo
    /// </summary>
    [DataMember]
    public string UrlServicio { get; set; }

    /// <summary>
    /// URL de la aplicación del módulo
    /// </summary>
    [DataMember]
    public string UrlAplicacion { get; set; }

    /// <summary>
    /// Retorna o asigna el tipo del proxy del servicio
    /// </summary>
    [DataMember]
    public string TipoProxy { get; set; }
  }
}