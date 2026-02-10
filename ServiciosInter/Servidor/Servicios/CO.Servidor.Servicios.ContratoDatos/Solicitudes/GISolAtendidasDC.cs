using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la informacion de la tbl Solicitud Giro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GISolAtendidasDC : DataContractBase
  {
    /// <summary>
    /// Es le numero de la Solicitud
    /// </summary>
    [DataMember]
    public long? IdSolicitudAtendida { get; set; }

    [DataMember]
    public string UsuarioSolAtendio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioAtiendeSol", Description = "UsuarioAtiendeSol")]
    public string NombreUsuarioSolAten { get; set; }

    [DataMember]
    public DateTime? FechaSolAten { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ObservaSolAtendida", Description = "ToolTipObservaSolAtendida")]
    public string ObservSolAten { get; set; }

    [DataMember]
    public string EstadoSolicitud { get; set; }

    /// <summary>
    /// Es el numero Autogenerado
    /// para la atencion de una solicitud
    /// </summary>
    [DataMember]
    public long NumeroAtencion { get; set; }
  }
}