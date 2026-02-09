using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  /// <summary>
  /// Reportes asociados a un Rol
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SERolReportes
  {
    [DataMember]
    /// <summary>
    /// Rol
    /// </summary>
    public SERol Rol { get; set; }

    [DataMember]
    /// <summary>
    /// Reportes del rol
    /// </summary>
    public ObservableCollection<REPInfoReporte> Reportes { get; set; }
  }
}