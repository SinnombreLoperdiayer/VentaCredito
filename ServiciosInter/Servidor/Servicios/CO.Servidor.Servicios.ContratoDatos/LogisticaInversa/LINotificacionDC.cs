using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  /// <summary>
  /// Clase con el DataContract de descargue de notificaciones
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class LINotificacionDC
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public long IdAdmision { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
    public long? NumeroGuia { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Recibido { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Identificacion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Telefono { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string OtrosDatos { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public DateTime FechaEntrega { get; set; }
  }
}