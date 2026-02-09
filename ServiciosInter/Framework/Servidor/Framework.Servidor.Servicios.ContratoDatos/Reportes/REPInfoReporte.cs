using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Reportes
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class REPInfoReporte
  {
    /// <summary>
    /// identificador único del reporte
    /// </summary>
    [DataMember]
    public int IdReporte { get; set; }

    /// <summary>
    /// identificador del módulo al cual pertenece el reporte
    /// </summary>
    [DataMember]
    public string IdModulo { get; set; }

    /// <summary>
    /// Nombre del reporte
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreReporte", Description = "NombreReporte")]
    public string NombreReporte { get; set; }

    /// <summary>
    /// Ruta relativa del reporte
    /// </summary>
    [DataMember]
    public string ReportPath { get; set; }

    /// <summary>
    /// Url del servidor de reportes
    /// </summary>
    [DataMember]
    public string ReportServerUrl { get; set; }

    /// <summary>
    /// Ruta relativa del reporte encriptada
    /// </summary>
    [DataMember]
    public string ReportPathYUrlEncriptados { get; set; }
  }
}