using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
  /// <summary>
  /// Clase que contiene la informacion de una ciudad que se manifesta en una ruta
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class RUCiudadManifestadaEnRuta : DataContractBase
  {
    [DataMember]
    public int IdCiudadManifiestaEnRuta { get; set; }

    [DataMember]
    public int IdRuta { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
    public PALocalidadDC CiudadManifiestaRuta { get; set; }

    [IgnoreDataMember]
    public PALocalidadDC PaisCiudadManifiestaRuta { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "GeneraConsolidado", Description = "ToolTipGeneraConsolidado")]
    public bool GeneraConsolidado { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}