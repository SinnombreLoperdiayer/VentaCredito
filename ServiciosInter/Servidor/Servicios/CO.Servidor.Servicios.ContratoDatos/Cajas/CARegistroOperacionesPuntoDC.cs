using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase de informacion de Registro de
  /// operaciones en el punto ó agencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroOperacionesPuntoDC : DataContractBase
  {
    /// <summary>
    /// Es el concepto de Caja Registrado
    /// </summary>
    /// <value>
    /// The concepto caja.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public CAConceptoCajaDC ConceptoDeCaja { get; set; }

    /// <summary>
    /// Es el valor reportado por el
    /// punto ó agencia
    /// </summary>
    /// <value>
    /// The valor reportado.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public decimal ValorRegistrado { get; set; }

    /// <summary>
    /// observación adjunta al Registro de la
    /// operación
    /// </summary>
    /// <value>
    /// The obsevacion.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones")]
    public string Observacion { get; set; }
  }
}