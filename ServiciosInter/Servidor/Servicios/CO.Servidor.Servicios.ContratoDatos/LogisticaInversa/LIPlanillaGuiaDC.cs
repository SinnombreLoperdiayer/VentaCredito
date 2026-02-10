using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  /// <summary>
  /// Clase con el DataContract de los archivos de logistica inversa
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIPlanillaDetalleDC
  {
    [DataMember]
    public long IdPlanillaGuia { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
    public long NumeroPlanilla { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public ADGuia AdmisionMensajeria { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public ADGuia AdmisionMensajeriaNueva { get; set; }

    [DataMember]
    public ADGuiaInternaDC GuiaInterna { get; set; }

    [DataMember]
    public int caja { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Fecha", Description = "Fecha")]
    public DateTime FechaGrabacion { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }

    [DataMember]
    public int DestinatarioModificado { get; set; }
  }
}