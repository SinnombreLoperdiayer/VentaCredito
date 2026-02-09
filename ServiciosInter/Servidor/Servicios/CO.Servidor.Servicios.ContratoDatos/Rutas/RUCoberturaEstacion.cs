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
  /// Clase que contiene la informacion las ciudades (cobertura) que pertenecen a una estacion
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class RUCoberturaEstacion : DataContractBase
  {
    [DataMember]
    public PALocalidadDC EstacionHija { get; set; }

    [IgnoreDataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
    public PALocalidadDC EstacionHijaAdd { get; set; }

    [IgnoreDataMember]
    public PALocalidadDC PaisEstacionHijaAdd { get; set; }

    [DataMember]
    public int IdCoberturaEstacionRuta { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}