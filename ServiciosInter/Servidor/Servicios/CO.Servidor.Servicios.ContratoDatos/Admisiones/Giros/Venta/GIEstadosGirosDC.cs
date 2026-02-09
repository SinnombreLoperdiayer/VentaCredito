using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta
{
  /// <summary>
  /// Clase que relaciona los estados de un Giro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIEstadosGirosDC : DataContractBase
  {
    /// <summary>
    /// Es el Numero del Giro
    /// </summary>
    [DataMember]
    public long idGiro { get; set; }

    /// <summary>
    /// Es el id interno del numero del giro
    /// </summary>
    [DataMember]
    public long IdAdminGiro { get; set; }

    /// <summary>
    /// Es el estado del giro
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "EstadoGiro")]
    public string EstadoGiro { get; set; }

    /// <summary>
    /// Es la fecha en la que cambio el estado el giro
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaCambioEstado")]
    public DateTime FechaCambioEstado { get; set; }

    /// <summary>
    /// Es el usuario que actualizo el estado
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
    public string Usuario { get; set; }
  }
}