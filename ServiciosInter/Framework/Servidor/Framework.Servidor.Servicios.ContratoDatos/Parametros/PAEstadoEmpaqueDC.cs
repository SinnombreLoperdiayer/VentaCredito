using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la información de los estados de los empaques
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAEstadoEmpaqueDC
  {
    /// <summary>
    /// retorna o asigna el id del estado del empaque
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "EstadoEmpaque", Description = "EstadoEmpaque")]
    public short IdEstadoEmpaque { get; set; }

    /// <summary>
    /// retorna o asigna el id del estado del empaque
    /// </summary>
    [DataMember]
    public string DescripcionEstado { get; set; }

    /// <summary>
    /// Retorna o asigna el peso inicial del estado del empaque seleccionado
    /// </summary>
    [DataMember]
    public decimal PesoInicial { get; set; }

    /// <summary>
    /// Retorna o asigna el peso final del estado del empaque seleccionado
    /// </summary>
    [DataMember]
    public decimal PesoFinal { get; set; }
  }
}