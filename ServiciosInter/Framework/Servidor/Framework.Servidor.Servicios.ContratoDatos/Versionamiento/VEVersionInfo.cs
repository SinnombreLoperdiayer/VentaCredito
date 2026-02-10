using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos
{
  /// <summary>
  /// Clase para manejo de información relacionada con la versión
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class VEVersionInfo : DataContractBase
  {
    /// <summary>
    /// Versión a mostrar al usuario final
    /// </summary>
    [DataMember]
    public string VersionComercial { get; set; }

    /// <summary>
    /// Binding de la version
    /// </summary>
    [DataMember]
    public string Binding { get; set; }

    /// <summary>
    /// Binding de la version
    /// </summary>
    [DataMember]
    public string Cultura { get; set; }

    /// <summary>
    /// Módulos de la versión
    /// </summary>
    [DataMember]
    public IEnumerable<VEModulo> Modulos { get; set; }

    /// <summary>
    /// Menus de Capacitacion de la versión
    /// </summary>
    [DataMember]
    public IEnumerable<VEMenuCapacitacion> MenusCapacitacion { get; set; }

    /// <summary>
    /// Datos que requeire el usuario autenticado durante su sesión
    /// </summary>
    [DataMember]
    public VEDatosInicioSesion DatosInicioSesion { get; set; }

    }
}